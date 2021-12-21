using System;

namespace Attributes
{
    /// <summary>
    /// Attribute for running methods before tests
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class Before: Attribute
    {
        
    }
}