using System.Reflection;

namespace MyNUnit.MethodInformation
{
    public class IncorrectMethodInfo : Info
    {
        public string[] Messages { get; }

        public IncorrectMethodInfo(MethodInfo methodInfo, string[] messages)
            : base(methodInfo) => Messages = messages;

        public override string ToString()
        {
            var message = string.Join("; ", Messages);
            return $"{base.ToString()} Errors: {message}";
        }
    }
}
