﻿using System;

namespace Task5.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class AfterClassAttribute : Attribute
    {
    }
}
