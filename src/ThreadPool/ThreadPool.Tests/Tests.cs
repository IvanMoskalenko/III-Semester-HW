using System;
using NUnit.Framework;

namespace ThreadPool.Tests
{
    public class Tests
    {
        private const int CountOfTasks = 100;
        private readonly IMyTask<int>[] _tasks = new IMyTask<int>[CountOfTasks];
        private readonly MyThreadPool _threadPool = new(10);
        private readonly Func<int>[] _functions = new Func<int>[CountOfTasks];
        
        
        [SetUp]
        public void Setup()
        {
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
        public void ThreadPoolShouldCalculateSubmittedTasksWhenUserShutdown()
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


    }
}