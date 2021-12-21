using System;

namespace Attributes
{
    /// <summary>
    /// Attribute for running methods after tests' class
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class AfterClass: Attribute
    {
        
    }
}