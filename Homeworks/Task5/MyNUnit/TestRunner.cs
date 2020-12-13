using Attributes;
using MyNUnit.MethodInformation;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MyNUnit
{
    /// <summary>
    /// Allows to run tests from assemblies.
    /// </summary>
    public class TestRunner
    {
        private readonly ConcurrentQueue<Info> testsInfo = new ConcurrentQueue<Info>();

        private readonly ConcurrentQueue<Type> classTypes;

        private readonly ConcurrentQueue<IncorrectMethodInfo> errorsInfo = new ConcurrentQueue<IncorrectMethodInfo>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TestRunner"/> class.
        /// </summary>
        /// <param name="path">Path to assemblies.</param>
        public TestRunner(string path)
        {
            var assemblyFiles = Directory.EnumerateFiles(path, "*.dll", SearchOption.AllDirectories);
            if (assemblyFiles.Count() == 0)
                throw new AssembliesNotFoundException($"Assemblies not found in path: {path}.");

            var assemblies = new ConcurrentQueue<Assembly>();
            Parallel.ForEach(assemblyFiles, x => assemblies.Enqueue(Assembly.LoadFrom(x)));

            var classes = assemblies.Distinct()
                .SelectMany(a => a.ExportedTypes)
                .Where(t => t.IsClass);

            var classesWithoutErrors = new ConcurrentQueue<Type>();
            Parallel.ForEach(classes, testClass =>
            {
                var errors = TestErrorFinder.FindErrorsInClass(testClass);
                foreach (var info in errors)
                {
                    errorsInfo.Enqueue(info);
                }
                var containsTests = testClass.GetMethods().Any(mi => mi.GetCustomAttributes()
                    .Any(attr => attr is TestAttribute));
                if (errors.Count() == 0 && containsTests)
                {
                    classesWithoutErrors.Enqueue(testClass);
                }
            });
            classTypes = new ConcurrentQueue<Type>(classesWithoutErrors);
        }

        /// <summary>
        /// Runs all tests from assemblies.
        /// </summary>
        public void Run() => Parallel.ForEach(classTypes, ExecuteTestsInClass);

        private void ExecuteTestsInClass(Type classType)
        {
            ExecuteStaticMethods(classType, typeof(BeforeClassAttribute));
            ExecuteTestMethods(classType);
            ExecuteStaticMethods(classType, typeof(AfterClassAttribute));
        }

        private void ExecuteStaticMethods(Type classType, Type attributeType)
        {
            var methods = GetMethodsWithAttribute(classType, attributeType);
            foreach (var method in methods)
            {
                method.Invoke(null, null);
            }
        }

        private void ExecuteTestMethods(Type classType)
        {
            var beforeMethods = GetMethodsWithAttribute(classType, typeof(BeforeAttribute));
            var afterMethods = GetMethodsWithAttribute(classType, typeof(AfterAttribute));

            var testMethods = GetMethodsWithAttribute(classType, typeof(TestAttribute));
            Parallel.ForEach(testMethods, test =>
            {
                var instance = Activator.CreateInstance(classType);
                foreach (var method in beforeMethods)
                {
                    method.Invoke(instance, null);
                }
                ExecuteTest(test, instance);
                foreach (var method in afterMethods)
                {
                    method.Invoke(instance, null);
                }
            });
        }

        private void ExecuteTest(MethodInfo method, object instance)
        {
            var attribute = (TestAttribute)method.GetCustomAttribute(typeof(TestAttribute));
            if (attribute.Ignore != null)
            {
                var testInfo = new IgnoredTestInfo(method, attribute.Ignore);
                testsInfo.Enqueue(testInfo);
                return;
            }

            var isPassed = false;
            Exception unexpected = null;
            var stopwatch = new Stopwatch();
            try
            {
                stopwatch.Start();
                method.Invoke(instance, null);
                isPassed = attribute.Expected == null;
            }
            catch (TargetInvocationException e)
            {
                isPassed = e.InnerException.GetType() == attribute.Expected;
                unexpected = e.InnerException.GetType() == attribute.Expected ? null : e.InnerException;
            }
            finally
            {
                stopwatch.Stop();
                var time = stopwatch.Elapsed;
                var testInfo = new TestResultInfo(method, isPassed, attribute.Expected, unexpected, time);
                testsInfo.Enqueue(testInfo);
            }
        }

        private IEnumerable<MethodInfo> GetMethodsWithAttribute(Type classType, Type attributeType)
        {
            return classType.GetMethods()
                .Where(mi => mi.GetCustomAttributes().Any(attr => attr.GetType() == attributeType));
        }

        /// <summary>
        /// Returns the information about tests.
        /// </summary>
        public IEnumerable<Info> GetTestsInfo() => testsInfo;

        public IEnumerable<IncorrectMethodInfo> GetErrorsInfo() => errorsInfo;
    }
}
