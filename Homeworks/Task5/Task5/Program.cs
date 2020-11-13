using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Task5
{
    class Program
    {
        static void Main(string[] args)
        {
            var sw = new Stopwatch();
            sw.Start();
            Exception exception = null;
            try
            {
                var array = new int[2];
                var item = array[2];
            }
            catch (IndexOutOfRangeException e)
            {
                exception = e;
            }
            var interval = sw.Elapsed;

            var info1 = new TestInformation.IgnoredTestInfo("A", "C", "Test2", "ignore");
            var info2 = new TestInformation.TestResultInfo("A", "C", "Test1", false,
                exception, interval);

            using var rw = new ReportWriter(Console.Out);
            rw.Write(new List<TestInformation.TestInfo>() { info1, info2 });
        }
    }
}
