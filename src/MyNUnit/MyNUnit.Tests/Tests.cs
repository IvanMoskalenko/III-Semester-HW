using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace MyNUnit.Tests
{
    public class Tests
    {
        private static readonly ConcurrentQueue<TestInformation> Info =
            new MyNUnit("../../../../TestsSet").TestsInformation;

        private readonly Dictionary<string, TestInformation> _testsInfo =
            Info.ToDictionary(testInfo => testInfo.Name);

        [Test]
        public void PassingTestsShouldPass()
        {
            var testInfo = _testsInfo["PassingTest"];
            Assert.AreEqual(testInfo.Result, "Passed");
            Assert.AreEqual(testInfo.IgnoreReason, null);
            Assert.True(testInfo.Time >= 900);
        }

        [Test]
        public void TestWithExceptionShouldWorkCorrectly()
        {
            var testInfo = _testsInfo["ExceptionTest"];
            Assert.AreEqual(testInfo.Result, "Passed");
            Assert.AreEqual(testInfo.IgnoreReason, null);
            Assert.True(testInfo.Time >= 600);
        }

        [Test]
        public void TestWithIgnoreShouldWorkCorrectly()
        {
            var testInfo = _testsInfo["IgnoredTest"];
            Assert.AreEqual(testInfo.Result, "Ignored");
            Assert.AreEqual(testInfo.IgnoreReason, "This test is empty and must be ignore");
        }

        [Test]
        public void FallingTestShouldFall()
        {
            var testInfo = _testsInfo["FallingTest"];
            Assert.AreEqual(testInfo.Result, "Failed");
            Assert.AreEqual(testInfo.IgnoreReason, null);
            Assert.True(testInfo.Time >= 300);
        }
    }
}