using System;

namespace MyNUnit.TestInformation
{
    /// <summary>
    /// Reresents information about executed test method.
    /// </summary>
    public class TestResultInfo : TestInfo
    {
        public bool IsPassed { get; }

        public Type Expected { get; }

        public Exception Unexpected { get; }

        public TimeSpan Time { get; }

        public TestResultInfo(string assemblyName, string className, string methodName,
            bool isPassed, Type expectedException, Exception unexpected, TimeSpan time)
            : base(assemblyName, className, methodName)
        {
            IsPassed = isPassed;
            Expected = expectedException;
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
