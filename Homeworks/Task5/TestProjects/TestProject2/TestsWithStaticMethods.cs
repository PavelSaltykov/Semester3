using System.Collections.Generic;
using Task5.Attributes;

namespace TestProject2
{
    public class TestsWithStaticMethods
    {
        private static int counter;

        [BeforeClass]
        public static void BeforeClass1() => counter++;

        [BeforeClass]
        public static void BeforeClass2() => counter += 2;

        [Test]
        public void Test()
        {
            List<int> list = counter == 3 ? new List<int>() : null;
            list.Add(0);
        }

        [AfterClass]
        public static void After() => counter = 0;
    }
}
