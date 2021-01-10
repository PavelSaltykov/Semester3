using Attributes;
using System.Collections.Generic;

namespace TestProject2
{
    public class TestsWithBeforeAndAfterMethods
    {
        private int counter;

        [Before]
        public void Before1() => counter++;

        [Before]
        public void Before2() => counter += 2;

        [Test]
        public void Test1()
        {
            List<int> list = counter == 3 ? new List<int>() : null;
            list.Add(0);
        }

        [Test]
        public void Test2()
        {
            List<string> list = counter == 3 ? new List<string>() : null;
            list.Add("0");
        }

        [After]
        public void After1() => counter--;

        [After]
        public void After2() => counter -= 2;
    }
}
