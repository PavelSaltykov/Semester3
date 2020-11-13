using System;

namespace Task5
{
    public class TestResult
    {
        public string AssemblyName { get; }

        public string MethodName { get; }

        public bool IsPassed { get; }

        public bool Ignored { get; }

        public string ReasonToIgnore { get; }

        public TimeSpan Time { get; }

        public TestResult(string assemblyName, string methodName, 
            bool isPassed, bool ignored, string reason, TimeSpan time)
        {
            AssemblyName = assemblyName;
            MethodName = methodName;
            IsPassed = isPassed;
            Ignored = ignored;
            ReasonToIgnore = reason;
            Time = time;
        }
    }
}
