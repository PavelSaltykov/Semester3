using System;

namespace Task2
{
    class MultithreadedLazy<T> : ILazy<T>
    {
        private volatile bool isCalculated;
        private T result;
        private readonly Func<T> supplier;
        private readonly object lockObject = new object();

        public MultithreadedLazy(Func<T> supplier) => this.supplier = supplier ?? throw new ArgumentNullException();

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
