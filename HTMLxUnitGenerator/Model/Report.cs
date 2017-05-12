using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace HTMLxUnitGenerator.Model
{
    public class Report
    {
        public List<Status> StatusList;

        public List<string> CategoryList;

        public List<TestSuite> TestSuiteList { get; set; }

        public DateTime StartTime { get; set; }

        public string StartTimeForReport => StartTime.ToString();

        public DateTime EndTime { get; set; }

        public string EndTimeForReport => EndTime.ToString();

        /// <summary>
        /// Error or other status messages
        /// </summary>
        public string StatusMessage { get; set; }

        /// <summary>
        /// File name generated that this data is for
        /// </summary>
        public string FileName { get; set; }

        public RunInfo RunInfo { get; private set; } = new RunInfo();



        /// <summary>
        /// Overall result of the test run (eg Passed, Failed)
        /// </summary>
        public Status Status
        {
            get
            {
                if (StatusList.Any(x => x == Status.Error || x == Status.Failed))
                    return Status.Failed;
                else if (StatusList.Any(x => x == Status.Skipped))
                    return Status.Skipped;
                else if (StatusList.Any(x => x == Status.Inconclusive))
                    return Status.Inconclusive;
                else
                    return Status.Passed;
            }
        }


        /// <summary>
        /// How long the test suite took to run (in milliseconds)
        /// </summary>
        public Double Duration { get; set; }

        /// <summary>
        /// Total number of tests run
        /// </summary>
        public int Total { get; set; }

        public int Passed { get; set; }

        public int Failed { get; set; }

        public int Inconclusive { get; set; }

        public int Skipped { get; set; }

        public int Errors { get; set; }

        public double PassedPercentage => (double)Passed / Total * 100;

        public double FailedPercentage => Failed / (double)Total * 100;

        public double InconclusivePercentage => (double)Inconclusive / (double)Total * 100;

        public double SkippedPercentage => (double)Skipped / (double)Total * 100;

        public double ErrorsPercentage => Errors / Total * 100;

        public string SideNavLinks { get; set; }

        public Report()
        {
            TestSuiteList = new List<TestSuite>();
            CategoryList = new List<string>();
            StatusList = new List<Status>();
        }
    }
}
