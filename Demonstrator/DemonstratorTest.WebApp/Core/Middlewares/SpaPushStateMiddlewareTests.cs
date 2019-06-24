using Demonstrator.WebApp.Core.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System.Threading.Tasks;
using Xunit;

namespace DemonstratorTest.WebApp.Core.Middlewares
{
    public class SpaPushStateMiddlewareTests
    {
        [Fact]
        public async void Valid_Is404()
        {

            var contextMock = new DefaultHttpContext();
            contextMock.Features.Get<IHttpRequestFeature>().Path = "/test";
            contextMock.Features.Get<IHttpResponseFeature>().StatusCode = 404;

            var middleware = new SpaPushStateMiddleware(next: (innerHttpContext) => Task.FromResult(0));

            await middleware.Invoke(contextMock);

            Assert.Equal(new PathString("/index.html"), contextMock.Request.Path);
        }

        [Fact]
        public async void Valid_Is200()
        {

            var contextMock = new DefaultHttpContext();
            contextMock.Features.Get<IHttpRequestFeature>().Path = "/test";
            contextMock.Features.Get<IHttpResponseFeature>().StatusCode = 200;

            var middleware = new SpaPushStateMiddleware(next: (innerHttpContext) => Task.FromResult(0));

            await middleware.Invoke(contextMock);

            Assert.Equal(new PathString("/test"), contextMock.Request.Path);
        }

    }
}
