using NUnit.Framework;
using System;
using System.Collections.Concurrent;
using System.Threading;
using Task3;

namespace Task3Tests
{
    [TestFixture]
    public class MyThreadPoolThreadSafetyTests
    {
        private MyThreadPool threadPool;
        private ManualResetEvent manualResetEvent;
        private ConcurrentQueue<int> results;

        private const int numberOfThreads = 4;
        private Thread[] threads;

        [SetUp]
        public void SetUp()
        {
            threadPool = new MyThreadPool(Environment.ProcessorCount);
            manualResetEvent = new ManualResetEvent(false);
            results = new ConcurrentQueue<int>();
            threads = new Thread[numberOfThreads];
        }

        [Test]
        public void ResultThreadSafeTest()
        {
            var task = threadPool.Submit(() =>
            {
                manualResetEvent.WaitOne();
                return 1;
            });

            for (var i = 0; i < numberOfThreads; ++i)
            {
                threads[i] = new Thread(() => results.Enqueue(task.Result));
                threads[i].Start();
            }

            manualResetEvent.Set();
            foreach (var thread in threads)
            {
                if (!thread.Join(100))
                {
                    Assert.Fail("Deadlock");
                }
            }

            Assert.AreEqual(numberOfThreads, results.Count);
            foreach (var result in results)
            {
                Assert.AreEqual(1, result);
            }
        }

        [Test]
        [Repeat(100)]
        public void ContinueWithThreadSafeTest()
        {
            var task = threadPool.Submit(() =>
            {
                manualResetEvent.WaitOne();
                Thread.Sleep(2);
                return -1;
            });

            for (var i = 0; i < numberOfThreads; ++i)
            {
                threads[i] = new Thread(() =>
                {
                    manualResetEvent.Set();
                    results.Enqueue(task.ContinueWith(x => -x).Result);
                });
            }

            foreach (var thread in threads)
            {
                thread.Start();
            }
            foreach (var thread in threads)
            {
                if (!thread.Join(200))
                {
                    Assert.Fail("Deadlock");
                }
            }

            Assert.AreEqual(numberOfThreads, results.Count);
            foreach (var result in results)
            {
                Assert.AreEqual(1, result);
            }
        }

        [Test]
        public void SubmitThreadSafeTest()
        {
            for (var i = 0; i < numberOfThreads; ++i)
            {
                threads[i] = new Thread(() => results.Enqueue(threadPool.Submit(() => 10).Result));
                threads[i].Start();
            }

            foreach (var thread in threads)
            {
                if (!thread.Join(100))
                {
                    Assert.Fail("Deadlock");
                }
            }

            Assert.AreEqual(numberOfThreads, results.Count);
            foreach (var result in results)
            {
                Assert.AreEqual(10, result);
            }
        }

        [Test]
        [Repeat(100)]
        public void SubmitWithShutdownTest()
        {
            for (var i = 0; i < numberOfThreads; ++i)
            {
                threads[i] = new Thread(() =>
                {
                    try
                    {
                        manualResetEvent.WaitOne();
                        results.Enqueue(threadPool.Submit(() => 10).Result);
                    }
                    catch (InvalidOperationException) { }
                });

                threads[i].Start();
            }

            var threadWithShutdown = new Thread(() =>
            {
                manualResetEvent.WaitOne();
                threadPool.Shutdown();
            });
            threadWithShutdown.Start();

            manualResetEvent.Set();

            foreach (var thread in threads)
            {
                if (!thread.Join(100))
                {
                    Assert.Fail("Deadlock");
                }
            }
        }
    }
}
