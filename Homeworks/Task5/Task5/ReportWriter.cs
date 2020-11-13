using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Task5.TestInformation;

namespace Task5
{
    public class ReportWriter : IDisposable
    {
        private readonly TextWriter writer;

        public ReportWriter(TextWriter writer) => this.writer = writer;

        public void Write(IEnumerable<TestInfo> testsInfo)
        {
            testsInfo = testsInfo.OrderBy(info => info.AssemblyName)
                                 .ThenBy(info => info.ClassName)
                                 .ThenBy(info => info.MethodName);

            var groupsByAssembly = testsInfo.GroupBy(info => info.AssemblyName);

            foreach (var group in groupsByAssembly)
            {
                writer.WriteLine($"Assembly: {group.Key}");
                WriteGroupByAssembly(group);
                writer.WriteLine();
            }
        }

        private void WriteGroupByAssembly(IGrouping<string, TestInfo> groupByAssembly)
        {
            var groupsByClass = groupByAssembly.GroupBy(info => info.ClassName);

            foreach (var groupByClass in groupsByClass)
            {
                writer.WriteLine($"\tClass: {groupByClass.Key}");
                foreach (var info in groupByClass)
                {
                    writer.WriteLine($"\t\t{info}");
                }
            }
        }

        public void Dispose() => writer.Dispose();
    }
}
