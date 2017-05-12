using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.Threading.Tasks;

using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using RazorEngine.Text;

using HTMLxUnitGenerator.Model;
using HTMLxUnitGenerator.Utils;
using HTMLxUnitGenerator.Logging;
using System.Globalization;
using HTMLxUnitGenerator.Parser.Nunit;

namespace HTMLxUnitGenerator.Parser
{
    internal class NUnit : IParser
    {
        private string resultsFile;

        private Logger logger = Logger.GetLogger();

        public Report Parse(string resultsFile)
        {
            this.resultsFile = resultsFile;

            XDocument doc = XDocument.Load(resultsFile);

            Report report = new Report();

            report.FileName = Path.GetFileNameWithoutExtension(resultsFile);
            report.RunInfo.AssemblyName = doc.Root.Attribute("name") != null ? doc.Root.Attribute("name").Value : null;
            report.RunInfo.TestRunner = TestRunner.NUnit;

            // run-info & environment values -> RunInfo
            var runInfo = CreateRunInfo(doc);

            if (runInfo != null)
            {
                report.RunInfo.AddInfo(runInfo);
            }

            // report counts
            report.Total = doc.Descendants("test-case").Count();

            report.Passed =
                doc.Root.Attribute("passed") != null
                    ? Int32.Parse(doc.Root.Attribute("passed").Value)
                    : doc.Descendants("test-case").Where(x => x.Attribute("result").Value.Equals("success", StringComparison.CurrentCultureIgnoreCase)).Count();

            report.Failed =
                doc.Root.Attribute("failed") != null
                    ? Int32.Parse(doc.Root.Attribute("failed").Value)
                    : Int32.Parse(doc.Root.Attribute("failures").Value);

            report.Errors =
                doc.Root.Attribute("errors") != null
                    ? Int32.Parse(doc.Root.Attribute("errors").Value)
                    : 0;

            report.Inconclusive =
                doc.Root.Attribute("inconclusive") != null
                    ? Int32.Parse(doc.Root.Attribute("inconclusive").Value)
                    : Int32.Parse(doc.Root.Attribute("inconclusive").Value);

            report.Skipped =
                doc.Root.Attribute("skipped") != null
                    ? Int32.Parse(doc.Root.Attribute("skipped").Value)
                    : Int32.Parse(doc.Root.Attribute("skipped").Value);

            report.Skipped +=
                doc.Root.Attribute("ignored") != null
                    ? Int32.Parse(doc.Root.Attribute("ignored").Value)
                    : 0;

            // report start time
            report.StartTime =
                doc.Root.Attribute(NunitAttributeName.START_TIME) != null
                    ? doc.Root.Attribute(NunitAttributeName.START_TIME).Value.ToDateTime()
                    : (doc.Root.Attribute("date").Value + " " + doc.Root.Attribute("time").Value).ToDateTime();

            //report end time
            report.EndTime =
                doc.Root.Attribute(NunitAttributeName.END_TIME) != null
                    ? doc.Root.Attribute(NunitAttributeName.END_TIME).Value.ToDateTime()
                    : default(DateTime);

            //report total duration
            report.Duration =
                doc.Root.Attribute(NunitAttributeName.DURATION) == null ? 0 : doc.Root.Attribute(NunitAttributeName.DURATION).Value.ToDouble();

            // report status messages
            var testSuiteTypeAssembly = doc.Descendants("test-suite")
                .Where(x => x.Attribute("result").Value.Equals("Failed") && x.Attribute("type").Value.Equals("Assembly"));
            report.StatusMessage = testSuiteTypeAssembly != null && testSuiteTypeAssembly.Count() > 0
                ? testSuiteTypeAssembly.First().Value
                : "";

            IEnumerable<XElement> suites = doc
                .Descendants("test-suite")
                .Where(x => x.Attribute("type").Value.Equals("TestFixture", StringComparison.CurrentCultureIgnoreCase));

            suites.AsParallel().ToList().ForEach(testSuiteElement =>
            {
                TestSuite testSuite = GetAttributesForSuite(testSuiteElement);

                testSuite.TestCasesLink = GetTestCaseLink(testSuiteElement);

                // any error messages and/or stack-trace
                var failure = testSuiteElement.Element("failure");
                if (failure != null)
                {
                    var message = failure.Element("message");
                    if (message != null)
                    {
                        testSuite.StatusMessage = message.Value;
                    }

                    var stackTrace = failure.Element("stack-trace");
                    if (stackTrace != null && !string.IsNullOrWhiteSpace(stackTrace.Value))
                    {
                        testSuite.StatusMessage = string.Format(
                            "{0}\n\nStack trace:\n{1}", testSuite.StatusMessage, stackTrace.Value);
                    }
                }

                var output = testSuiteElement.Element("output")?.Value;
                if (!string.IsNullOrWhiteSpace(output))
                {
                    testSuite.StatusMessage += $"\n\nOutput:\n" + output;
                }

                // get test suite level categories
                var suiteCategories = this.GetCategories(testSuiteElement, false);

                // Test Cases
                testSuiteElement.Descendants("test-case").AsParallel().ToList().ForEach(testCaseElement =>
                {
                    var test = new Test();

                    test.Name = testCaseElement.Attribute("name").Value;
                    test.Status = StatusExtensions.ToStatus(testCaseElement.Attribute("result").Value);

                    // main a master list of all status
                    // used to build the status filter in the view
                    report.StatusList.Add(test.Status);

                    // TestCase Time Info
                    test.StartTime =
                        testCaseElement.Attribute(NunitAttributeName.START_TIME) != null
                            ? testCaseElement.Attribute(NunitAttributeName.START_TIME).Value.ToDateTime()
                            : default(DateTime);
                    test.StartTime =
                        test.StartTime.Equals(default(DateTime)) && (testCaseElement.Attribute("time") != null)
                            ? testCaseElement.Attribute("time").Value.ToDateTime()
                            : test.StartTime;
                    test.EndTime =
                        testCaseElement.Attribute(NunitAttributeName.END_TIME) != null
                            ? testCaseElement.Attribute(NunitAttributeName.END_TIME).Value.ToDateTime()
                            : default(DateTime);

                    test.Duration =
                    testCaseElement.Attribute(NunitAttributeName.DURATION) == null ? 0 : testCaseElement.Attribute(NunitAttributeName.DURATION).Value.ToDouble();

                    // description
                    var description =
                        testCaseElement.Descendants("property")
                        .FirstOrDefault(c => c.Attribute("name").Value.Equals("Description", StringComparison.CurrentCultureIgnoreCase));

                    test.Description = description == null ? String.Empty : description.Attribute("value").Value;

                    // link on image with error
                    var errorImageLink = testCaseElement.Descendants("property")
                        .FirstOrDefault(c => c.Attribute("name").Value.Equals("LinkOnImageWithError", StringComparison.CurrentCultureIgnoreCase));
                    test.ImageExceptionLink =
                        errorImageLink == null ? String.Empty : errorImageLink.Attribute("value").Value;


                    // get test case level categories
                    var categories = this.GetCategories(testCaseElement, true);

                    // if this is a parameterized test, get the categories from the parent test-suite
                    var parameterizedTestElement = testCaseElement
                        .Ancestors("test-suite").ToList()
                        .Where(x => x.Attribute("type").Value.Equals("ParameterizedTest", StringComparison.CurrentCultureIgnoreCase))
                        .FirstOrDefault();

                    if (null != parameterizedTestElement)
                    {
                        var paramCategories = this.GetCategories(parameterizedTestElement, false);
                        categories.UnionWith(paramCategories);
                    }

                    //Merge test level categories with suite level categories and add to test and report
                    categories.UnionWith(suiteCategories);
                    test.CategoryList.AddRange(categories);
                    report.CategoryList.AddRange(categories);


                    // error and other status messages
                    test.StatusMessage =
                        testCaseElement.Element("failure") != null
                            ? testCaseElement.Element("failure").Element("message").Value.Trim()
                            : "";
                    test.StatusMessage +=
                        testCaseElement.Element("failure") != null
                            ? testCaseElement.Element("failure").Element("stack-trace") != null
                                ? testCaseElement.Element("failure").Element("stack-trace").Value.Trim()
                                : ""
                            : "";

                    test.StatusMessage += testCaseElement.Element("reason") != null && testCaseElement.Element("reason").Element("message") != null
                        ? testCaseElement.Element("reason").Element("message").Value.Trim()
                        : "";

                    // add NUnit console output to the status message
                    test.StatusMessage += testCaseElement.Element("output") != null
                      ? testCaseElement.Element("output").Value.Trim()
                      : "";

                    testSuite.TestList.Add(test);
                });

                testSuite.Status = ReportUtil.GetFixtureStatus(testSuite.TestList);

                report.TestSuiteList.Add(testSuite);
            });

            //Sort category list so it's in alphabetical order
            report.CategoryList.Sort();

            return report;
        }

