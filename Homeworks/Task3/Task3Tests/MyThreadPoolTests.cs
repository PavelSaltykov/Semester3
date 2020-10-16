﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Task3.Tests
{
    [TestFixture()]
    public class MyThreadPoolTests
    {
        private MyThreadPool threadPool;

        [SetUp]
        public void SetUp() => threadPool = new MyThreadPool(Environment.ProcessorCount);

        [Test]
        [Repeat(100)]
        public void NumberOfWorkingThreadsTest()
        {
            int expectedNumberOfThreads = Environment.ProcessorCount;
            var manualResetEvent = new ManualResetEvent(false);
            using var countdownEvent = new CountdownEvent(expectedNumberOfThreads);

            for (var i = 0; i < expectedNumberOfThreads; ++i)
            {
                threadPool.Submit(() =>
                {
                    countdownEvent.Signal();
                    manualResetEvent.WaitOne();
                    return 0;
                });
            }

            var waitingThread = new Thread(() =>
            {
                countdownEvent.Wait();
                manualResetEvent.Set();
            });

            waitingThread.Start();
            if (!waitingThread.Join(10))
            {
                Assert.Fail($"Number of threads in the {nameof(threadPool)} is less than expected");
            }
        }

        [Test]
        [Repeat(100)]
        public void ResultTest()
        {
            var tasks = new List<IMyTask<int>>();

            const int numberOfTasks = 10;
            for (var i = 0; i < numberOfTasks; ++i)
            {
                var localI = i;
                tasks.Add(threadPool.Submit(() => localI));
            }

            for (var i = 0; i < numberOfTasks; ++i)
            {
                Assert.AreEqual(i, tasks[i].Result);
            }
        }

        [Test]
        [Repeat(100)]
        public void IsCompletedTest()
        {
            var manualResetEvent = new ManualResetEvent(false);
            var task = threadPool.Submit(() =>
            {
                manualResetEvent.WaitOne();
                return 0;
            });

            Assert.IsFalse(task.IsCompleted);

            manualResetEvent.Set();
            _ = task.Result;
            Assert.IsTrue(task.IsCompleted);
        }

        [Test]
        [Repeat(100)]
        public void AggregateExceptionTest()
        {
            var task = threadPool.Submit(() =>
            {
                List<int> list = null;
                list.Add(0);
                return 0;
            });

            var aggregateException = Assert.Throws<AggregateException>(() => _ = task.Result);
            Assert.IsTrue(aggregateException.InnerException is NullReferenceException);
        }

        [Test]
        [Repeat(100)]
        public void SubmitAfterShutdownTest()
        {
            threadPool.Shutdown();
            Assert.Throws<InvalidOperationException>(() => threadPool.Submit(() => 0));
        }

        [Test]
        [Repeat(100)]
        public void ShutdownWithLongTaskTest()
        {
            var expectedResult = "result";
            var longTask = threadPool.Submit(() =>
            {
                Thread.Sleep(100);
                return expectedResult;
            });

            threadPool.Shutdown();
            Assert.IsTrue(longTask.IsCompleted);
            Assert.AreEqual(expectedResult, longTask.Result);
        }

        [Test]
        [Repeat(100)]
        public void SimpleContinueWithTest()
        {
            var task = threadPool.Submit(() => -1);
            _ = task.Result;

            var continuationTask = task.ContinueWith(x => x + 5);
            Assert.AreEqual(4, continuationTask.Result);
        }

        [Test]
        [Repeat(100)]
        public void ContinueWithShouldNotBlockThreadTest()
        {
            var manualResetEvent = new ManualResetEvent(false);
            var longTask = threadPool.Submit(() =>
            {
                manualResetEvent.WaitOne();
                return 0;
            });

            var threadWithContinuationTask = new Thread(() =>
            {
                longTask.ContinueWith(x => x.ToString());
                manualResetEvent.Set();
            });

            threadWithContinuationTask.Start();
            if (!threadWithContinuationTask.Join(10))
            {
                Assert.Fail("ContinueWith blocked thread");
            }
        }

        [Test]
        [Repeat(100)]
        public void ShutdownAfterContinueWithTest()
        {
            var task = threadPool.Submit(() => -1);

            var continuationTask = task.ContinueWith(x => x.ToString());

            threadPool.Shutdown();
            Assert.IsTrue(continuationTask.IsCompleted);
            Assert.AreEqual("-1", continuationTask.Result);
        }

        [Test]
        [Repeat(100)]
        public void ContinueWithShouldBeComletedTest()
        {
            var task = threadPool.Submit(() =>
            {
                Thread.Sleep(20);
                return 1;
            });
            var continuationTask = task.ContinueWith(x => x + 1);
            threadPool.Shutdown();
            Assert.IsTrue(continuationTask.IsCompleted);
            Assert.AreEqual(2, continuationTask.Result);
        }

        [Test]
        [Repeat(100)]
        public void ContinueWithAfterShutdownTest()
        {
            var task = threadPool.Submit(() => 0);

            threadPool.Shutdown();
            Assert.Throws<InvalidOperationException>(() => task.ContinueWith(x => x.ToString()));
        }

        [Test]
        [Repeat(100)]
        public void ShutdownSeveralTimesTest()
        {
            threadPool.Submit(() => 2 * 2);
            threadPool.Shutdown();
            threadPool.Shutdown();
            Assert.Pass();
        }
    }
}