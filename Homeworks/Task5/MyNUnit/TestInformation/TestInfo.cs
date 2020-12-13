namespace MyNUnit.TestInformation
{
    /// <summary>
    /// Reresents information about test method.
    /// </summary>
    public abstract class TestInfo
    {
        public string AssemblyName { get; }

        public string ClassName { get; }

        public string MethodName { get; }

        public TestInfo(string assemblyName, string className, string methodName)
        {
            AssemblyName = assemblyName;
            ClassName = className;
            MethodName = methodName;
        }

        public override string ToString() => MethodName;
    }
}
