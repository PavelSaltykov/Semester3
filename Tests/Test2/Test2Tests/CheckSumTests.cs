using NUnit.Framework;

namespace Test2.Tests
{
    [TestFixture]
    public class CheckSumTests
    {
        [TestCase(@"..\..\..\..")]
        [TestCase(@"..\..\..")]
        public void ComputeSingleThreadedTest(string path)
        {
            var checkSum1 = CheckSum.ComputeSingleThreaded(path);
            var checkSum2 = CheckSum.ComputeSingleThreaded(path);
            Assert.AreEqual(checkSum1, checkSum2);
        }

        [TestCase(@"..\..\..\..")]
        [TestCase(@"..\..\..")]
        public void ComputeMultiThreadedTest(string path)
        {
            var checkSum1 = CheckSum.ComputeMultiThreaded(path).Result;
            var checkSum2 = CheckSum.ComputeMultiThreaded(path).Result;
            Assert.AreEqual(checkSum1, checkSum2);
        }

        [Test]
        public void DifferentPathsSingleThreadedTest()
        {
            var checkSum1 = CheckSum.ComputeSingleThreaded(@"..\..");
            var checkSum2 = CheckSum.ComputeSingleThreaded(".");
            Assert.AreNotEqual(checkSum1, checkSum2);
        }

        [Test]
        public void DifferentPathsMultiThreadedTest()
        {
            var checkSum1 = CheckSum.ComputeMultiThreaded(@"..\..").Result;
            var checkSum2 = CheckSum.ComputeMultiThreaded(".").Result;
            Assert.AreNotEqual(checkSum1, checkSum2);
        }

        [Test]
        public void SingleAndMultiThreadedTest()
        {
            const string path = @"..\..\..\..";
            var checkSum1 = CheckSum.ComputeSingleThreaded(path);
            var checkSum2 = CheckSum.ComputeMultiThreaded(path).Result;
            Assert.AreEqual(checkSum1, checkSum2);
        }
    }
}