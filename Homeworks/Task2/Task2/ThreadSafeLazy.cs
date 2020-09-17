using System;

namespace Task2
{
    /// <summary>
    /// Represents the thread-safe lazy calulation.
    /// </summary>
    /// <typeparam name="T">The type of the returned object.</typeparam>
    internal class ThreadSafeLazy<T> : ILazy<T>
    {
        private volatile bool isCalculated;
        private T result;
        private readonly Func<T> supplier;
        private readonly object lockObject = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadSafeLazy{T}"/> class.
        /// </summary>
        /// <param name="supplier">The function that calculates.</param>
        /// <exception cref="ArgumentNullException">Thrown when supplier is null.</exception>
        public ThreadSafeLazy(Func<T> supplier) => this.supplier = supplier ?? throw new ArgumentNullException();

        public T Get()
        {
            if (isCalculated)
                return result;

            lock (lockObject)
            {
                if (isCalculated)
                    return result;

                result = supplier();
                isCalculated = true;
                return result;
            }
        }
    }
}
