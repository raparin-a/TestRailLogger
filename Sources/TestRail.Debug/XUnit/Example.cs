using TestRail.Logger.XUnit;
using Xunit;

namespace TestRail.Debug.XUnit
{
    public class Example
    {
        [Fact]
        public void PositiveTest()
        {
            Assert.True(true);
        }

        [Fact]
        [CaseId(182558)]
        public void NegativeTest()
        {
            Assert.True(false);
        }

        [Fact]
        public void TestCaseWithoutAttribute()
        {
            Assert.True(true);
        }

        [Fact]
        public void NonExistingTestCase()
        {
            Assert.True(true);
        }
    }
}