using Attributes;
using MyNUnit.MethodInformation;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MyNUnit
{
    /// <summary>
    /// Diagnoses errors in the test classes.
    /// </summary>
    public static class TestErrorFinder
    {
        private static IEnumerable<MethodInfo> GetMethodsWithAttribute(Type classType, Type attributeType)
        {
            return classType.GetMethods()
                .Where(mi => mi.GetCustomAttributes().Any(attr => attr.GetType() == attributeType));
        }

        /// <summary>
        /// Finds errors in the specified test class.
        /// </summary>
        /// <param name="classType">Test class type.</param>
        /// <returns>Info about errors.</returns>
        public static IEnumerable<IncorrectMethodInfo> FindErrorsInClass(Type classType)
        {
            var beforeClassMethods = GetMethodsWithAttribute(classType, typeof(BeforeClassAttribute));
            var afterClassMethods = GetMethodsWithAttribute(classType, typeof(AfterClassAttribute));
            var staticMethods = beforeClassMethods.Concat(afterClassMethods);

            var beforeMethods = GetMethodsWithAttribute(classType, typeof(BeforeAttribute));
            var testMethods = GetMethodsWithAttribute(classType, typeof(TestAttribute));
            var afterMethods = GetMethodsWithAttribute(classType, typeof(AfterAttribute));
            var nonStaticMethods = beforeMethods.Concat(testMethods).Concat(afterMethods);

            var methods = staticMethods.Concat(nonStaticMethods).ToArray();
            var errors = new ConcurrentQueue<IncorrectMethodInfo>();
            Parallel.For(0, methods.Count(), i =>
            {
                var messages = ErrorsInMethod(methods[i], i < staticMethods.Count());
                if (messages.Length > 0)
                {
                    errors.Enqueue(new IncorrectMethodInfo(methods[i], messages));
                }
            });
            return errors;
        }

        private static string[] ErrorsInMethod(MethodInfo method, bool shouldBeStatic)
        {
            var messages = new List<string>();
            if (shouldBeStatic && !method.IsStatic)
            {
                messages.Add("Should be static");
            }
            if (!shouldBeStatic && method.IsStatic)
            {
                messages.Add("Should not be static");
            }
            if (method.ReturnType != typeof(void))
            {
                messages.Add("Should be void");
            }
            if (method.GetParameters().Length != 0)
            {
                messages.Add("Should not have parameters");
            }
            return messages.ToArray();
        }
    }
}
