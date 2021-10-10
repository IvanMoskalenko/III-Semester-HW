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
        private Func<T> _supplier;
        private T _result;

        /// <summary>
        /// Initializes a new instance of the <see cref="Lazy{T}"/> class.
        /// </summary>
        /// <param name="supplier">Function to calculate.</param>
        /// <exception cref="ArgumentNullException">supplier function is null.</exception>
        public Lazy(Func<T> supplier) 
            => _supplier = supplier ?? throw new ArgumentNullException(nameof(supplier));

        /// <inheritdoc/>
        public T Get()
        {
            if (_isComputed)
            {
                return _result;
            }

            _result = _supplier();
            _supplier = null;
            _isComputed = true;
            return _result;
        }
    }
}