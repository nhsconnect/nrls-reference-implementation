using Moq;
using Moq.Protected;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DemonstratorTest.StubClasses
{
    public static class HttpMessageStub
    {
        public static Mock<HttpClientHandler> GetClientHandler(HttpResponseMessage responseMessage)
        {
            var mockMessageHandler = new Mock<HttpClientHandler>();
            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(responseMessage);

            return mockMessageHandler;
        }
    }
}
