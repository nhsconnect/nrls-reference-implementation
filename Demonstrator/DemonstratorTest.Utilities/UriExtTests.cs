using Demonstrator.Utilities.Extensions;
using Xunit;

namespace DemonstratorTest.Utilities
{
    public class UriExtTests
    {
        [Fact]
        public void Uri_From_QueryString_Valid_AllEmpty()
        {
            var test = "";

            var parsedUri = test.ParseQuery();

            Assert.Equal(0, parsedUri.Count);

        }

        [Fact]
        public void Uri_From_QueryString_Valid_OneEmpty()
        {
            var test = "test";

            var parsedUri = test.ParseQuery();

            Assert.True(parsedUri.ContainsKey("test"));
            Assert.Null(parsedUri["test"]);

        }

        [Fact]
        public void Uri_From_QueryString_Valid_OneComplete()
        {
            var test = "test=true";

            var parsedUri = test.ParseQuery();

            Assert.True(parsedUri.ContainsKey("test"));
            Assert.Equal("true", parsedUri["test"]);
        }

        [Fact]
        public void Uri_From_QueryString_Valid_OneComplete_OneEmpty()
        {
            var test = "test=false&anothertest";

            var parsedUri = test.ParseQuery();

            Assert.True(parsedUri.ContainsKey("test"));
            Assert.Equal("false", parsedUri["test"]);

            Assert.True(parsedUri.ContainsKey("anothertest"));
            Assert.Null(parsedUri["anothertest"]);
        }

        [Fact]
        public void Uri_From_QueryString_Valid_AllEmpty_OnePartial()
        {
            var test = "&another";

            var parsedUri = test.ParseQuery();

            Assert.True(parsedUri.ContainsKey("another"));
            Assert.Null(parsedUri["another"]);
        }

        [Fact]
        public void Uri_From_QueryString_Valid_TwoComplete()
        {
            var test = "test=true&anothertest=false";

            var parsedUri = test.ParseQuery();

            Assert.True(parsedUri.ContainsKey("test"));
            Assert.Equal("true", parsedUri["test"]);

            Assert.True(parsedUri.ContainsKey("anothertest"));
            Assert.Equal("false", parsedUri["anothertest"]);
        }

        [Fact]
        public void Uri_From_QueryString_Valid_OnePartial()
        {
            var test = "&=false";

            var parsedUri = test.ParseQuery();

            Assert.Equal(0, parsedUri.Count);
        }
    }
}
