using System;

namespace Task2
{
    public static class LazyFactory<T>
    {
        public static ILazy<T> CreateLazy(Func<T> supplier)
        {
            if (supplier == null)
                throw new ArgumentNullException();

            return new Lazy<T>(supplier);
        }

        public static ILazy<T> CreateMultithreadedLazy(Func<T> supplier)
        {
            if (supplier == null)
                throw new ArgumentNullException();

            return new MultithreadedLazy<T>(supplier);
        }
    }
}
