﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTMLxUnitGenerator.Templates
{
    internal class Summary
    {
        public static string GetSource()
        {
            return @"
@using HTMLxUnitGenerator.Model
@using HTMLxUnitGenerator.Extensions
@inherits RazorEngine.Templating.TemplateBase<CompositeTemplate>

<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""utf-8"">
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
            <a id=""menu"" data-activates=""slide-out"" class=""waves-effect waves-light"" href=""javascript:void(0)"">
                <i class=""material-icons"" title=""Menu"">menu</i>
            </a>
            <span class=""test-suite-name executive-summary"">Executive Summary</span>
        </div>
    </section>
    <ul id=""slide-out"" class=""side-nav"">
        <p>Test Reports</p>
        @Model.SideNavLinks
    </ul>
    <div class=""main-content"">
        <table class=""table bordered"" id=""executive-summary-table"">
            <thead>
                <tr>
                    <th>File</th>
                    <th>Status</th>
                    <th>Total Tests</th>
                    <th>Passed</th>
                    <th>Failed</th>
                    <th>Others</th>
                    <th>Total Duration</th>
                    <th>Quick Summary</th>
                </tr>
            </thead>
            <tbody>
                @foreach (var file in Model.ReportList)
                {
                    <tr>
                        <td><a href=""./@(file.FileName).html"">@file.FileName</a></td>
                        <td class=""@file.Status.ToString().ToLower()"">@file.Status.ToString()</td>
                        <td>@file.Total</td>
                        <td>@file.Passed</td>
                        <td>@file.Failed</td>
                        <td>@(file.Total - (file.Passed + file.Failed))</td>
                        <td>@file.Duration.ToReportString()</td>
                        <td>
                            <div class=""progress-bar-container"">
                                @if (file.PassedPercentage > 0)
                                {
                                    <span class=""progress-bar progress-bar-success"" style=""width:@file.PassedPercentage.ToStringEn()%""><span class=""percent-value"">@file.Passed</span></span>
                                }
                                @if (file.FailedPercentage > 0)
                                {
                                    <span class=""progress-bar progress-bar-failed"" style=""width:@file.FailedPercentage.ToStringEn()%""><span class=""percent-value"">@file.Failed</span></span>
                                }
                                @if (file.SkippedPercentage > 0)
                                {
                                    <span class=""progress-bar progress-bar-skipped"" style=""width:@file.SkippedPercentage.ToStringEn()%""><span class=""percent-value"">@file.Skipped</span></span>
                                }
                                @if (file.InconclusivePercentage > 0)
                                {
                                    <span class=""progress-bar progress-bar-inconclusive"" style=""width:@file.InconclusivePercentage.ToStringEn()%""><span class=""percent-value"">@file.Inconclusive</span></span>
                                }
                            </div>
                        </td>
                    </tr>
                }
                <tr class=""bold"">
                    <td><span class=""weight-normal"">Totals</span></td>
                    <td>-</td>
                    <td id=""total-tests"">@Model.ReportList.Sum(x => x.Total)</td>
                    <td id=""total-passed"">@Model.ReportList.Sum(x => x.Passed)</td>
                    <td id=""total-failed"">@Model.ReportList.Sum(x => x.Failed)</td>
                    <td id=""total-others"">@Model.ReportList.Sum(x => x.Total - (x.Failed + x.Passed))</td>
                    <td id=""total-duration"">@Model.ReportList.Sum(x => x.Duration).ToReportString()</td>
                    <td>-</td>
                </tr>
            </tbody>
        </table>
    </div>

    <script src=""https://code.jquery.com/jquery-3.1.1.min.js""
            integrity=""sha256-hVVnYaiADRTO2PzUGmuLJr8BLUSjGIZsDYGmIJLv2b8=""
            crossorigin=""anonymous""></script>
    <!-- Include all compiled plugins (below), or include individual files as needed -->
    <!-- Compiled and minified JavaScript -->
    <script src=""https://cdnjs.cloudflare.com/ajax/libs/materialize/0.97.8/js/materialize.min.js""></script>
    <script src=""https://rawgit.com/petr-konopelko/HTMLxUnitGenerator/master/HTMLxUnitGenerator/Resources/Javascript.js"" type=""text/javascript""></script>
</body>
</html>";
        }
    }
}
