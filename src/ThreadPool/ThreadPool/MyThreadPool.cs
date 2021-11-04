using System;
using System.Collections.Concurrent;
using System.Threading;

namespace ThreadPool
{
    public class MyThreadPool
    {
        private readonly ConcurrentQueue<Action> _tasks;
        private readonly Thread[] _threads;
        private readonly CancellationTokenSource _cancellationToken;

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
            private readonly Func<TResult> _func;
            private AggregateException _exception;
            private readonly ConcurrentQueue<Action> _innerTasks;
            private readonly MyThreadPool _threadPool;
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

            public MyTask(Func<TResult> function, MyThreadPool pool)
            {
                _func = function;
                _threadPool = pool;
                _innerTasks = new ConcurrentQueue<Action>();
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
                while (!_innerTasks.IsEmpty)
                {
                    if (_innerTasks.TryDequeue(out var continueTask))
                    {
                        _threadPool._tasks.Enqueue(continueTask);
                    }
                }
            }

            public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> func)
            {
                lock (_threadPool._cancellationToken)
                {
                    if (_threadPool._cancellationToken.Token.IsCancellationRequested)
                    {
                        throw new ThreadInterruptedException();
                    }

                    if (IsCompleted)
                    {
                        return _threadPool.Submit(() =>
                        {
                            if (_exception != null)
                            {
                                throw _exception;
                            }

                            return func(_result);
                        });
                    }

                    var newInnerTask = new MyTask<TNewResult>(() =>
                    {
                        if (_exception != null)
                        {
                            throw _exception;
                        }

                        return func(_result);
                    }, _threadPool);
                    _innerTasks.Enqueue(newInnerTask.Run);
                    return newInnerTask;
                }
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

                var myTask = new MyTask<T>(task, this);
                _tasks.Enqueue(myTask.Run);
                return myTask;
            }
        }


        public void Shutdown()
        {
            _cancellationToken.Cancel();

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