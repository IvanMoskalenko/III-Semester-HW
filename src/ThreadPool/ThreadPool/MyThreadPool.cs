using System;
using System.Collections.Concurrent;
using System.Threading;

namespace ThreadPool
{
    public class MyThreadPool
    {
        private ConcurrentQueue<Action> _tasks;
        private Thread[] _threads;
        private CancellationTokenSource _cancellationToken;

        public MyThreadPool(int countOfThreads)
        {
            _tasks = new ConcurrentQueue<Action>();
            _threads = new Thread[countOfThreads];
            _cancellationToken = new CancellationTokenSource();
            for (var i = 0; i < countOfThreads; i++)
            {
                _threads[i] = CreateThread();
                _threads[i].Start();
            }
            
        }

        private class MyTask<TResult> : IMyTask<TResult>
        {
            private TResult _result;
            private Func<TResult> _func;
            private AggregateException _exception;
            public bool IsCompleted { get; private set; }

            public TResult Result
            {
                get
                {
                    while (true)
                    {
                        if (!IsCompleted) continue;
                        if (_exception != null)
                        {
                            throw _exception;
                        }

                        return _result;
                    }
                }
            }
            
            public MyTask(Func<TResult> function)
            {
                _func = function;
            }

            public void Run()
            {
                try
                {
                    _result = _func();
                }
                catch (Exception e)
                {
                    _exception = new AggregateException(e);
                }
                IsCompleted = true;
            }
            public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> func)
            {
                throw new NotImplementedException();
            }
        }

        public IMyTask<T> Submit<T>(Func<T> task)
        {
            lock (_cancellationToken)
            {
                if (_cancellationToken.Token.IsCancellationRequested)
                {
                    throw new ThreadInterruptedException();
                }
            }
            var myTask = new MyTask<T>(task);
            _tasks.Enqueue(myTask.Run);
            return myTask;
        }
        
        
        public void Shutdown()
        {
            lock (_cancellationToken)
            {
                _cancellationToken.Cancel();
            }

            foreach (var t in _threads)
            {
                t.Join();
            }
            
        }

        private Thread CreateThread()
        {
            var thread = new Thread(() =>
            {
                while (true)
                {
                    if (_cancellationToken.Token.IsCancellationRequested && _tasks.IsEmpty)
                    {
                        return;
                    }
                    if (!_tasks.TryDequeue(out var task)) continue;
                    task();
                }
            });
            return thread;
        }
    }
}