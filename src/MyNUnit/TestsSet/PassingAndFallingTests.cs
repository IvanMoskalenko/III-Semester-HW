using System;
using System.Threading;
using Attributes;

namespace TestsSet
{
    public class PassingAndFallingTests
    {
        [BeforeClass]
        public static void BeforeClass()
        {
        }

        [Before]
        public static void Before()
        {
        }

        [Test]
        public static void PassingTest()
        {
            Thread.Sleep(900);
        }

        [Test]
        public static void FallingTest()
        {
            Thread.Sleep(300);
            throw new Exception();
        }

        [After]
        public static void After()
        {
        }

        [AfterClass]
        public static void AfterClass()
        {
        }
    }
}