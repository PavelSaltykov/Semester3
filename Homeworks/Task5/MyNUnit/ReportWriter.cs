using MyNUnit.TestInformation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MyNUnit
{
    /// <summary>
    /// Provides output of the report to the output stream.
    /// </summary>
    public class ReportWriter : IDisposable
    {
        private readonly TextWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportWriter"/> class.
        /// </summary>
        /// <param name="writer">Object that represents the standard output stream.</param>
        public ReportWriter(TextWriter writer) => this.writer = writer ??
            throw new ArgumentNullException(nameof(writer));

        /// <summary>
        /// Prints information about tests to the output stream.
        /// </summary>
        /// <param name="testsInfo">Collection of tests information.</param>
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
