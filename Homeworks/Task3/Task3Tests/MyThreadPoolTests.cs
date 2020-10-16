using NUnit.Framework;
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
            if (!waitingThread.Join(100))
            {
                Assert.Fail($"Number of threads in the {nameof(threadPool)} is less than {expectedNumberOfThreads}");
            }
        }

        [Test]
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
        public void SubmitAfterShutdownTest()
        {
            threadPool.Shutdown();
            Assert.Throws<InvalidOperationException>(() => threadPool.Submit(() => 0));
        }

        [Test]
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
        public void SimpleContinueWithTest()
        {
            var task = threadPool.Submit(() => 0);
            _ = task.Result;

            var continuationTask = task.ContinueWith(x => x + 5);
            Assert.AreEqual(5, continuationTask.Result);
        }

        [Test]
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
            if (!threadWithContinuationTask.Join(100))
            {
                Assert.Fail("ContinueWith blocked thread");
            }
        }

        [Test]
        public void ShutdownAfterContinueWithTest()
        {
            var task = threadPool.Submit(() => 0);

            var continuationTask = task.ContinueWith(x =>
            {
                Thread.Sleep(20);
                return x.ToString();
            });

            threadPool.Shutdown();
            Assert.IsTrue(continuationTask.IsCompleted);
            Assert.AreEqual("0", continuationTask.Result);
        }

        [Test]
        public void ContinueWithAfterShutdownTest()
        {
            var task = threadPool.Submit(() => 0);

            threadPool.Shutdown();
            Assert.Throws<InvalidOperationException>(() => task.ContinueWith(x => x.ToString()));
        }
    }
}