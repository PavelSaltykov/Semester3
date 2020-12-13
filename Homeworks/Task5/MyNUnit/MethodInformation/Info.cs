using System.Reflection;

namespace MyNUnit.MethodInformation
{
    /// <summary>
    /// Reresents information about a method from a test class.
    /// </summary>
    public abstract class Info
    {
        /// <summary>
        /// Name of the assembly containing the test class with method.
        /// </summary>
        public string AssemblyName { get; }

        /// <summary>
        /// Name of the class containing the method.
        /// </summary>
        public string ClassName { get; }

        /// <summary>
        /// Name of the method.
        /// </summary>
        public string MethodName { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Info"/> class.
        /// </summary>
        public Info(MethodInfo methodInfo)
        {
            AssemblyName = methodInfo.DeclaringType.Assembly.GetName().Name;
            ClassName = methodInfo.DeclaringType.Name;
            MethodName = methodInfo.Name;
        }

        public override string ToString() => MethodName;
    }
}
