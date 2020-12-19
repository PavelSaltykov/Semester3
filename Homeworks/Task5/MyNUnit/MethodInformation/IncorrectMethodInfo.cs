using System.Reflection;

namespace MyNUnit.MethodInformation
{
    /// <summary>
    /// Reresents information about incorrect method.
    /// </summary>
    public class IncorrectMethodInfo : Info
    {
        /// <summary>
        /// Error messages.
        /// </summary>
        public string[] Messages { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="IncorrectMethodInfo"/> class.
        /// </summary>
        /// <param name="messages">Messages about errors.</param>
        public IncorrectMethodInfo(MethodInfo methodInfo, string[] messages)
            : base(methodInfo) => Messages = messages;

        public override string ToString()
        {
            var message = string.Join("; ", Messages);
            return $"{base.ToString()} Errors: {message}";
        }
    }
}
