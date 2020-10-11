using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Task3
{
    public class MyThreadPool
    {
        private class MyTask<TResult> : IMyTask<TResult>
        {
            public MyTask(Func<TResult> supplier) => this.supplier = supplier;

            private Func<TResult> supplier;
            public bool IsCompleted { get; private set; }
            private Exception caughtException;
            private readonly ManualResetEvent ManualResetEvent = new ManualResetEvent(false);

            private TResult result;
            public TResult Result
            {
                get
                {
                    ManualResetEvent.WaitOne();

                    if (caughtException != null)
                        throw new AggregateException(caughtException);

                    return result;
                }
            }

            public void Run()
            {
                try
                {
                    result = supplier();
                }
                catch (Exception e)
                {
                    caughtException = e;
                }
                finally
                {
                    supplier = null;
                    IsCompleted = true;
                    ManualResetEvent.Set();
                }
            }

            public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> func)
            {
                throw new NotImplementedException();
            }
        }

        private ConcurrentQueue<Action> actionQueue;
        private Thread[] threads;
        private AutoResetEvent AutoResetEvent = new AutoResetEvent(false);

        public MyThreadPool(int numberOfThreads)
        {
            if (numberOfThreads <= 0)
                throw new ArgumentException("Number of threads must be greater than 0.");

        }

    }
}
