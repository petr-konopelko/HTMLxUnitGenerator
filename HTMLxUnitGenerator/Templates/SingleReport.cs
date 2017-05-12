using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportUnit.Templates
{
    class SingleReport
    {
        public static string GetSource()
        {
            return @"
@using HTMLxUnitGenerator.Model;
@using System.Web;
@using HTMLxUnitGenerator.Extensions;

@inherits RazorEngine.Templating.TemplateBase<Report>

<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""utf-8"">
    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
    <title>ReportUnit</title>
    <!-- Materialize -->
    <link rel=""stylesheet"" href=""https://cdnjs.cloudflare.com/ajax/libs/materialize/0.97.8/css/materialize.min.css"">
    <link rel=""stylesheet"" href=""https://rawgit.com/petr-konopelko/HTMLxUnitGenerator/master/HTMLxUnitGenerator/Resources/ReportUnitStyle.css"">
    <!-- Fonts -->
    <link href=""https://fonts.googleapis.com/icon?family=Material+Icons"" rel=""stylesheet"">
</head>
<body>
    <section class=""header light-blue z-depth-1"">
        <div class=""headerIcon"">
            @if (!String.IsNullOrEmpty(Model.SideNavLinks))
            {
                <a id=""menu"" data-activates=""slide-out"" class=""waves-effect waves-light left"" href=""javascript:void(0)"">
                    <i class=""material-icons"" title=""Menu"">menu</i>
                </a>
            }
            <span class=""test-suite-name"">@Model.FileName</span>
        </div>
        <a id=""runInfo"" data-position=""bottom""
           class=""headerIcon waves-effect waves-light right tooltipped""
           href=""#runInfoModal"" data-tooltip=""Run info"">
            <i class=""material-icons"">info_outline</i>
        </a>
    </section>
    <ul id=""slide-out"" class=""side-nav"">
        <p>Test Reports</p>
        @Model.SideNavLinks
    </ul>
    <div class=""main-content"">
        @if (Model.Total == 0)
        {
            <div class='row no-tests'>
                <div clas='col s12 m6 l4'>
                    <div class='no-tests-message card-panel no-margin-v'>
                        <p>No tests were found in @Model.FileName.</p>
                        @if (!String.IsNullOrEmpty(@Model.StatusMessage))
                        {
                            <pre>@Model.StatusMessage</pre>
                        }
                    </div>
                </div>
            </div>
        }
        else
        {
            <ul id=""diagrams"" class=""row collapsible"" data-collapsible=""expandable"">
                <li class=""col s4"">
                    <div class=""collapsible-header total-suite-info active truncate"">
                        <span class=""bold"">Suite Summary:</span>
                        <span class=""suites-passed""></span> suites(s) passed,
                        <span class=""suites-failed""></span> suites(s) failed,
                        <span class=""suites-others""></span> others
                    </div>
                    <div class=""collapsible-body"">
                        <div class=""diagram"">
                            <canvas id=""suiteDiagram""></canvas>
                        </div>
                    </div>
                </li>
                <li class=""col s4"">
                    <div class=""collapsible-header total-suite-info active truncate"">
                        <span class=""bold"">Summary:</span> @Model.Passed test(s) passed, @Model.Failed test(s) failed, @(Model.Total - (Model.Passed + Model.Failed)) others
                    </div>
                    <div class=""collapsible-body"">
                        <div class=""diagram"">
                            <canvas id=""testDiagram""></canvas>
                        </div>
                    </div>
                </li>
                <li class=""col s4"">
                    <div class=""collapsible-header total-suite-info active truncate"">
                        <span class=""bold"">Percentage:</span>
                        <span class=""pass-percentage"">@Math.Round(Model.PassedPercentage, 2) %</span>
                    </div>
                    <div class=""collapsible-body"">
                        <div class=""diagram"">

                            <div class=""percentage-amount"">
                               @Math.Round(Model.PassedPercentage, 2) %
                            </div>
                            <div id=""percentageDiagram"" class=""progress-bar-container progress-bar-graph"">
                                @if (Model.PassedPercentage > 0)
                                {
                                    <span class=""progress-bar progress-bar-success"" style=""width:@Model.PassedPercentage.ToStringEn()%""></span>
                                }
                                @if (Model.FailedPercentage > 0)
                                {
                                    <span class=""progress-bar progress-bar-failed"" style=""width:@Model.FailedPercentage.ToStringEn()%""></span>
                                }
                                @if (Model.SkippedPercentage > 0)
                                {
                                    <span class=""progress-bar progress-bar-skipped"" style=""width:@Model.SkippedPercentage.ToStringEn()%""></span>
                                }
                                @if (Model.InconclusivePercentage > 0)
                                {
                                    <span class=""progress-bar progress-bar-inconclusive"" style=""width:@Model.InconclusivePercentage.ToStringEn()%""></span>
                                }
                            </div>
                        </div>
                    </div>
                </li>
                <li class='right-align'>
                    <a id=""hideDiagrams"" href=""javascript:void(0)"" data-position=""left""
                       class=""tooltipped"" data-delay=""500"" data-tooltip=""Show/Hide diagrams"">
                        <i class=""material-icons"">keyboard_arrow_down</i>
                    </a>
                </li>
            </ul>
            <div id='content' class=""row"">
                <div class=""col s4 test-suites-column"">
                    <div class=""z-depth-2 test-suites"">
                        <div class=""filter-section"">
                            <a class=""dropdown-button"" href=""javascript:void(0)"" data-activates=""suiteFilter"" title=""Suite filter""><i class=""material-icons"">filter_list</i></a>
                            <ul id=""suiteFilter"" class='dropdown-content'>
                                @foreach (var status in Model.TestSuiteList.Select(x => x.Status).Distinct())
                                {
                                    var statusString = status.ToString().ToLower();
                                    <li><a class=""suite-filter"" suite-status=""@statusString"" href=""javascript:void(0)"">@statusString</a></li>
                                }
                            </ul>
                            <a id=""resetSuiteFilter"" class=""disabled clear-filter"" href=""javascript:void(0)"" title=""Clear filter""><i class=""material-icons"">settings_backup_restore</i></a>
                        </div>
                        <div class=""row"">
                            <div class=""col s9"">Test suite name</div>
                            <div class=""col s3 center-align"">Status</div>
                        </div>
                        @foreach (var testSuite in Model.TestSuiteList)
                        {
                            var testSuiteStatus = testSuite.Status.ToString().ToLower();
                            <div class=""row suite-info waves-effect suite-@testSuiteStatus"">
                                <div class=""col s9 suite-name truncate"" title=""@HttpUtility.HtmlEncode(testSuite.Name)"">
                                    <a href=""javascript:void(0)"" class=""suite-details-button"">@HttpUtility.HtmlEncode(testSuite.Name)</a>
                                </div>
                                <div class=""col s3 center-align"">
                                    <div class=""@testSuiteStatus @testSuiteStatus-border"">@testSuiteStatus</div>
                                </div>
                                <div class=""suite-details hide"">
                                    <span class=""suite-start-time"">@testSuite.StartTimeForReport</span>
                                    <span class=""suite-end-time"">@testSuite.EndTimeForReport</span>
                                    <span class=""total-duration"">@testSuite.Duration.ToReportString()</span>
                                    <span class=""test-suite-link"">@testSuite.TestCasesLink</span>
                                </div>
                                <div class=""test-section hide"">
                                    @foreach (var test in testSuite.TestList)
                                    {
                                        var testStatus = test.Status.ToString().ToLower();
                                        <div class=""row test test-@testStatus"">
                                            <div class=""col s9 test-name-block truncate"" title=""@HttpUtility.HtmlEncode(test.Name)"">
                                                <a href=""javascript:void(0)"" class=""test-name"">@HttpUtility.HtmlEncode(test.Name)</a>
                                                <div class=""test-info hide"">
                                                    <span class=""start-time"">@test.StartTimeForReport</span>
                                                    <span class=""end-time"">@test.EndTimeForReport</span>
                                                    <span class=""duration"">@test.Duration.ToReportString()</span>
                                                    <span class=""description"">@HttpUtility.HtmlEncode(test.Description)</span>
                                                </div>
                                            </div>
                                            <div class=""col s2 center-align"">
                                                <div class=""test-status @testStatus @testStatus-border"">@testStatus</div>
                                            </div>

                                            <div class=""col s1 center-align"">
                                                @if (!String.IsNullOrEmpty(test.StatusMessage))
                                                {
                                                    String classWarning = test.Status == Status.Error || test.Status == Status.Failed ? ""failed"" : ""warning"";
                                                    <a href=""javascript:void(0)"" class=""test-error""><i class=""material-icons @classWarning"">warning</i></a>
                                                    <div class=""test-error-content hide"">
                                                        <pre class=""test-error-message"">@HttpUtility.HtmlEncode(test.StatusMessage)</pre>
                                                        @if (!String.IsNullOrEmpty(test.ImageExceptionLink))
                                                        {
                                                            <span class=""image-link"">@test.ImageExceptionLink</span>
                                                        }
                                                    </div>
                                                }
                                            </div>
                                        </div>
                                    }
                                </div>
                            </div>
                        }
                    </div>
                </div>
                <div class=""col s8 z-depth-2 tests-column"">
                    <div class=""filter-section"">
                        <a class=""dropdown-button"" href=""javascript:void(0)"" data-activates=""testFilter"" title=""Test filter""><i class=""material-icons"">filter_list</i></a>
                        <ul id=""testFilter"" class='dropdown-content'></ul>
                        <a id=""resetTestFilter"" class=""disabled clear-filter"" href=""javascript:void(0)"" title=""Clear filter""><i class=""material-icons"">settings_backup_restore</i></a>
                    </div>
                    <div class=""row test-header"">
                        <div class=""col s9 bold"">Test Name</div>
                        <div class=""col s2 center-align bold"">Status</div>
                        <div class=""col s1 center-align bold"">Message</div>
                    </div>
                    <div id=""tests""></div>
                </div>
            </div>
                            }
    </div>
    <div id='runInfoModal' class='modal'>
        <div class='modal-content'>
            <h5><!--%FILENAME%--> Run info</h5>
            <table class=""bordered"">
                <thead>
                    <tr>
                        <th style=""min-width:160px"">Param</th>
                        <th>Value</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td class=""bold"">TestRunner</td>
                        <td>@Model.RunInfo.TestRunner</td>
                    </tr>
                    <tr>
                        <td class=""bold"">Start time</td>
                        <td>@Model.StartTimeForReport</td>
                    </tr>
                    <tr>
                        <td class=""bold"">End time</td>
                        <td>@Model.EndTimeForReport</td>
                    </tr>
                    <tr>
                        <td class=""bold"">Duration</td>
                        <td>@Model.Duration.ToReportString()</td>
                    </tr>
                    @if (Model.RunInfo.Info != null && Model.RunInfo.Info.Any())
                    {
                        foreach (var testInfo in Model.RunInfo.Info)
                        {
                            <tr>
                                <td class=""bold"">@testInfo.Key</td>
                                <td>@testInfo.Value</td>
                            </tr>
                        }
                    }

                </tbody>
            </table>
        </div>
    </div>
    <div id='suiteInfoDynamicModal' class='modal'>
        <div class='modal-content'>
            <h5 id=""suiteName""></h5>
            <table class=""bordered"">
                <tbody>
                    <tr>
                        <td class=""bold"">Start time</td>
                        <td id=""suiteStartTime""></td>
                    </tr>
                    <tr>
                        <td class=""bold"">End time</td>
                        <td id=""suiteEndTime""></td>
                    </tr>
                    <tr>
                        <td class=""bold"">Total duration</td>
                        <td id=""totalDuration""></td>
                    </tr>
                    <tr id=""testSuiteLink"">
                        <td class=""bold"">Test suite link</td>
                        <td>
                            <a target=""_blank"" href="""">Link</a>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
    <div id='testinfoDynamicModal' class='modal'>
        <div class='modal-content'>
            <h5 class=""test-name""></h5>
            <table class=""bordered"">
                <tbody>
                    <tr>
                        <td class=""bold"">Start time</td>
                        <td class=""start-time""></td>
                    </tr>
                    <tr>
                        <td class=""bold"">End time</td>
                        <td class=""end-time""></td>
                    </tr>
                    <tr>
                        <td class=""bold"">Duration</td>
                        <td class=""duration""></td>
                    </tr>
                </tbody>
            </table>
            <div class=""description hide"">
                <h6 class=""bold"">Description :</h6>
                <pre id=""testDescription""></pre>
            </div>
        </div>
    </div>
    <div id='testErrorDynamicModal' class='modal'>
        <div class='modal-content'>
            <h5 class=""test-name""></h5>
            <h6 class=""bold"">Error message:</h6>
            <pre id=""errorMessage""></pre>
            <h6 id=""linkOnImage"" style=""display:inline-block"">
                <span class=""bold"">Link on image:</span>
                <a target=""_blank"" href="""" style=""margin-left:10px"">Link</a>
            </h6>
        </div>
    </div>
    <div id=""suitesInfo"" class=""hide"">
        <span class=""total-suite-amount"">@Model.TestSuiteList.Count</span>
        @foreach (var status in Model.TestSuiteList.Select(x => x.Status).Distinct())
        {
            string statusString = status.ToString().ToLower();
            <span class=""suites-@statusString"">@Model.TestSuiteList.Where(x => x.Status == status).Count()</span>
        }
    </div>
    <div id=""testsInfo"" class=""hide"">
        <span class=""total-tests-amount"">@Model.Total</span>
        <span class=""tests-passed"">@Model.Passed</span>
        <span class=""tests-failed"">@Model.Failed</span>
        <span class=""tests-skipped"">@Model.Skipped</span>
        <span class=""tests-inconclusive"">@Model.Inconclusive</span>
    </div>
    <script src=""https://code.jquery.com/jquery-3.1.1.min.js""
            integrity=""sha256-hVVnYaiADRTO2PzUGmuLJr8BLUSjGIZsDYGmIJLv2b8=""
            crossorigin=""anonymous""></script>
    <!-- Include all compiled plugins (below), or include individual files as needed -->
    <!-- Compiled and minified JavaScript -->
    <script src=""https://cdnjs.cloudflare.com/ajax/libs/materialize/0.97.8/js/materialize.min.js""></script>
    <script type=""text/javascript"" src=""https://cdnjs.cloudflare.com/ajax/libs/Chart.js/2.4.0/Chart.min.js""></script>
    <script src=""https://rawgit.com/petr-konopelko/HTMLxUnitGenerator/master/HTMLxUnitGenerator/Resources/Javascript.js"" type=""text/javascript""></script>
</body>
</html>";
        }
    }
}
