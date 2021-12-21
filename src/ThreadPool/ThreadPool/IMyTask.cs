using System;

namespace ThreadPool
{
    /// <summary>
    /// Interface for MyTask, object returned by MyThreadPool
    /// </summary>
    /// <typeparam name="TResult">Type of returned result</typeparam>
    public interface IMyTask<out TResult>
    {
        /// <summary>
        /// Checks whether the task has been completed
        /// </summary>
        public bool IsCompleted { get; }

        /// <summary>
        /// Returns the result of the task
        /// </summary>
        public TResult Result { get; }

        /// <summary>
        /// Adds a new task, the result of which depends on another task
        /// </summary>
        /// <param name="func">New function to calculate</param>
        /// <typeparam name="TNewResult">Type of returned result by new function</typeparam>
        /// <returns>New task, which depends on another task</returns>
        public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> func);
    }
}