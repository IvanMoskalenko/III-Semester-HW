using System;

namespace Attributes
{
    /// <summary>
    /// Attribute for running tests
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class Test: Attribute
    {
        /// <summary>
        /// Expected exception
        /// </summary>
        public Type Expected { get; set; }
        /// <summary>
        /// Ignoring test
        /// </summary>
        public string Ignore { get; set; }
    }
}