using System;
using System.Reflection;

namespace MyNUnit.MethodInformation
{
    /// <summary>
    /// Reresents information about executed test method.
    /// </summary>
    public class TestResultInfo : Info
    {
        /// <summary>
        /// <see langword="true"/> if the test method completed successfully; otherwise, <see langword="false"/>.
        /// </summary>
        public bool IsPassed { get; }

        /// <summary>
        /// Type of the expected exception.
        /// </summary>
        public Type Expected { get; }

        /// <summary>
        /// Unexpected exception thrown during test method execution.
        /// </summary>
        public Exception Unexpected { get; }

        /// <summary>
        /// Method execution time.
        /// </summary>
        public TimeSpan Time { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestResultInfo"/> class.
        /// </summary>
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

            info += Time;
            return info;
        }

        public string FailedMessage
        {
            get
            {
                if (IsPassed)
                    return "";

                if (Expected == null)
                    return $"Unexpected exception: {Unexpected.GetType().Name} ({Unexpected.Message})";
                
                if (Unexpected == null)
                    return $"Expected: {Expected.Name}";
                
                return $"Expected: {Expected.Name}, " +
                        $"but was: {Unexpected.GetType().Name} ({Unexpected.Message})";
            }
        }
    }
}
