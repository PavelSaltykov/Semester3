using NUnit.Framework;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MyNUnit.Tests
{
    [TestFixture]
    public class TestErrorFinderTests
    {
        private Type classWithTests;

        [OneTimeSetUp]
        public void SetUp()
        {
            const string path = @"..\..\..\..\TestProjects\TestProject3";
            var assemblyFiles = Directory.EnumerateFiles(path, "*.dll", SearchOption.AllDirectories);
            var assemblies = new ConcurrentQueue<Assembly>();
            Parallel.ForEach(assemblyFiles, x => assemblies.Enqueue(Assembly.LoadFrom(x)));

            var classes = assemblies.Distinct()
                .SelectMany(a => a.ExportedTypes)
                .Where(t => t.IsClass);

            classWithTests = classes.Where(c => c.GetMethods()
                .Any(mi => mi.GetCustomAttributes().Any(attr => attr is Attributes.TestAttribute)))
                .FirstOrDefault();
        }

        [TestCase("BeforeClassNonStatic", new[] { "Should be static" })]
        [TestCase("TestReturningValue", new[] { "Should be void" })]
        [TestCase("BeforeHavingParameter", new[] { "Should not have parameters" })]
        [TestCase("StaticTestReturningValueAndHavingParameters",
            new[] { "Should not be static", "Should not have parameters", "Should be void" })]
        public void FindErrorsTest(string methodName, string[] messages)
        {
            var info = TestErrorFinder.FindErrorsInClass(classWithTests)
                .Where(m => m.MethodName == methodName)
                .FirstOrDefault();

            CollectionAssert.AreEquivalent(messages, info.Messages);
        }
    }
}
