using System;
using System.Threading;

namespace Lazy
{
    public class LazyConcurrent<T> : ILazy<T>
    {
        private readonly object _lockObject = new();
        private volatile bool _isComputed;
        private Func<T> _supplier;
        private T _result;
        public LazyConcurrent(Func<T> supplier)
        {
            _supplier = supplier;
        }
        public T Get()
        {
            if (_isComputed)
            {
                return _result;
            }

            lock (_lockObject)
            {
                if (_isComputed)
                {
                    return _result;
                }
                _result = _supplier();
                _isComputed = true;
            }

            return _result;
        }
    }
}