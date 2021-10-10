using System;

namespace Lazy
{
    /// <summary>
    /// Factory for creating new instances of <see cref="Lazy{T}"/> and <see cref="LazyConcurrent{T}"/> classes.
    /// </summary>
    public static class LazyFactory
    {
        /// <summary>
        /// Creates new instance of <see cref="Lazy{T}"/> class.
        /// </summary>
        /// <param name="supplier">Function to calculate.</param>
        /// <returns>New instance of <see cref="Lazy{T}"/> class.</returns>
        public static ILazy<T> CreateLazy<T>(Func<T> supplier) => new Lazy<T>(supplier);

        /// <summary>
        /// Creates new instance of <see cref="LazyConcurrent{T}"/> class.
        /// </summary>
        /// <param name="supplier">Function to calculate.</param>
        /// <returns>New instance of <see cref="LazyConcurrent{T}"/> class.</returns>
        public static ILazy<T> CreateLazyConcurrent<T>(Func<T> supplier) => new LazyConcurrent<T>(supplier);
    }
}