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
        private readonly ConcurrentQueue<Action> _tasks;
        private readonly Thread[] _threads;
        private readonly CancellationTokenSource _cancellationToken;

        public MyThreadPool(int countOfThreads)
        {
            if (countOfThreads <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(countOfThreads));
            }

            _tasks = new ConcurrentQueue<Action>();
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
            private readonly Func<TResult> _func;
            private AggregateException _exception;
            private readonly ConcurrentQueue<Action> _innerTasks;
            private readonly MyThreadPool _threadPool;

            /// <inheritdoc />
            public bool IsCompleted { get; private set; }

            /// <inheritdoc />
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

            /// <summary>
            /// Calculates the result
            /// </summary>
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

            /// <inheritdoc />
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
                    throw new ThreadInterruptedException();
                }

                var myTask = new MyTask<T>(func, this);
                _tasks.Enqueue(myTask.Run);
                return myTask;
            }
        }

        /// <summary>
        /// Shutdowns MyThreadPool
        /// </summary>
        public void Shutdown()
        {
            _cancellationToken.Cancel();

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