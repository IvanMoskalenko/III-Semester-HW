using System.Collections.Generic;
using System.Reflection;

namespace MyNUnit
{
    /// <summary>
    /// Implementation of lists of methods for every attribute
    /// </summary>
    public class MethodsLists
    {
        public List<MethodInfo> After { get; } = new();
        public List<MethodInfo> AfterClass { get; } = new();
        public List<MethodInfo> Before { get; } = new();
        public List<MethodInfo> BeforeClass { get; } = new();
        public List<MethodInfo> Tests { get; } = new();
    }
}