        private string GetTestCaseLink(XElement ts)
        {
            var propertiesAttribute =
                                    ts.Element("properties");

            XElement testCaseLink = null;

            if (propertiesAttribute != null && !propertiesAttribute.IsEmpty)
            {
                testCaseLink = propertiesAttribute.Descendants("property")
                    .FirstOrDefault(c => c.Attribute("name").Value.Equals("TestCaseLinkProperty", StringComparison.CurrentCultureIgnoreCase));
            }

            return testCaseLink != null ? testCaseLink.Attribute("value").Value : String.Empty;
        }

        private TestSuite GetAttributesForSuite(XElement ts)
        {
            var testSuite = new TestSuite();
            testSuite.Name = ts.Attribute("name").Value;

            // Suite Time Info
            testSuite.StartTime =
                ts.Attribute(NunitAttributeName.START_TIME) != null
                    ? ts.Attribute(NunitAttributeName.START_TIME).Value.ToDateTime()
                    : default(DateTime);

            testSuite.StartTime =
                testSuite.StartTime.Equals(default(DateTime)) && ts.Attribute("time") != null
                    ? ts.Attribute("time").Value.ToDateTime()
                    : testSuite.StartTime;

            testSuite.EndTime =
                ts.Attribute(NunitAttributeName.END_TIME) != null
                    ? ts.Attribute(NunitAttributeName.END_TIME).Value.ToDateTime()
                    : default(DateTime);

            testSuite.Duration =
                ts.Attribute(NunitAttributeName.DURATION) == null ? 0 : ts.Attribute(NunitAttributeName.DURATION).Value.ToDouble();

            return testSuite;
        }

