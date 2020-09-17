using NUnit.Framework;
using System;
using System.Threading;

namespace Task2.Tests
{
    [TestFixture]
    public class LazyTests
    {
        private static readonly Func<Func<int>, ILazy<int>>[] creationFunctions =
        {
            supplier => LazyFactory.CreateLazy(supplier),
            supplier => LazyFactory.CreateThreadSafeLazy(supplier),
        };

        [TestCaseSource(nameof(creationFunctions))]
        public void NullSupplierTest(Func<Func<int>, ILazy<int>> create)
        {
            Func<int> func = null;
            Assert.Throws<ArgumentNullException>(() => create(func));
        }

        [TestCaseSource(nameof(creationFunctions))]
        public void GetTest(Func<Func<int>, ILazy<int>> create)
        {
            Func<int> func = () => 1;
            var lazy = create(func);
            Assert.AreEqual(1, lazy.Get());
        }

        [TestCaseSource(nameof(creationFunctions))]
        public void GetTwiceTest(Func<Func<int>, ILazy<int>> create)
        {
            var value = 0;
            Func<int> func = () => ++value;
            var lazy = create(func);

            Assert.AreEqual(1, lazy.Get());
            Assert.AreEqual(1, lazy.Get());
        }

        [Test]
        [Repeat(10)]
        public void MultithreadedTest()
        {
            var value = 0;
            Func<int> func = () => --value;
            var lazy = LazyFactory.CreateThreadSafeLazy(func);

            var threads = new Thread[Environment.ProcessorCount];
            var results = new int[threads.Length * 2];
            for (var i = 0; i < threads.Length; ++i)
            {
                var localI = i;
                threads[i] = new Thread(() =>
                {
                    results[localI * 2] = lazy.Get();
                    results[localI * 2 + 1] = lazy.Get();
                });
            }

            foreach (var thread in threads)
            {
                thread.Start();
            }
            foreach (var thread in threads)
            {
                thread.Join();
            }

            foreach (var result in results)
            {
                Assert.AreEqual(-1, result);
            }
        }
    }
}