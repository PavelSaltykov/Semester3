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

        private readonly ConcurrentQueue<Action> actionQueue = new ConcurrentQueue<Action>();
        private readonly Thread[] threads;
        private readonly AutoResetEvent AutoResetEvent = new AutoResetEvent(false);
        private readonly CancellationTokenSource cts = new CancellationTokenSource();

        public MyThreadPool(int numberOfThreads)
        {
            if (numberOfThreads <= 0)
                throw new ArgumentException("Number of threads must be greater than 0.");

            threads = new Thread[numberOfThreads];
            for (var i = 0; i < numberOfThreads; ++i)
            {
                threads[i] = new Thread(() => ExecuteActions());
                threads[i].Start();
            }
        }

        private void ExecuteActions()
        {
            while (!cts.IsCancellationRequested || !actionQueue.IsEmpty)
            {               
                if (actionQueue.TryDequeue(out var runTask))
                {
                    runTask();
                }
                else
                {
                    AutoResetEvent.WaitOne();

                    if (cts.IsCancellationRequested)
                        AutoResetEvent.Set();
                }
            }
        }

        public IMyTask<TResult> Submit<TResult>(Func<TResult> supplier)
        {
            if (cts.IsCancellationRequested)
                throw new InvalidOperationException();

            if (supplier == null)
                throw new ArgumentNullException(nameof(supplier));

            var task = new MyTask<TResult>(supplier);
            actionQueue.Enqueue(task.Run);
            AutoResetEvent.Set();
            return task;
        }

        public void Shutdown()
        {
            cts.Cancel();
            AutoResetEvent.Set();

            foreach (var thread in threads)
            {
                thread.Join();
            }
        }
    }
}
