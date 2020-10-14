using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Task3
{
    public class MyThreadPool
    {
        private class MyTask<TResult> : IMyTask<TResult>
        {
            public MyTask(Func<TResult> supplier, MyThreadPool threadPool)
            {
                this.supplier = supplier;
                this.threadPool = threadPool;
            }

            private readonly MyThreadPool threadPool;
            private readonly ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            private readonly List<Action> continuationTasks = new List<Action>();

            private Func<TResult> supplier;
            private Exception caughtException;

            public bool IsCompleted { get; private set; }

            private TResult result;
            public TResult Result
            {
                get
                {
                    manualResetEvent.WaitOne();

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
                    foreach (var taskRunAction in continuationTasks)
                    {
                        threadPool.actionQueue.Enqueue(taskRunAction);
                        threadPool.autoResetEvent.Set();
                    }
                    manualResetEvent.Set();
                }
            }

            public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> func)
            {
                if (func == null)
                    throw new ArgumentNullException(nameof(func));

                if (threadPool.cts.IsCancellationRequested)
                    throw new InvalidOperationException();

                var newTask = new MyTask<TNewResult>(() => func(result), threadPool);
                if (IsCompleted)
                {
                    threadPool.actionQueue.Enqueue(newTask.Run);
                    threadPool.autoResetEvent.Set();
                }
                else
                {
                    continuationTasks.Add(newTask.Run);
                }

                return newTask;
            }
        }

        private readonly ConcurrentQueue<Action> actionQueue = new ConcurrentQueue<Action>();
        private readonly Thread[] threads;
        private readonly AutoResetEvent autoResetEvent = new AutoResetEvent(false);
        private readonly CancellationTokenSource cts = new CancellationTokenSource();

        public MyThreadPool(int numberOfThreads)
        {
            if (numberOfThreads <= 0)
                throw new ArgumentException("Number of threads must be greater than 0.");

            threads = new Thread[numberOfThreads];
            for (var i = 0; i < numberOfThreads; ++i)
            {
                threads[i] = new Thread(() => ExecuteTaskRunActions());
                threads[i].Start();
            }
        }

        private void ExecuteTaskRunActions()
        {
            while (!cts.IsCancellationRequested || !actionQueue.IsEmpty)
            {
                if (actionQueue.TryDequeue(out var taskRunAction))
                {
                    taskRunAction();
                }
                else
                {
                    autoResetEvent.WaitOne();

                    if (cts.IsCancellationRequested)
                        autoResetEvent.Set();
                }
            }
        }

        public IMyTask<TResult> Submit<TResult>(Func<TResult> supplier)
        {
            if (cts.IsCancellationRequested)
                throw new InvalidOperationException();

            if (supplier == null)
                throw new ArgumentNullException(nameof(supplier));

            var task = new MyTask<TResult>(supplier, this);
            actionQueue.Enqueue(task.Run);
            autoResetEvent.Set();
            return task;
        }

        public void Shutdown()
        {
            cts.Cancel();
            autoResetEvent.Set();

            foreach (var thread in threads)
            {
                thread.Join();
            }
        }
    }
}
