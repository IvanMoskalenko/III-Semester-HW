using System;

namespace Lazy
{
    /// <summary>
    /// Factory for creating new instances of <see cref="Lazy{T}"/> and <see cref="LazyConcurrent{T}"/> classes.
    /// </summary>
    /// <typeparam name="T">Type of returning object.</typeparam>
    public class LazyFactory<T>
    {
        /// <summary>
        /// Creates new instance of <see cref="Lazy{T}"/> class.
        /// </summary>
        /// <param name="supplier">Function to calculate.</param>
        /// <returns>New instance of <see cref="Lazy{T}"/> class.</returns>
        public static ILazy<T> CreateLazy(Func<T> supplier)
        {
            return new Lazy<T>(supplier);
        }

        /// <summary>
        /// Creates new instance of <see cref="LazyConcurrent{T}"/> class.
        /// </summary>
        /// <param name="supplier">Function to calculate.</param>
        /// <returns>New instance of <see cref="LazyConcurrent{T}"/> class.</returns>
        public static ILazy<T> CreateLazyConcurrent(Func<T> supplier)
        {
            return new LazyConcurrent<T>(supplier);
        }
    }
}