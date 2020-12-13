using System.Reflection;

namespace MyNUnit.MethodInformation
{
    /// <summary>
    /// Reresents information about ignored test method.
    /// </summary>
    public class IgnoredTestInfo : Info
    {
        public string Message { get; }

        public IgnoredTestInfo(MethodInfo methodInfo, string message)
            : base(methodInfo) => Message = message;

        public override string ToString() => $"{base.ToString()} Ignored: {Message}";
    }
}
