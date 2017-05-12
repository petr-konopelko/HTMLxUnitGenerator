using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTMLxUnitGenerator.Model
{
    public class TestSuite
    {
        public TestSuite()
        {
            TestList = new List<Test>();
            this.Status = Status.Unknown;
        }

        public string Name { get; set; }
        
        public string Description { get; set; }

        public string TestCasesLink { get; set; }

        public Status Status { get; set; }

        /// <summary>
        /// Error or other status messages
        /// </summary>
        public string StatusMessage { get; set; }

        public DateTime StartTime { get; set; }

        public string StartTimeForReport => StartTime.ToString();

        public DateTime EndTime { get; set; }

        public string EndTimeForReport => EndTime.ToString();

        /// <summary>
        /// How long the test fixture took to run (in milliseconds)
        /// </summary>
        public double Duration { get; set; }

        public List<Test> TestList { get; private set; }
    }
}
