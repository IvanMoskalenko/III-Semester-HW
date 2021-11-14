using NUnit.Framework;

namespace MyNUnit.Tests
{
    public class Tests
    {
        private readonly TestInformation[] _info = new MyNUnit("../../../../TestsSet").TestsInformation.ToArray();

        [Test]
        public void PassingTestsShouldPass()
        {
            Assert.AreEqual(_info[3].Result, "Passed");
            Assert.AreEqual(_info[3].IgnoreReason, null);
            Assert.True(_info[3].Time >= 900);
        }

        [Test]
        public void TestWithExceptionShouldWorkCorrectly()
        {
            Assert.AreEqual(_info[2].Result, "Passed");
            Assert.AreEqual(_info[2].IgnoreReason, null);
            Assert.True(_info[2].Time >= 600);
        }

        [Test]
        public void TestWithIgnoreShouldWorkCorrectly()
        {
            Assert.AreEqual(_info[0].Result, "Ignored");
            Assert.AreEqual(_info[0].IgnoreReason, "This test is empty and must be ignore");
        }

        [Test]
        public void FallingTestShouldFall()
        {
            Assert.AreEqual(_info[1].Result, "Failed");
            Assert.AreEqual(_info[1].IgnoreReason, null);
            Assert.True(_info[3].Time >= 300);
        }
    }
}