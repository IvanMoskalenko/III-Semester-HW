using System;
using System.IO;
using NUnit.Framework;

namespace ChecksumCalculator.Tests
{
    public class Tests
    {
        private readonly string _path = Path.Join(
            "..", "..", "..", "..", "ChecksumCalculator.Tests", "TestData");

        [Test]
        public void SingleThreadedAndMultiThreadedShouldBeEquivalent()
        {
            for (var i = 0; i < 100; i++)
            {
                var hashSingleThreaded = ChecksumCalculator.Calculate(_path, false);
                var hashMultiThreaded = ChecksumCalculator.Calculate(_path, true);
                Assert.AreEqual(
                    BitConverter.ToString(hashSingleThreaded),
                    BitConverter.ToString(hashMultiThreaded));
            }
        }

        [Test]
        public void ChecksumShouldNotChange()
        {
            for (var i = 0; i < 100; i++)
            {
                var hash = ChecksumCalculator.Calculate(_path, true);
                var secondHash = ChecksumCalculator.Calculate(_path, true);
                Assert.AreEqual(
                    BitConverter.ToString(hash),
                    BitConverter.ToString(secondHash));
            }
        }
    }
}