using System;

namespace Task2
{
    public static class LazyFactory
    {
        public static ILazy<T> CreateLazy<T>(Func<T> supplier)
        {
            if (supplier == null)
                throw new ArgumentNullException();

            return new Lazy<T>(supplier);
        }

        public static ILazy<T> CreateMultithreadedLazy<T>(Func<T> supplier)
        {
            if (supplier == null)
                throw new ArgumentNullException();

            return new MultithreadedLazy<T>(supplier);
        }
    }
}
