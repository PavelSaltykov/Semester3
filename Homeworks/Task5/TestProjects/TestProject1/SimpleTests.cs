using Attributes;
using System;
using System.Collections.Generic;

namespace TestProject1
{
    public class SimpleTests
    {
        [Test(Ignore = "ignore")]
        public int IgnoredTest() => 0;

        public void WithoutAttributeTest()
        {
        }

        [Test]
        public void Test()
        {
        }

        [Test(Expected = typeof(IndexOutOfRangeException))]
        public void TestWithExpectedException()
        {
            var array = new int[1];
            array[1] = 0;
        }

        [Test]
        public void TestWithException()
        {
            List<int> list = null;
            list.Add(0);
        }

        [Test(Expected = typeof(ArgumentNullException))]
        public void TestWithUnexpectedException()
        {
            throw new DivideByZeroException();
        }

        [Test(Expected = typeof(NullReferenceException))]
        public void TestNotThrowingException()
        {
        }
    }
}
