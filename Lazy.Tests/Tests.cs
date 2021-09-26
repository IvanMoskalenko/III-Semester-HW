using System.Threading;
using NUnit.Framework;

namespace Lazy.Tests
{
    public static class Tests
    {
        [Test]
        public static void SimpleLazy()
        {
            var stringResult = LazyFactory<string>.CreateLazy(() => "Ibr?");
            Assert.AreEqual("Ibr?", stringResult.Get());
            var intResult = LazyFactory<int>.CreateLazy(() => 228);
            Assert.AreEqual(228, intResult.Get());
        }

        [Test]
        public static void SimpleConcurrentLazy()
        {
            var stringResult = LazyFactory<string>.CreateLazyConcurrent(() => "Ibr?");
            Assert.AreEqual("Ibr?", stringResult.Get());
            var intResult = LazyFactory<int>.CreateLazyConcurrent(() => 228);
            Assert.AreEqual(228, intResult.Get());
        }

        [Test]
        public static void MultipleLazy()
        {
            var counter = 0;
            var result = LazyFactory<int>.CreateLazy(() => ++counter);
            for (var i = 0; i < 10; i++)
            {
                Assert.AreEqual(1, result.Get());
            }
        }

        [Test]
        public static void MultipleConcurrentLazy()
        {
            var counter = 0;
            var result = LazyFactory<int>.CreateLazyConcurrent(() => ++counter);
            for (var i = 0; i < 10; i++)
            {
                Assert.AreEqual(1, result.Get());
            }
        }

        [Test]
        public static void Races()
        {
            var counter = 0;
            var result = LazyFactory<int>.CreateLazyConcurrent(() => Interlocked.Increment(ref counter));
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