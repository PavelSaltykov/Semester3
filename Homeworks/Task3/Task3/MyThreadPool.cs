using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Task3
{
    /// <summary>
    /// Provides a pool of threads that can be used to execute tasks. 
    /// </summary>
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
            private readonly object lockObject = new object();
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
                    lock (lockObject)
                    {
                        IsCompleted = true;
                    }
                    manualResetEvent.Set();

                    supplier = null;
                    foreach (var taskRunAction in continuationTasks)
                    {
                        threadPool.EnqueueTaskRunAction(taskRunAction);
                        Interlocked.Decrement(ref threadPool.numberOfContinuationTasksPendingEnqueue);
                    }
                }
            }

            public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> func)
            {
                if (func == null)
                    throw new ArgumentNullException(nameof(func));

                if (threadPool.cts.IsCancellationRequested)
                    throw new InvalidOperationException();

                lock (lockObject)
                {
                    if (IsCompleted)
                    {
                        return threadPool.Submit(() => func(result));
                    }

                    lock (threadPool.ctsLockObject)
                    {
                        if (threadPool.cts.IsCancellationRequested)
                            throw new InvalidOperationException();

                        var newTask = new MyTask<TNewResult>(() => func(result), threadPool);
                        continuationTasks.Add(newTask.Run);
                        Interlocked.Increment(ref threadPool.numberOfContinuationTasksPendingEnqueue);
                        return newTask;
                    }
                }
            }
        }

        private readonly ConcurrentQueue<Action> actionQueue = new ConcurrentQueue<Action>();
        private readonly Thread[] threads;
        private readonly AutoResetEvent autoResetEvent = new AutoResetEvent(false);
        private readonly CancellationTokenSource cts = new CancellationTokenSource();
        private readonly object ctsLockObject = new object();
        private volatile int numberOfContinuationTasksPendingEnqueue;

        /// <summary>
        /// Initializes a new instanse of the <see cref="MyThreadPool"/> class and starts threads.
        /// </summary>
        /// <param name="numberOfThreads">The number of threads started in the <see cref="MyThreadPool"/>.</param>
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
            while (!cts.IsCancellationRequested || !actionQueue.IsEmpty || numberOfContinuationTasksPendingEnqueue > 0)
            {
                if (actionQueue.TryDequeue(out var taskRunAction))
                {
                    taskRunAction();
                }
                else
                {
                    autoResetEvent.WaitOne();

                    if (actionQueue.Count > 1 || cts.IsCancellationRequested)
                        autoResetEvent.Set();
                }
            }
        }

        /// <summary>
        /// Submits a task for execution.
        /// </summary>
        /// <typeparam name="TResult">The type of the result produced by the task.</typeparam>
        /// <param name="supplier">The function that calculates the result.</param>
        /// <returns>A new instance of the <see cref="IMyTask{TResult}"/>.</returns>
        public IMyTask<TResult> Submit<TResult>(Func<TResult> supplier)
        {
            if (cts.IsCancellationRequested)
                throw new InvalidOperationException();

            if (supplier == null)
                throw new ArgumentNullException(nameof(supplier));

            var task = new MyTask<TResult>(supplier, this);
            EnqueueTask(task);
            return task;
        }

        private void EnqueueTask<TResult>(MyTask<TResult> task)
        {
            lock (ctsLockObject)
            {
                if (cts.IsCancellationRequested)
                    throw new InvalidOperationException();

                EnqueueTaskRunAction(task.Run);
            }
        }

        private void EnqueueTaskRunAction(Action taskRunAction)
        {
            actionQueue.Enqueue(taskRunAction);
            autoResetEvent.Set();
        }

        /// <summary>
        /// Terminates threads.
        /// Waits until all tasks are completed blocking the calling thread.
        /// </summary>
        public void Shutdown()
        {
            lock (ctsLockObject)
            {
                cts.Cancel();
                autoResetEvent.Set();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }
        }
    }
}
