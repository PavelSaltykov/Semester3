using Attributes;

namespace TestProject3
{
    public class TestsWithIncorrectMethods
    {
        [BeforeClass]
        public void BeforeClassNonStatic()
        {
        }

        [Test]
        public int TestReturningValue() => 0;

        [Before]
        public void BeforeHavingParameter(string s)
        {
        }

        [Test]
        public static string StaticTestReturningValueAndHavingParameters(string s, int i) => s[0..i];
    }
}
