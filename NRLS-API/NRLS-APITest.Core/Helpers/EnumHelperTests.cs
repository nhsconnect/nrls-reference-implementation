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
    }

    public enum TestEnums
    {
        EnumA,
        EnumB
    }
}
