using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;

namespace Lazy.Tests
{
    public class Tests
    {
        private static readonly object[] TestData =
        {
            228,
            1337,
            "Ibr?",
            "AF",
            "Kurdi",
            'a',
            23L
        };

        private static List<TestCaseData> Lazies()
        {
            var testCaseData = new List<TestCaseData>();
            foreach (var item in TestData)
            {
                testCaseData.Add(new TestCaseData(item, LazyFactory.CreateLazy(() => item)));
                testCaseData.Add(new TestCaseData(item, LazyFactory.CreateLazyConcurrent(() => item)));
            }

            return testCaseData;
        }

        [TestCaseSource(nameof(Lazies))]
        public void LazyAndConcurrentLazyShouldReturnTheSameObjectOnSingleCalls<T>(object item, ILazy<T> lazy)
            => Assert.AreEqual(item, lazy.Get());

        [Test]
        public void LazyAndConcurrentLazyShouldThrowExceptionWhenSupplierIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => LazyFactory.CreateLazy<object>(null));
            Assert.Throws<ArgumentNullException>(() => LazyFactory.CreateLazyConcurrent<object>(null));
        }

        [Test]
        public static void LazyShouldReturnTheSameObjectOnMultipleCalls()
        {
            var counter = 0;
            var result = LazyFactory.CreateLazy(() => ++counter);
            for (var i = 0; i < 10; i++)
            {
                Assert.AreEqual(1, result.Get());
            }
        }

        [Test]
        public static void ConcurrentLazyShouldReturnTheSameObjectOnMultipleCalls()
        {
            var counter = 0;
            var result = LazyFactory.CreateLazyConcurrent(() => ++counter);
            for (var i = 0; i < 10; i++)
            {
                Assert.AreEqual(1, result.Get());
            }
        }

        [Test]
        public static void ConcurrentLazyShouldNotAllowRaces()
        {
            var counter = 0;
            var result = LazyFactory.CreateLazyConcurrent(() => Interlocked.Increment(ref counter));
            var threads = new Thread[10000];
            for (var i = 0; i < threads.Length; i++)
            {
                threads[i] = new Thread(() =>
                {
                    for (var j = 0; j < 1000; j++)
                    {
                        Assert.AreEqual(1, result.Get());
                    }
                });
            }

            foreach (var thread in threads)
            {
                thread.Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }
        }
    }
}