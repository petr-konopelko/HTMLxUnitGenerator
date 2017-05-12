using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RazorEngine;
using RazorEngine.Templating;
using RazorEngine.Text;

namespace HTMLxUnitGenerator.Model
{
    public class CompositeTemplate
    {
        private List<Report> _reportList = new List<Report>();

        public void AddReport(Report report)
        {
            _reportList.Add(report);

            SideNavLinks += Engine.Razor.RunCompile(Templates.SideNav.Link, "sidenav", typeof(Report), report, null);
        }

        public List<Report> ReportList => _reportList;

        public string SideNavLinks { get; internal set; }
    }
}
