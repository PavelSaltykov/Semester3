using System.Reflection;

namespace MyNUnit.MethodInformation
{
    /// <summary>
    /// Reresents information about ignored test method.
    /// </summary>
    public class IgnoredTestInfo : Info
    {
        /// <summary>
        /// Reason to ignore method execution.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="IgnoredTestInfo"/> class.
        /// </summary>
        /// <param name="message">Reason to ignore.</param>
        public IgnoredTestInfo(MethodInfo methodInfo, string message)
            : base(methodInfo) => Message = message;

        public override string ToString() => $"{base.ToString()} Ignored: {Message}";
    }
}
