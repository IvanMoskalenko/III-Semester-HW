using System;
using System.Threading;

namespace Lazy
{
    public class LazyConcurrent<T> : ILazy<T>
    {
        private readonly object _lockObject = new();
        private bool _isComputed;
        private Func<T> _supplier;
        private T _result;
        public LazyConcurrent(Func<T> supplier)
        {
            Volatile.Write(ref _supplier, supplier);
        }
        public T Get()
        {
            if (Volatile.Read (ref _isComputed))
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