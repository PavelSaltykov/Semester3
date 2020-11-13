using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Task5
{
    public class ReportWriter : IDisposable
    {
        private readonly TextWriter writer;

        public ReportWriter(TextWriter writer) => this.writer = writer;

        public void Write(IEnumerable<TestResult> testResults)
        {
            testResults.OrderBy(tr => tr.AssemblyName)
                       .ThenBy(tr => tr.ClassName)
                       .ThenBy(tr => tr.MethodName);

            var groupsByAssembly = testResults.GroupBy(tr => tr.AssemblyName);

            foreach (var group in groupsByAssembly)
            {
                writer.WriteLine(group.Key);
                WriteGroupByAssembly(group);
                writer.WriteLine();
            }
        }

        private void WriteGroupByAssembly(IGrouping<string, TestResult> groupByAssembly)
        {
            var groupsByClass = groupByAssembly.GroupBy(tr => tr.ClassName);

            foreach (var groupByClass in groupsByClass)
            {
                writer.WriteLine($"\t{groupByClass.Key}");
                foreach (var testResult in groupByClass)
                {
                    writer.Write($"\t\t{testResult}");
                }
            }
        }

        public void Dispose() => writer.Dispose();
    }
}
