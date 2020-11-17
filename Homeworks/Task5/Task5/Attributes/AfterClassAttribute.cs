using System;

namespace Task5.Attributes
{
    /// <summary>
    /// Identifies a static method to be called once after all tests have run.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class AfterClassAttribute : Attribute
    {
    }
}
