using System;

namespace Task5.Attributes
{
    /// <summary>
    /// Identifies a method to be called before each test is run.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class BeforeAttribute : Attribute
    {
    }
}
