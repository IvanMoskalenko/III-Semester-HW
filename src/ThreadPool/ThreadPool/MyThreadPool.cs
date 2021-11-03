using System;
using System.Collections.Concurrent;

namespace ThreadPool
{
    public class MyThreadPool
    {
        private ConcurrentQueue<Action> tasks;

        public MyThreadPool(int countOfThreads)
        {
            throw new NotImplementedException();
        }

        private class MyTask<TResult> : IMyTask<TResult>
        {
            public bool IsCompleted { get; }
            public TResult Result { get; }
            private Func<TResult> _func;
            public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> func)
            {
                throw new NotImplementedException();
            }
            public MyTask(Func<TResult> function)
            {
                _func = function;
            }
        }
        
        public IMyTask<T> Submit<T>(Func<T> task)
        {
            var myTask = new MyTask<T>(task);
            return myTask;
        }
        
        
        public void Shutdown()
        {
            throw new NotImplementedException();
        }
    }
}