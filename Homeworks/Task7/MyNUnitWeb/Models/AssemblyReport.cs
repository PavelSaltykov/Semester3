using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyNUnitWeb.Models
{
    public class AssemblyReport
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public int NumberOfPassed { get; set; }

        public int NumberOfFailed { get; set; }

        public int NumberOfIgnored { get; set; }

        public IEnumerable<TestReport> TestReports { get; set; }
    }
}
