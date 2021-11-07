using System;
using System.Collections.Concurrent;
using System.Threading;

namespace ThreadPool
{
    /// <summary>
    /// Provides a pool of threads that can be used to execute tasks
    /// </summary>
    public class MyThreadPool
    {
        private readonly BlockingCollection<Action> _tasks;
        private readonly Thread[] _threads;
        private readonly CancellationTokenSource _cancellationToken;

        public MyThreadPool(int countOfThreads)
        {
            if (countOfThreads <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(countOfThreads));
            }

            _tasks = new BlockingCollection<Action>();
            _threads = new Thread[countOfThreads];
            _cancellationToken = new CancellationTokenSource();
            for (var i = 0; i < countOfThreads; i++)
            {
                _threads[i] = CreateThread();
                _threads[i].Start();
            }
        }

        /// <summary>
        /// Represents an operation
        /// </summary>
        /// <typeparam name="TResult">Type of returned result produced by operation</typeparam>
        private class MyTask<TResult> : IMyTask<TResult>
        {
            private TResult _result;
            private Func<TResult> _func;
            private AggregateException _exception;
            private readonly ConcurrentQueue<Action> _innerTasks;
            private readonly MyThreadPool _threadPool;
            private readonly ManualResetEvent _isCalculated;
            private readonly object _lockObject = new();
            
            /// <inheritdoc />
            public bool IsCompleted { get; private set; }

            /// <inheritdoc />
            public TResult Result
            {
                get
                {
                    _isCalculated.WaitOne();
                    if (_exception != null)
                    {
                        throw _exception;
                    }

                    return _result;
                }
            }

            public MyTask(Func<TResult> function, MyThreadPool pool)
            {
                _func = function;
                _threadPool = pool;
                _innerTasks = new ConcurrentQueue<Action>();
                _isCalculated = new ManualResetEvent(false);
            }

            /// <summary>
            /// Calculates the result
            /// </summary>
            public void Run()
            {
                try
                {
                    _result = _func();
                    _func = null;
                }
                catch (Exception e)
                {
                    _exception = new AggregateException(e);
                }
                
                lock (_lockObject)
                {
                    IsCompleted = true;
                    _isCalculated.Set();
                    while (!_innerTasks.IsEmpty)
                    {
                        if (!_innerTasks.TryDequeue(out var continueTask)) continue;
                        try
                        {
                            _threadPool._tasks.Add(continueTask, _threadPool._cancellationToken.Token);
                        }
                        catch (OperationCanceledException)
                        {
                            return;
                        }
                    }
                }
            }

            /// <inheritdoc />
            public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> func)
            {
                lock (_threadPool._cancellationToken)
                {

                    if (_threadPool._cancellationToken.Token.IsCancellationRequested)
                    {
                        throw new InvalidOperationException();
                    }

                    lock (_lockObject)
                    {
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
        }

        /// <summary>
        /// Add func to queue
        /// </summary>
        /// <param name="func">Func to calculate</param>
        /// <typeparam name="T">Type of returned result</typeparam>
        /// <returns>New task</returns>
        /// <exception cref="ThreadInterruptedException">Cancellation was requested</exception>
        public IMyTask<T> Submit<T>(Func<T> func)
        {
            lock (_cancellationToken)
            {
                if (_cancellationToken.Token.IsCancellationRequested)
                {
                    throw new InvalidOperationException();
                }

                var myTask = new MyTask<T>(func, this);
                _tasks.Add(myTask.Run);
                return myTask;
            }
        }

        /// <summary>
        /// Shutdowns MyThreadPool
        /// </summary>
        public void Shutdown()
        {
            lock (_cancellationToken)
            {
                _cancellationToken.Cancel();
                _tasks.CompleteAdding();
            }
            foreach (var t in _threads)
            {
                t.Join();
            }
        }

        /// <summary>
        /// Creates worker for ThreadPool
        /// </summary>
        /// <returns>Thread for ThreadPool</returns>
        private Thread CreateThread()
        {
            var thread = new Thread(() =>
            {
                foreach (var task in _tasks.GetConsumingEnumerable())
                {
                    task();
                }
            })
            {
                IsBackground = true
            };
            return thread;
        }
    }
}