using System;

namespace Lazy
{
    public class LazyFactory<T>
    {
        public static ILazy<T> CreateLazy(Func<T> supplier)
        {
            return new Lazy<T>(supplier);
        }

        public static ILazy<T> CreateLazyConcurrent(Func<T> supplier)
        {
            return new LazyConcurrent<T>(supplier);
        }
    }
}