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

        public ReportWriter(TextWriter writer) => this.writer = writer ??
            throw new ArgumentNullException(nameof(writer));

        public void Write(IEnumerable<TestInfo> testsInfo)
        {
            if (testsInfo == null)
                throw new ArgumentNullException(nameof(testsInfo));

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
                    var symbol = Symbol(info);
                    writer.WriteLine($"\t\t{symbol} {info}");
                }
            }
        }

        private char Symbol(TestInfo info)
        {
            if (info is TestResultInfo)
            {
                return (info as TestResultInfo).IsPassed ? '+' : '-';
            }
            return '?';
        }

        public void Dispose() => writer.Dispose();
    }
}
