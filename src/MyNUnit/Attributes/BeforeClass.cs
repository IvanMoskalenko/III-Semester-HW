using System;

namespace Attributes
{
    /// <summary>
    /// Attribute for running methods before tests' class
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class BeforeClass: Attribute
    {
        
    }
}