using System.Threading;
using Task5.Attributes;

namespace TestProject1
{
    public class Class1
    {
        [Test(Ignore = "ignore")]
        public int Test1() => 0;

        [Test()]
        public void Test()
        {
            Thread.Sleep(10);
        }

    }
}
