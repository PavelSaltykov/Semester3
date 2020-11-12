using System;

namespace Task5.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class AfterAttribute : Attribute
    {
    }
}
