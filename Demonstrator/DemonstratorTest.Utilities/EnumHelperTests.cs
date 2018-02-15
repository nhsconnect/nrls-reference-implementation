using Demonstrator.Utilities;
using Xunit;

namespace DemonstratorTest.Utilities
{
    public class EnumHelperTests
    {
        [Fact]
        public void ValidEnum()
        {
            Assert.True(EnumHelpers.IsValidName<TestEnums>("EnumA"));
        }

        [Fact]
        public void InvalidEnum()
        {
            Assert.False(EnumHelpers.IsValidName<TestEnums>("EnumC"));
        }

        [Fact]
        public void ValidGetEnum()
        {
            var enumA = EnumHelpers.GetEnum<TestEnums>("EnumA");

            Assert.Equal(TestEnums.EnumA, enumA);
        }
    }

    public enum TestEnums
    {
        EnumA,
        EnumB
    }
}
