using System;

namespace Lazy
{
    /// <summary>
    /// Simple implementation of <see cref="ILazy{T}"/>.
    /// </summary>
    /// <typeparam name="T">Type of returning object.</typeparam>
    public class Lazy<T> : ILazy<T>
    {
        private bool _isComputed;
        private readonly Func<T> _supplier;
        private T _result;

        /// <summary>
        /// Initializes a new instance of the <see cref="Lazy{T}"/> class.
        /// </summary>
        /// <param name="supplier">Function to calculate.</param>
        public Lazy(Func<T> supplier)
        {
            _supplier = supplier;
        }

        /// <summary>
        /// Allows to calculate and return the value or return previously calculated one.
        /// </summary>
        /// <returns>Computation result.</returns>
        public T Get()
        {
            if (_isComputed)
            {
                return _result;
            }

            _result = _supplier();
            _isComputed = true;
            return _result;
        }
    }
}