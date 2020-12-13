using System.Reflection;

namespace MyNUnit.TestInformation
{
    /// <summary>
    /// Reresents information about ignored test method.
    /// </summary>
    public class IgnoredTestInfo : TestInfo
    {
        public string Message { get; }

        public IgnoredTestInfo(MethodInfo methodInfo, string message)
            : base(methodInfo)
        {
            Message = message;
        }

        public override string ToString()
        {
            return $"{base.ToString()} Ignored: {Message}";
        }
    }
}
