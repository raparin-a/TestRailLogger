using NUnit.Framework;
using System.Collections.Generic;
using TestRail.Logger.NUnit;

namespace TestRail.Debug.NUnit
{
    [TestFixture]
    public class Example
    {
        [Test]
        [CaseId(182556)]
        public void PositiveTest()
        {
            Assert.Pass();
        }

        [Test]
        [CaseTitle("Market entity should be removed when linked LineItem does not exist")]
        public void NegativeTest()
        {
            Assert.Fail();
        }

        [TestCaseSource(nameof(TestCaseSource))]
        [CaseId(12451683)]
        public void TestCaseData(string str1, string str2)
        {

            Assert.AreEqual(str1, str2);
        }

        public static IEnumerable<TestCaseData> TestCaseSource()
        {

            yield return new TestCaseData(
                "test1",
                "test1"
                );
            yield return new TestCaseData(
                "test1",
                "test1"
                );
            yield return new TestCaseData(
                "test1",
                "test2"
                );

        }

        [Test]
        [CaseId(0)]
        public void TestCaseWithoutAttribute()
        {
            Assert.Pass();
        }

        [Test, CaseId(182614)]
        [TestCase(false)]
        [TestCase(true)]
        public void DoubleCaseTest(bool arg)
        {
            Assert.True(arg);
        }
    }

}