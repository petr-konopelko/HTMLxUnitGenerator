using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using RazorEngine.Text;
using HTMLxUnitGenerator.Logging;
using HTMLxUnitGenerator.Model;
using HTMLxUnitGenerator.Parser;
using System.Web;

namespace HTMLxUnitGenerator
{
    class ReportUnitService
    {
        private Logger _logger = Logger.GetLogger();

        public void CreateReport(PathTypeEnum pathType, string inputPath, string outputPath = null)
        {
            IEnumerable<FileInfo> filePathList;
            if (pathType.Equals(PathTypeEnum.Directory))
            {
                filePathList = new DirectoryInfo(inputPath).GetFiles("*.xml", SearchOption.AllDirectories)
                    .OrderByDescending(f => f.CreationTime);
            }
            else
            {
                String directory = Path.GetDirectoryName(inputPath);
                filePathList = new DirectoryInfo(directory).GetFiles(Path.GetFileName(inputPath));
            }

            InitializeRazor();

            var compositeTemplate = new CompositeTemplate();

            foreach (var filePath in filePathList)
            {
                var testRunner = GetTestRunner(filePath.FullName);

                try
                {
                    IParser parser = ParserFactory.GetParser(testRunner);
                    var report = parser.Parse(filePath.FullName);

                    compositeTemplate.AddReport(report);
                }
                catch (NotSupportedException ex)
                {
                    _logger.Log(Level.Error, ex.Message);
                }

            }

            if (!compositeTemplate.ReportList.Any())
            {
                Logger.GetLogger().Fatal("No reports added - invalid files?");
                return;
            }

            if (pathType == PathTypeEnum.Directory)
            {
                if (String.IsNullOrEmpty(outputPath))
                    outputPath = inputPath;

                compositeTemplate.SideNavLinks = compositeTemplate.SideNavLinks.Insert(0, Templates.SideNav.IndexLink);

                string summary = Engine.Razor.RunCompile(Templates.TemplateManager.GetSummaryTemplate(), "summary", typeof(Model.CompositeTemplate), compositeTemplate, null);

                File.WriteAllText(Path.Combine(outputPath, "Index.html"), summary);

                foreach (var report in compositeTemplate.ReportList)
                {
                    report.SideNavLinks = compositeTemplate.SideNavLinks;
                    var html = Engine.Razor.RunCompile(Templates.TemplateManager.GetFileTemplate(), "report", typeof(Model.Report), report, null);

                    File.WriteAllText(Path.Combine(outputPath, report.FileName + ".html"), html);
                }
            }
            else
            {
                var report = compositeTemplate.ReportList.FirstOrDefault();
                var html = Engine.Razor.RunCompile(Templates.TemplateManager.GetFileTemplate(), "report", typeof(Model.Report), report, null);

                if (String.IsNullOrEmpty(outputPath))
                    outputPath = Path.Combine(Path.GetDirectoryName(inputPath), Path.GetFileNameWithoutExtension(inputPath) + ".html");
                File.WriteAllText(outputPath, html);
            }
        }

        private TestRunner GetTestRunner(string inputFile)
        {
            var testRunner = new ParserFactory(inputFile).GetTestRunnerType();

            _logger.Info("The file " + inputFile + " contains " + Enum.GetName(typeof(TestRunner), testRunner) + " test results");

            return testRunner;
        }

        private void InitializeRazor()
        {
            HttpUtility.HtmlEncode(String.Empty);
            TemplateServiceConfiguration templateConfig = new TemplateServiceConfiguration();
            templateConfig.DisableTempFileLocking = true;
            templateConfig.EncodedStringFactory = new RawStringFactory();
            templateConfig.CachingProvider = new DefaultCachingProvider(x => { });
            var service = RazorEngineService.Create(templateConfig);
            Engine.Razor = service;
        }
    }
}
