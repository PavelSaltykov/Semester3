using System.Reflection;

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

        public TestInfo(MethodInfo methodInfo)
        {
            AssemblyName = methodInfo.DeclaringType.Assembly.GetName().Name;
            ClassName = methodInfo.DeclaringType.Name;
            MethodName = methodInfo.Name;
        }

        public override string ToString() => MethodName;
    }
}
