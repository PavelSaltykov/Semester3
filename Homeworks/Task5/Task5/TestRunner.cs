using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Task5.Attributes;
using Task5.TestInformation;

namespace Task5
{
    public class TestRunner
    {
        private readonly ConcurrentQueue<TestInfo> testsInfo = new ConcurrentQueue<TestInfo>();

        private readonly ConcurrentQueue<Type> classTypes;

        public TestRunner(string path)
        {
            var assemblyFiles = Directory.EnumerateFiles(path, "*.dll", SearchOption.AllDirectories);
            if (assemblyFiles.Count() == 0)
                throw new AssembliesNotFoundException($"Assemblies not found in path: {path}.");

            var classes = assemblyFiles.Select(Assembly.LoadFrom).ToHashSet()
                .SelectMany(a => a.ExportedTypes)
                .Where(t => t.IsClass);

            var classesContaininigTests = classes.Where(c => c.GetMethods()
                .Any(mi => mi.GetCustomAttributes().Any(attr => attr is TestAttribute)));

            classTypes = new ConcurrentQueue<Type>(classesContaininigTests);
        }

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
                if (!method.IsStatic)
                    throw new InvalidOperationException($"{method} must be static.");

                method.Invoke(null, null);
            }
        }

        private void ExecuteTestMethods(Type classType)
        {
            var instance = Activator.CreateInstance(classType);

            var beforeMethods = GetMethodsWithAttribute(classType, typeof(BeforeAttribute));
            var afterMethods = GetMethodsWithAttribute(classType, typeof(AfterAttribute));

            var testMethods = GetMethodsWithAttribute(classType, typeof(TestAttribute));
            foreach (var test in testMethods)
            {
                foreach (var method in beforeMethods)
                {
                    method.Invoke(instance, null);
                }
                ExecuteTest(test, instance);
                foreach (var method in afterMethods)
                {
                    method.Invoke(instance, null);
                }
            }
        }

        private void ExecuteTest(MethodInfo method, object instance)
        {
            var assemblyName = method.DeclaringType.Assembly.GetName().Name;
            var className = method.DeclaringType.Name;
            var methodName = method.Name;

            var attribute = (TestAttribute)method.GetCustomAttribute(typeof(TestAttribute));
            if (attribute.Ignore != null)
            {
                var testInfo = new IgnoredTestInfo(assemblyName, className, methodName, attribute.Ignore);
                testsInfo.Enqueue(testInfo);
                return;
            }

            var isPassed = false;
            Exception unexpected = null;
            var stopwatch = Stopwatch.StartNew();
            try
            {
                stopwatch.Start();
                method.Invoke(instance, null);
                isPassed = attribute.Expected == null;
            }
            catch (TargetInvocationException e) when (e.InnerException.GetType() != attribute.Expected)
            {
                unexpected = e.InnerException;
            }
            finally
            {
                stopwatch.Stop();
                var testInfo = new TestResultInfo(assemblyName, className, methodName,
                    isPassed, unexpected, stopwatch.Elapsed);
                testsInfo.Enqueue(testInfo);
            }
        }

        private IEnumerable<MethodInfo> GetMethodsWithAttribute(Type classType, Type attributeType)
        {
            return classType.GetMethods()
                .Where(mi => mi.GetCustomAttributes().Any(attr => attr.GetType() == attributeType));
        }

        public IEnumerable<TestInfo> GetTestsInfo() => testsInfo;
    }
}
