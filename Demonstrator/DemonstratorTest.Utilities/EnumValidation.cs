using Demonstrator.Utilities;
using System;
using Xunit;

namespace DemonstratorTest.Utilities
{
    public class EnumValidation
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
    }

    public enum TestEnums
    {
        EnumA,
        EnumB
    }
}
