using System;
using System.Reflection;

namespace MyNUnit.MethodInformation
{
    /// <summary>
    /// Reresents information about executed test method.
    /// </summary>
    public class TestResultInfo : Info
    {
        public bool IsPassed { get; }

        public Type Expected { get; }

        public Exception Unexpected { get; }

        public TimeSpan Time { get; }

        public TestResultInfo(MethodInfo methodInfo, bool isPassed,
            Type expected, Exception unexpected, TimeSpan time)
            : base(methodInfo)
        {
            IsPassed = isPassed;
            Expected = expected;
            Unexpected = unexpected;
            Time = time;
        }

        public override string ToString()
        {
            var info = base.ToString() + " ";

            info += IsPassed ? "Passed | " : $"FAILED:  {FailedMessage} | ";

            info += $@"{Time:mm\:ss\.fff}";
            return info;
        }

        private string FailedMessage
        {
            get
            {
                if (Expected == null)
                {
                    return $"Unexpected exception: {Unexpected.GetType().Name} ({Unexpected.Message})";
                }
                if (Unexpected == null)
                {
                    return $"Expected: {Expected.Name}";
                }
                return $"Expected: {Expected.Name}, " +
                        $"but was: {Unexpected.GetType().Name} ({Unexpected.Message})";
            }
        }
    }
}
