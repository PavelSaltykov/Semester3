using System;

namespace Task2
{
    class Lazy<T> : ILazy<T>
    {
        private bool isCalculated;
        private T result;
        private readonly Func<T> supplier;

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
