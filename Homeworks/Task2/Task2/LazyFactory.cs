using System;

namespace Task2
{
    /// <summary>
    /// Creates lazy calculation objects.
    /// </summary>
    public static class LazyFactory
    {
        /// <summary>
        /// Creates a lazy calculation object.
        /// </summary>
        /// <typeparam name="T">The type of the object returned by the supplier.</typeparam>
        /// <param name="supplier">The function that calculates.</param>
        /// <exception cref="ArgumentNullException">Thrown when supplier is null.</exception>
        public static ILazy<T> CreateLazy<T>(Func<T> supplier)
        {
            if (supplier == null)
                throw new ArgumentNullException();

            return new Lazy<T>(supplier);
        }

        /// <summary>
        /// Creates a thread-safe lazy calculation object.
        /// </summary>
        /// <typeparam name="T">The type of the object returned by the supplier.</typeparam>
        /// <param name="supplier">The function that calculates.</param>
        /// <exception cref="ArgumentNullException">Thrown when supplier is null.</exception>
        public static ILazy<T> CreateThreadSafeLazy<T>(Func<T> supplier)
        {
            if (supplier == null)
                throw new ArgumentNullException();

            return new ThreadSafeLazy<T>(supplier);
        }
    }
}
