using System;
using System.Collections.Generic;

using HTMLxUnitGenerator.Parser;

namespace HTMLxUnitGenerator.Model
{
    /// <summary>
    /// Detailed information on the environment and machine that the tests were run under
    /// </summary>
    public class RunInfo
    {
        public RunInfo()
        {
            Info = new Dictionary<string, string>();
        }

        /// <summary>
        /// Execution info such as username, machine-name, domain etc.
        /// </summary>
        public Dictionary<string, string> Info { get; private set; }

        public void AddInfo(Dictionary<string, string> info)
        {
            Info = info;
        }

        /// <summary>
        /// The type of test runner that generated the data (eg NUnit, mstest)
        /// </summary>
        public TestRunner TestRunner { get; set; }

        /// <summary>
        /// Name of the assembly that the tests are for
        /// </summary>
        public string AssemblyName { get; set; }
    }
}
