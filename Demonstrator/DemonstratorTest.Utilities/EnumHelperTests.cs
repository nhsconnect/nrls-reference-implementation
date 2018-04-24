using Demonstrator.Utilities;
using Demonstrator.Utilities.Exceptions;
using Xunit;

namespace DemonstratorTest.Utilities
{
    public class EnumHelperTests
    {
        [Fact]
        public void Enum_Check_Valid()
        {
            Assert.True(EnumHelpers.IsValidName<TestEnums>("EnumA"));
        }

        [Fact]
        public void Enum_Check_Invalid()
        {
            Assert.False(EnumHelpers.IsValidName<TestEnums>("EnumC"));
        }

        [Fact]
        public void Enum_Get_Valid()
        {
            var enumA = EnumHelpers.GetEnum<TestEnums>("EnumA");

            Assert.Equal(TestEnums.EnumA, enumA);
        }

        [Fact]
        public void Enum_Get_Invalid_Exception()
        {
            Assert.Throws<InvalidEnumException>(delegate { EnumHelpers.GetEnum<TestEnums>("EnumC"); });
        }
    }

    public enum TestEnums
    {
        EnumA,
        EnumB
    }
}
