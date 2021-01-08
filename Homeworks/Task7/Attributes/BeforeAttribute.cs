using System;

namespace Attributes
{
    /// <summary>
    /// Identifies a method to be called before each test is run.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class BeforeAttribute : Attribute
    {
    }
}
