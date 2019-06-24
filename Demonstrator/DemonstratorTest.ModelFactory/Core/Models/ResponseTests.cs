using Demonstrator.Models.Core.Models;
using Xunit;

namespace DemonstratorTest.ModelFactory.Core.Models
{
    public class ResponseTests
    {
        [Fact]
        public void Response_Default()
        {
            var response = new Response();


            Assert.False(response.Success);
            Assert.Null(response.Message);
        }

        [Fact]
        public void Response_SuccessOverride()
        {
            var response = new Response(true);


            Assert.True(response.Success);
            Assert.Null(response.Message);
        }

        [Fact]
        public void Response_MessageOverride()
        {
            var response = new Response("hello");


            Assert.False(response.Success);
            Assert.Equal("hello", response.Message);
        }

        [Fact]
        public void Response_SuccessAndMessageOverride()
        {
            var response = new Response(true, "hello");


            Assert.True(response.Success);
            Assert.Equal("hello", response.Message);
        }

    }
}
