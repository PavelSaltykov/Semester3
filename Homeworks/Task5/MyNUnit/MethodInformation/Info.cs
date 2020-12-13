using System.Reflection;

namespace MyNUnit.MethodInformation
{
    /// <summary>
    /// Reresents information about test method.
    /// </summary>
    public abstract class Info
    {
        public string AssemblyName { get; }

        public string ClassName { get; }

        public string MethodName { get; }

        public Info(MethodInfo methodInfo)
        {
            AssemblyName = methodInfo.DeclaringType.Assembly.GetName().Name;
            ClassName = methodInfo.DeclaringType.Name;
            MethodName = methodInfo.Name;
        }

        public override string ToString() => MethodName;
    }
}