        /// <summary>
        /// Returns categories for the direct children or all descendents of an XElement
        /// </summary>
        /// <param name="elem">XElement to parse</param>
        /// <param name="allDescendents">If true, return all descendent categories.  If false, only direct children</param>
        /// <returns></returns>
        private HashSet<string> GetCategories(XElement elem, bool allDescendents)
        {
            //Define which function to use
            var parser = allDescendents
                ? new Func<XElement, string, IEnumerable<XElement>>((e, s) => e.Descendants(s))
                : new Func<XElement, string, IEnumerable<XElement>>((e, s) => e.Elements(s));

            //Grab unique categories
            HashSet<string> categories = new HashSet<string>();
            bool hasCategories = parser(elem, "categories").Any();
            if (hasCategories)
            {
                List<XElement> cats = parser(elem, "categories").Elements("category").ToList();

                cats.ForEach(x =>
                {
                    string cat = x.Attribute("name").Value;
                    categories.Add(cat);
                });
            }

            return categories;
        }

        private Dictionary<string,string> CreateRunInfo(XDocument doc)
        {
            var environment = doc.Descendants("environment").FirstOrDefault();

            if (environment == null)
                return null;

            Dictionary<string,string> runInfo = new Dictionary<string, string>();

            XElement env = doc.Descendants("environment").First();
            runInfo.Add("Test Results File", resultsFile);
            if (env.Attribute("nunit-version") != null)
                runInfo.Add("NUnit Version", env.Attribute("nunit-version").Value);
            runInfo.Add("OS Version", env.Attribute("os-version").Value);
            runInfo.Add("Platform", env.Attribute("platform").Value);
            runInfo.Add("CLR Version", env.Attribute("clr-version").Value);
            runInfo.Add("Machine Name", env.Attribute("machine-name").Value);
            runInfo.Add("User", env.Attribute("user").Value);
            runInfo.Add("User Domain", env.Attribute("user-domain").Value);

            return runInfo;
        }

        public NUnit() { }
    }
}
