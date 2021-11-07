using System;
using System.IO;
using System.Threading;
using NUnit.Framework;

namespace ThreadPool.Tests
{
    public class Tests
    {
        private const int CountOfTasks = 100;
        private readonly IMyTask<int>[] _tasks = new IMyTask<int>[CountOfTasks];
        private readonly Func<int>[] _functions = new Func<int>[CountOfTasks];
        private MyThreadPool _threadPool;

        [SetUp]
        public void Setup()
        {
            _threadPool = new MyThreadPool(10);
            for (var i = 0; i < CountOfTasks; i++)
            {
                var index = i;
                _functions[i] = () => index;
            }

            for (var i = 0; i < CountOfTasks; i++)
            {
                _tasks[i] = _threadPool.Submit(_functions[i]);
            }
        }

        [Test]
        public void ThreadPoolShouldCalculateSimpleTasks()
        {
            for (var i = 0; i < CountOfTasks; i++)
            {
                Assert.AreEqual(i, _tasks[i].Result);
            }
        }

        [Test]
        public void ThreadPoolShouldCalculateSubmittedTasksAfterShutdown()
        {
            _threadPool.Shutdown();
            for (var i = 0; i < CountOfTasks; i++)
            {
                Assert.AreEqual(i, _tasks[i].Result);
            }
        }

        [Test]
        public void ContinueWithShouldCalculateSimpleTasks()
        {
            var continueTasks = new IMyTask<int>[CountOfTasks];
            for (var i = 0; i < CountOfTasks; i++)
            {
                continueTasks[i] = _tasks[i].ContinueWith(x => x + 1);
            }
            for (var i = 0; i < CountOfTasks; i++)
            {
                Assert.AreEqual(i + 1, continueTasks[i].Result);
            }
        }
        
        [Test]
        public void ContinueWithShouldCalculateSimpleTasksAfterShutdown()
        {
            var mre = new ManualResetEvent(false);
            var continueTasks = new IMyTask<int>[CountOfTasks];
            for (var i = 0; i < CountOfTasks; i++)
            {
                continueTasks[i] = _tasks[i].ContinueWith(x =>
                {
                    mre.WaitOne();
                    return x + 1;
                });
            }
            var thread = new Thread(() => _threadPool.Shutdown());
            thread.Start();
            mre.Set();
            thread.Join();

            for (var i = 0; i < CountOfTasks; i++)
            {
                Assert.AreEqual(i + 1, continueTasks[i].Result);
            }
        }

        [Test]
        public void ThreadPoolShouldRaiseExceptionWhenNumberOfThreadsLessOrEqualToZero()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var _ = new MyThreadPool(-228);
            });
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var _ = new MyThreadPool(0);
            });
        }

        [Test]
        public void ThreadPoolAndContinueWithShouldRaiseExceptionWhenSubmitToTurnedOffPool()
        {
            _threadPool.Shutdown();
            Assert.Throws<InvalidOperationException>(() => _threadPool.Submit(() => 0));
            var newThreadPool = new MyThreadPool(10);
            var task = newThreadPool.Submit(() => 0);
            newThreadPool.Shutdown();
            Assert.Throws<InvalidOperationException>(() => task.ContinueWith(x => x + 1));
        }

        [Test]
        public void ResultShouldThrowExceptions()
        {
            var task = _threadPool.Submit<object>(() => throw new Exception());
            Assert.Throws<AggregateException>(() =>
            {
                var _ = task.Result;
            });
        }

    }
}