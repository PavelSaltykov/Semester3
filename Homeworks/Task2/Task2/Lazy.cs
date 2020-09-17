using System;

namespace Task2
{
    /// <summary>
    /// Represents the lazy calulation.
    /// </summary>
    /// <typeparam name="T">The type of the returned object.</typeparam>
    internal class Lazy<T> : ILazy<T>
    {
        private bool isCalculated;
        private T result;
        private readonly Func<T> supplier;

        /// <summary>
        /// Initializes a new instance of the <see cref="Lazy{T}"/> class.
        /// </summary>
        /// <param name="supplier">The function that calculates.</param>
        /// <exception cref="ArgumentNullException">Thrown when supplier is null.</exception>
        public Lazy(Func<T> supplier) => this.supplier = supplier ?? throw new ArgumentNullException();

        public T Get()
        {
            if (isCalculated)
                return result;

            result = supplier();
            isCalculated = true;
            return result;
        }
    }
}
