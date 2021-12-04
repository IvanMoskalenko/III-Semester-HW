using System;
using System.Threading;
using Attributes;

namespace TestsSet
{
    public class ExceptionAndIgnoredTests
    {
        [Test(Expected = typeof(ArgumentOutOfRangeException))]
        public static void ExceptionTest()
        {
            Thread.Sleep(600);
            throw new ArgumentOutOfRangeException();
        }

        [Test(Ignore = "This test is empty and must be ignored")]
        public static void IgnoredTest()
        {
        }
    }
}