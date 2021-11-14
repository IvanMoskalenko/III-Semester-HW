using System;
using System.Threading;
using Attributes;

namespace TestsSet
{
    public class Tests
    {
        [BeforeClass]
        public static void BeforeClass()
        {
        }

        [Before]
        public void Before()
        {
        }

        [Test]
        public void PassingTest()
        {
            Thread.Sleep(300);
        }

        [Test(Expected = typeof(ArgumentOutOfRangeException))]
        public void ExceptionTest()
        {
            Thread.Sleep(200);
            throw new ArgumentOutOfRangeException();
        }

        [Test(Ignore = "This test is empty and must be ignore")]
        public void IgnoredTest()
        {
        }

        [Test]
        public void FallingTest()
        {
            Thread.Sleep(100);
            throw new Exception();
        }

        [After]
        public void After()
        {
        }

        [AfterClass]
        public static void AfterClass()
        {
        }
    }
}