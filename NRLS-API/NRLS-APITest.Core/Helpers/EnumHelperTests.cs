using NRLS_API.Core.Exceptions;
using NRLS_API.Core.Helpers;
using Xunit;

namespace NRLS_APITest.Core.Helpers
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
        public void Enum_GetName_Valid()
        {
            Assert.IsType<TestEnums>(EnumHelpers.GetEnum<TestEnums>("EnumB"));
        }

        [Fact]
        public void Enum_GetName_Invalid()
        {
            Assert.Throws<InvalidEnumException>(() => {
                var test = EnumHelpers.GetEnum<TestEnums>("EnumC");
            });
        }
    }

    public enum TestEnums
    {
        EnumA,
        EnumB
    }
}
