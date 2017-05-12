using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace HTMLxUnitGenerator.Model
{
    public class Test
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string ImageExceptionLink { get; set; }

        public string TestCasesLink { get; set; }

        /// <summary>
        /// Status of the test run (eg Passed, Failed)
        /// </summary>
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
        /// How long the test took to run (in milliseconds)
        /// </summary>
        public double Duration { get; set; }

        /// <summary>
        /// Categories & features associated with the test
        /// </summary>
        public List<string> CategoryList;

        public string GetCategories()
        {
            if (CategoryList.Count == 0)
            {
                return "";
            }

            return string.Join(" ", CategoryList);
        }

        public Test()
        {
            CategoryList = new List<string>();
            Status = Status.Unknown;
        }
    }
}
