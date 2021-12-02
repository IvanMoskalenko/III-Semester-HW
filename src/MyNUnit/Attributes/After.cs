using System;

namespace Attributes
{
    /// <summary>
    /// Attribute for running methods after tests
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class After: Attribute
    {
        
    }
}