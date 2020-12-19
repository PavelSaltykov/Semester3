using System;

namespace Attributes
{
    /// <summary>
    /// Identifies a method to be called after each test is run.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class AfterAttribute : Attribute
    {
    }
}
