using System.Collections.Generic;

namespace MyNUnitWeb.Models
{
    public class AssemblyReport
    {
        public string Name { get; set; }

        public int NumberOfPassed { get; set; }

        public int NumberOfFailed { get; set; }

        public int NumberOfIgnored { get; set; }

        public IEnumerable<TestReport> TestReports { get; set; }
    }
}
