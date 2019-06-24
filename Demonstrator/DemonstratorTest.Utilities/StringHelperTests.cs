using Demonstrator.Utilities;
using Xunit;

namespace DemonstratorTest.Utilities
{
    public class StringHelperTests
    {
        [Theory]
        [InlineData("test", "test")]
        [InlineData("test 678 > £", "test 678 > £")]
        [InlineData("test 678 > £ tree 9 (happ)", "test 678 > £ tree 9 (happ)")]
        [InlineData("test 678 > £ tree 9 (happ) <", "test 678 > £ tree 9 (happ) ")]
        [InlineData("test 678 > £ tree 9 (happ) <!", "test 678 > £ tree 9 (happ) !")]
        [InlineData("test 678 > £ tree 9 (happ) <! ^", "test 678 > £ tree 9 (happ) ! ")]
        [InlineData("test 678 > £ tree 9 (happ) <! ^ $&*", "test 678 > £ tree 9 (happ) !  $&*")]
        [InlineData("test 678 > £ tree 9 (happ) <! ^ $&* {dogs}", "test 678 > £ tree 9 (happ) !  $&* dogs")]
        [InlineData("test 678 > £ tree 9 (happ) <! ^ $&* {dogs} @ #~curly~", "test 678 > £ tree 9 (happ) !  $&* dogs @ #~curly~")]
        [InlineData("test 678 > £ tree 9 (happ) <! ^ $&* {dogs} @ #~curly~ _under", "test 678 > £ tree 9 (happ) !  $&* dogs @ #~curly~ _under")]
        [InlineData("test 678 > £ tree 9 (happ) <! ^ $&* {dogs} @ #~curly~ _under[inside],;stop:'top'|` full.", "test 678 > £ tree 9 (happ) !  $&* dogs @ #~curly~ _under[inside],;stop:'top' full.")]
        [InlineData("test 50% - 10% = 40%", "test 50% - 10%  40%")]
        [InlineData("5 / 2 = 2.5", "5 / 2  2.5")]
        [InlineData(@"TEST 5 \ 2 = 2.5 ?", "TEST 5  2  2.5 ?")]
        [InlineData(@"0123456789", "0123456789")]
        public void Cleaned_Matches(string input, string expected)
        {
            Assert.Equal(expected, StringHelper.CleanInput(input));
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("space test", "space-test")]
        [InlineData("double  space test", "double-space-test")]
        [InlineData("multi--hyphen---test", "multi-hyphen-test")]
        [InlineData("not abc test $", "not-abc-test-")]
        public void UrlString_Matches(string input, string expected)
        {
            Assert.Equal(expected, StringHelper.UrlString(input));
        }


        [Theory]
        [InlineData("eyJzdWIiOiIxMjM0NTY3ODkwIn0", "{\"sub\":\"1234567890\"}")]
        [InlineData("eyJzdWIiOiIxMjM0NTY3ODkwfiN-In0", "{\"sub\":\"1234567890~#~\"}")]
        public void Base64UrlDecode_Valid(string input, string expected)
        {
            var actual = StringHelper.Base64UrlDecode(input);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("blaaa-hash", "{\"sub\":\"12345678903\"}")]
        public void Base64UrlDecode_Invalid(string input, string expected)
        {
            var actual = StringHelper.Base64UrlDecode(input);
            Assert.NotEqual(expected, actual);
        }

    }

}
