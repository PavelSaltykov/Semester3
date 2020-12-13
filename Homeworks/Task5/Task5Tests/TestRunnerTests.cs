using MyNUnit.TestInformation;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyNUnit.Tests
{
    [TestFixture]
    public class TestRunnerTests
    {
        IEnumerable<TestInfo> simpleTestsInfo;

        [OneTimeSetUp]
        public void GetSimpleTestsInfo()
        {
            var runner = new TestRunner(@"..\..\..\..\TestProjects\TestProject1");
            runner.Run();
            simpleTestsInfo = runner.GetTestsInfo();
        }

        [Test]
        public void NumberOfTestsTest()
        {
            Assert.AreEqual(6, simpleTestsInfo.Count());
            Assert.AreEqual(0, simpleTestsInfo.Where(i => i.ClassName == "WithoutAttributeTest").Count());
        }

        [Test]
        public void CheckAssemblyAndClassName()
        {
            foreach (var info in simpleTestsInfo)
            {
                Assert.AreEqual("TestProject1", info.AssemblyName);
                Assert.AreEqual("SimpleTests", info.ClassName);
            }
        }

        [Test]
        public void IgnoreTest()
        {
            var ignoredTests = simpleTestsInfo.Where(i => i is IgnoredTestInfo)
                .Select(i => (IgnoredTestInfo)i).ToArray();

            Assert.AreEqual(1, ignoredTests.Count());
            var info = ignoredTests[0];

            Assert.AreEqual("ignore", info.Message);
            Assert.AreEqual("IgnoredTest", info.MethodName);
        }

        [Test]
        public void CheckPassedTests()
        {
            var passedTests = simpleTestsInfo.Where(i => i is TestResultInfo)
                .Where(i => (i as TestResultInfo).IsPassed)
                .Select(i => i.MethodName).ToArray();

            var expectedPassedTests = new string[] { "Test", "TestWithExpectedException" };
            CollectionAssert.AreEquivalent(expectedPassedTests, passedTests);
        }

        [Test]
        public void CheckFailedTests()
        {
            var failedTests = simpleTestsInfo.Where(i => i is TestResultInfo)
                .Select(i => i as TestResultInfo).Where(i => !i.IsPassed).ToArray();

            var expectedTests = new string[]
                {
                    "TestWithException",
                    "TestWithUnexpectedException",
                    "TestNotThrowingException"
                };

            CollectionAssert.AreEquivalent(expectedTests, failedTests.Select(i => i.MethodName));

            var testInfo = failedTests.Where(i => i.MethodName == "TestWithException").First();
            Assert.Null(testInfo.Expected);
            Assert.That(testInfo.Unexpected is NullReferenceException);

            testInfo = failedTests.Where(i => i.MethodName == "TestNotThrowingException").First();
            Assert.AreEqual(typeof(NullReferenceException), testInfo.Expected);
            Assert.Null(testInfo.Unexpected);

            testInfo = failedTests.Where(i => i.MethodName == "TestWithUnexpectedException").First();
            Assert.AreEqual(typeof(ArgumentNullException), testInfo.Expected);
            Assert.That(testInfo.Unexpected is DivideByZeroException);
        }

        [Test]
        [Repeat(2)]
        public void StaticMethodsTest()
        {
            var runner = new TestRunner(@"..\..\..\..\TestProjects\TestProject2");
            runner.Run();
            var testsInfo = runner.GetTestsInfo().Where(i => i.ClassName == "TestsWithStaticMethods");

            Assert.AreEqual(1, testsInfo.Count());
            Assert.IsTrue((testsInfo.First() as TestResultInfo).IsPassed);
        }

        [Test]
        public void BeforeAndAfterTest()
        {
            var runner = new TestRunner(@"..\..\..\..\TestProjects\TestProject2");
            runner.Run();
            var testsInfo = runner.GetTestsInfo().Where(i => i.ClassName == "TestsWithBeforeAndAfterMethods");

            Assert.AreEqual(2, testsInfo.Count());
            foreach (var info in testsInfo)
            {
                Assert.IsTrue((info as TestResultInfo).IsPassed);
            }
        }
    }
}