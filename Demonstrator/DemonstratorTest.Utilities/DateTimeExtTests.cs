using Demonstrator.Utilities.Extensions;
using Xunit;

namespace DemonstratorTest.Utilities
{
    public class DateTimeExtTests
    {
        [Theory]
        [InlineData("2001")]
        [InlineData("-2001")]
        [InlineData("2001-02")]
        [InlineData("2001-02-03")]
        public void Date_From_String_Valid(string strDate)
        {
            var dateMatch = strDate.ToDateTime();

            Assert.NotNull(dateMatch);
        }

        [Theory]
        [InlineData("-")]
        [InlineData("200")]
        [InlineData("-001")]
        [InlineData("201-01-23")]
        [InlineData("201-01-")]
        [InlineData("bad_date")]
        public void Date_From_String_Invalid(string strDate)
        {
            var dateMatch = strDate.ToDateTime();

            Assert.Null(dateMatch);
        }
    }
}
