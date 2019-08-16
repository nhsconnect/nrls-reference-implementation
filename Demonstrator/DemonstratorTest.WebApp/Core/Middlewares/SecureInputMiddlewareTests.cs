using Demonstrator.Core.Exceptions;
using Demonstrator.Core.Interfaces.Helpers;
using Demonstrator.Core.Resources;
using Demonstrator.Models.Core.Models;
using Demonstrator.WebApp.Core.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DemonstratorTest.WebApp.Core.Middlewares
{
    public class SecureInputMiddlewareTests : IDisposable
    {
        private IJwtHelper _jwtHelper;

        public SecureInputMiddlewareTests()
        {
            var jwtHelper = new Mock<IJwtHelper>();
            jwtHelper.Setup(x => x.IsValidUser(It.IsAny<string>())).Returns(new Response(true));
            jwtHelper.Setup(x => x.IsValidUser(It.Is<string>(j => j == "invalid-jwt"))).Returns(new Response());

            _jwtHelper = jwtHelper.Object;
        }

        public void Dispose()
        {
            _jwtHelper = null;
        }

        [Fact]
        public async void Valid_Headers()
        {
            var requestMock = new Mock<HttpRequest>();
            requestMock.Setup(x => x.Method).Returns("GET");
            requestMock.Setup(x => x.Path).Returns(new PathString("/nrls-ri/SSP/https%3A%2F%2Ftest999.domain.com%2Fbinary%2Ffile%2F0001"));
            requestMock.Setup(x => x.Headers).Returns(new HeaderDictionary()
            {
                { HeaderNames.Authorization, "we-are-not-validating-jwt-here" },
                { FhirConstants.HeaderSspFrom, "200000000116" },
                { FhirConstants.HeaderSspTraceId, "c0fbfe499a48485492299a4c14d78118" }
            });


            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(x => x.Request).Returns(requestMock.Object);

            var secureInputMiddleware = new SecureInputMiddleware(next: (innerHttpContext) => Task.FromResult(0), jwtHelper: _jwtHelper);

            //Test will fail if invalid
            await secureInputMiddleware.Invoke(contextMock.Object);
        }

        [Fact]
        public void Invalid_Header_Authorization_Missing()
        {
            var requestMock = new Mock<HttpRequest>();
            requestMock.Setup(x => x.Method).Returns("GET");
            requestMock.Setup(x => x.Headers).Returns(new HeaderDictionary() { });

            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(x => x.Request).Returns(requestMock.Object);

            var secureInputMiddleware = new SecureInputMiddleware(next: (innerHttpContext) => Task.FromResult(0), jwtHelper: _jwtHelper);

            //Test will fail if invalid
            Assert.ThrowsAsync<HttpFhirException>(async delegate
            {
                await secureInputMiddleware.Invoke(contextMock.Object);
            });
        }

        [Fact]
        public void Invalid_Header_Authorization_Invalid()
        {
            var requestMock = new Mock<HttpRequest>();
            requestMock.Setup(x => x.Method).Returns("GET");
            requestMock.Setup(x => x.Headers).Returns(new HeaderDictionary()
            {
                { HeaderNames.Authorization, "invalid-jwt" }
            });

            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(x => x.Request).Returns(requestMock.Object);

            var secureInputMiddleware = new SecureInputMiddleware(next: (innerHttpContext) => Task.FromResult(0), jwtHelper: _jwtHelper);

            //Test will fail if invalid
            Assert.ThrowsAsync<HttpFhirException>(async delegate
            {
                await secureInputMiddleware.Invoke(contextMock.Object);
            });
        }

        [Fact]
        public void Invalid_Header_SspFrom_Missing()
        {
            var requestMock = new Mock<HttpRequest>();
            requestMock.Setup(x => x.Method).Returns("GET");
            requestMock.Setup(x => x.Headers).Returns(new HeaderDictionary()
            {
                { HeaderNames.Authorization, "we-are-not-validating-jwt-here" },
                { FhirConstants.HeaderSspTraceId, "c0fbfe499a48485492299a4c14d78118" }
            });

            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(x => x.Request).Returns(requestMock.Object);

            var secureInputMiddleware = new SecureInputMiddleware(next: (innerHttpContext) => Task.FromResult(0), jwtHelper: _jwtHelper);

            //Test will fail if invalid
            Assert.ThrowsAsync<HttpFhirException>(async delegate
            {
                await secureInputMiddleware.Invoke(contextMock.Object);
            });
        }

        [Fact]
        public void Invalid_Header_SspTraceId_Missing()
        {
            var requestMock = new Mock<HttpRequest>();
            requestMock.Setup(x => x.Method).Returns("GET");
            requestMock.Setup(x => x.Headers).Returns(new HeaderDictionary()
            {
                { HeaderNames.Authorization, "we-are-not-validating-jwt-here" },
                { FhirConstants.HeaderSspFrom, "200000000116" }
            });

            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(x => x.Request).Returns(requestMock.Object);

            var secureInputMiddleware = new SecureInputMiddleware(next: (innerHttpContext) => Task.FromResult(0), jwtHelper: _jwtHelper);

            //Test will fail if invalid
            Assert.ThrowsAsync<HttpFhirException>(async delegate
            {
                await secureInputMiddleware.Invoke(contextMock.Object);
            });
        }
    }
}
