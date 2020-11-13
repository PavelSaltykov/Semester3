using System;

namespace Task5
{
    public class TestResult
    {
        public string AssemblyName { get; }

        public string ClassName { get; }

        public string MethodName { get; }

        public bool IsPassed { get; }

        public bool IsIgnored { get; }

        public string ReasonToIgnore { get; }

        public TimeSpan Time { get; }

        public TestResult(string assemblyName, string className, string methodName,
            bool isPassed, bool isIgnored, string reasonToIgnore, TimeSpan time)
        {
            AssemblyName = assemblyName;
            ClassName = className;
            MethodName = methodName;
            IsPassed = isPassed;
            IsIgnored = isIgnored;
            ReasonToIgnore = reasonToIgnore;
            Time = time;
        }
    }
}
