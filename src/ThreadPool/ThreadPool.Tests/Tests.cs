using System;
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

        [Test]
        public void ThreadPoolShouldWorkCorrectlyWithSubmitAndShutdownWhenWorksConcurrently()
        {
            var mre = new ManualResetEvent(false);
            var t1 = new Thread(() =>
            {
                _threadPool.Shutdown();
                mre.Set();
            });
            var t2 = new Thread(() =>
            {
                mre.WaitOne();
                Assert.Throws<InvalidOperationException>(() => _threadPool.Submit(() => 228));
            });
            t2.Start();
            t1.Start();
            t2.Join();
            t1.Join();

            var newThreadPool = new MyThreadPool(10);
            mre.Reset();
            var t3 = new Thread(() =>
            {
                mre.WaitOne();
                newThreadPool.Shutdown();
            });
            var t4 = new Thread(() =>
            {
                var task = newThreadPool.Submit(() => 1);
                mre.Set();
                Assert.AreEqual(1, task.Result);
            });
            t4.Start();
            t3.Start();
            t4.Join();
            t3.Join();
        }

        [Test]
        public void ThreadPoolShouldWorkCorrectlyWithContinueWithAndShutdownWhenWorksConcurrently()
        {
            var mre = new ManualResetEvent(false);
            var mre1 = new ManualResetEvent(false);
            var t1 = new Thread(() =>
            {
                mre1.WaitOne();
                _threadPool.Shutdown();
                mre.Set();
            });
            var t2 = new Thread(() =>
            {
                var task = _threadPool.Submit(() => 228);
                mre1.Set();
                mre.WaitOne();
                Assert.Throws<InvalidOperationException>(() =>
                {
                    task.ContinueWith(x => x + 1);
                });
            });
            t2.Start();
            t1.Start();
            t2.Join();
            t1.Join();

            var newThreadPool = new MyThreadPool(10);
            mre.Reset();
            mre1.Reset();
            var t3 = new Thread(() =>
            {
                mre.WaitOne();
                newThreadPool.Shutdown();
            });
            var t4 = new Thread(() =>
            {
                var task = newThreadPool.Submit(() => 1);
                var newTask = task.ContinueWith(x => x + 1);
                mre.Set();
                Assert.AreEqual(2, newTask.Result);
            });
            t4.Start();
            t3.Start();
            t4.Join();
            t3.Join();
        }

    }
}