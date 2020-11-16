using System;

namespace Task5.TestInformation
{
    public class TestResultInfo : TestInfo
    {
        public bool IsPassed { get; }

        public Exception UnexpectedException { get; }

        public TimeSpan Time { get; }

        public TestResultInfo(string assemblyName, string className, string methodName,
            bool isPassed, Exception unexpectedException, TimeSpan time)
            : base(assemblyName, className, methodName)
        {
            IsPassed = isPassed;
            UnexpectedException = unexpectedException;
            Time = time;
        }

        public override string ToString()
        {
            var info = base.ToString() + " ";

            var failMessage = $"FAILED: {UnexpectedException?.GetType().Name}: {UnexpectedException?.Message} | ";
            info += IsPassed ? "Passed | " : failMessage;

            info += $@"Time: {Time:mm\:ss\.fff}";
            return info;
        }
    }
}
