using System;

namespace Lazy
{
    public class Lazy<T> : ILazy<T>
    {
        private bool _isComputed;
        private Func<T> _supplier;
        private T _result;
        public Lazy(Func<T> supplier)
        {
            _isComputed = false;
            _supplier = supplier;
            _result = default;
        }
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