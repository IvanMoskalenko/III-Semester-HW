using System;

namespace Attributes
{
    public class Test: Attribute
    {
        public Type Expected { get; set; }
        public string Ignore { get; set; }
    }
}