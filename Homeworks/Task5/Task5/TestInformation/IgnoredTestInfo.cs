namespace Task5.TestInformation
{
    public class IgnoredTestInfo : TestInfo
    {
        public string Message { get; }

        public IgnoredTestInfo(string assemblyName, string className, string methodName, string message)
            : base(assemblyName, className, methodName)
        {
            Message = message;
        }

        public override string ToString()
        {
            return $"{base.ToString()} Ignored: {Message}";
        }
    }
}
