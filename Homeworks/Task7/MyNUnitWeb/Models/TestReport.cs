using System;
using System.ComponentModel.DataAnnotations;

namespace MyNUnitWeb.Models
{
    public class TestReport
    {
        [Key]
        public int Id { get; set; }

        public TestStatus Status { get; set; }

        public string ClassName { get; set; }

        public string Name { get; set; }

        public TimeSpan? Time { get; set; }

        public string Message { get; set; }
    }

    public enum TestStatus
    {
        None,
        Passed,
        Failed,
        Ignored
    }
}
