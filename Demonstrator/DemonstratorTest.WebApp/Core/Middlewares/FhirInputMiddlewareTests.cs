using Demonstrator.Core.Exceptions;
using Demonstrator.Models.Core.Models;
using Demonstrator.WebApp.Core.Middlewares;
using Hl7.Fhir.Rest;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DemonstratorTest.WebApp.Core.Middlewares
{
    public class FhirInputMiddlewareTests : IDisposable
    {
        IOptionsSnapshot<ApiSetting> _apiSettings;
        string _validFormat = "application/fhir%2Bjson";

        public FhirInputMiddlewareTests()
        {
            var opts = new ApiSetting
            {
                BaseUrl = "://localhost.com",
                Secure = false,
                SecureOnly = false,
                DefaultPort = "51913",
                SecurePort = "44313",
                SupportedContentTypes = new List<string> { "application/fhir+json" }
            };

            var settingsMock = new Mock<IOptionsSnapshot<ApiSetting>>();
            settingsMock.Setup(op => op.Value).Returns(opts);

            _apiSettings = settingsMock.Object;
        }

        public void Dispose()
        {
            _apiSettings = null;
        }


        [Fact]
        public async void Valid_no_param_and_no_header()
        {

            var requestMock = new Mock<HttpRequest>();
            requestMock.Setup(x => x.Scheme).Returns("http");
            requestMock.Setup(x => x.Host).Returns(new HostString("localhost"));
            requestMock.Setup(x => x.Path).Returns(new PathString("/test"));
            requestMock.Setup(x => x.PathBase).Returns(new PathString("/"));
            requestMock.Setup(x => x.Method).Returns("GET");
            requestMock.Setup(x => x.Body).Returns(new MemoryStream());
            requestMock.Setup(x => x.QueryString).Returns(new QueryString(""));
            requestMock.Setup(x => x.Headers).Returns(new HeaderDictionary() { });


            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(x => x.Request).Returns(requestMock.Object);

            var fhirInputMiddleware = new FhirInputMiddleware(next: (innerHttpContext) => Task.FromResult(0));

            //Test will fail if invalid
            await fhirInputMiddleware.Invoke(contextMock.Object, _apiSettings);
        }

        [Fact]
        public async void Valid_param_and_header()
        {

            var requestMock = new Mock<HttpRequest>();
            requestMock.Setup(x => x.Scheme).Returns("http");
            requestMock.Setup(x => x.Host).Returns(new HostString("localhost"));
            requestMock.Setup(x => x.Path).Returns(new PathString("/test"));
            requestMock.Setup(x => x.PathBase).Returns(new PathString("/"));
            requestMock.Setup(x => x.Method).Returns("GET");
            requestMock.Setup(x => x.Body).Returns(new MemoryStream());
            requestMock.Setup(x => x.QueryString).Returns(new QueryString($"?_format={_validFormat}"));
            requestMock.Setup(x => x.Headers).Returns(new HeaderDictionary()
            {
                { HeaderNames.Accept, $"{_validFormat}; {Encoding.UTF8.WebName}" }
            });


            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(x => x.Request).Returns(requestMock.Object);

            var fhirInputMiddleware = new FhirInputMiddleware(next: (innerHttpContext) => Task.FromResult(0));

            //Test will fail if invalid
            await fhirInputMiddleware.Invoke(contextMock.Object, _apiSettings);
        }

        [Fact]
        public async void Valid_param_and_no_header()
        {

            var requestMock = new Mock<HttpRequest>();
            requestMock.Setup(x => x.Scheme).Returns("http");
            requestMock.Setup(x => x.Host).Returns(new HostString("localhost"));
            requestMock.Setup(x => x.Path).Returns(new PathString("/test"));
            requestMock.Setup(x => x.PathBase).Returns(new PathString("/"));
            requestMock.Setup(x => x.Method).Returns("GET");
            requestMock.Setup(x => x.Body).Returns(new MemoryStream());
            requestMock.Setup(x => x.QueryString).Returns(new QueryString($"?_format={_validFormat}"));
            requestMock.Setup(x => x.Headers).Returns(new HeaderDictionary() { });


            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(x => x.Request).Returns(requestMock.Object);

            var fhirInputMiddleware = new FhirInputMiddleware(next: (innerHttpContext) => Task.FromResult(0));

            //Test will fail if invalid
            await fhirInputMiddleware.Invoke(contextMock.Object, _apiSettings);
        }

        [Fact]
        public async void No_param_and_Valid_header()
        {

            var requestMock = new Mock<HttpRequest>();
            requestMock.Setup(x => x.Scheme).Returns("http");
            requestMock.Setup(x => x.Host).Returns(new HostString("localhost"));
            requestMock.Setup(x => x.Path).Returns(new PathString("/test"));
            requestMock.Setup(x => x.PathBase).Returns(new PathString("/"));
            requestMock.Setup(x => x.Method).Returns("GET");
            requestMock.Setup(x => x.Body).Returns(new MemoryStream());
            requestMock.Setup(x => x.QueryString).Returns(new QueryString(""));
            requestMock.Setup(x => x.Headers).Returns(new HeaderDictionary()
            {
                { HeaderNames.Accept, $"{_validFormat}; {Encoding.UTF8.WebName}" }
            });


            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(x => x.Request).Returns(requestMock.Object);

            var fhirInputMiddleware = new FhirInputMiddleware(next: (innerHttpContext) => Task.FromResult(0));

            //Test will fail if invalid
            await fhirInputMiddleware.Invoke(contextMock.Object, _apiSettings);
        }

        [Fact]
        public async void Valid_param_and_Invalid_header()
        {

            var requestMock = new Mock<HttpRequest>();
            requestMock.Setup(x => x.Scheme).Returns("http");
            requestMock.Setup(x => x.Host).Returns(new HostString("localhost"));
            requestMock.Setup(x => x.Path).Returns(new PathString("/test"));
            requestMock.Setup(x => x.PathBase).Returns(new PathString("/"));
            requestMock.Setup(x => x.Method).Returns("GET");
            requestMock.Setup(x => x.Body).Returns(new MemoryStream());
            requestMock.Setup(x => x.QueryString).Returns(new QueryString($"?_format={_validFormat}"));
            requestMock.Setup(x => x.Headers).Returns(new HeaderDictionary()
            {
                { HeaderNames.Accept, $"blaa" }
            });


            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(x => x.Request).Returns(requestMock.Object);

            var fhirInputMiddleware = new FhirInputMiddleware(next: (innerHttpContext) => Task.FromResult(0));

            await fhirInputMiddleware.Invoke(contextMock.Object, _apiSettings);
        }

        [Fact]
        public void Invalid_param_and_Valid_header()
        {

            var requestMock = new Mock<HttpRequest>();
            requestMock.Setup(x => x.Scheme).Returns("http");
            requestMock.Setup(x => x.Host).Returns(new HostString("localhost"));
            requestMock.Setup(x => x.Path).Returns(new PathString("/test"));
            requestMock.Setup(x => x.PathBase).Returns(new PathString("/"));
            requestMock.Setup(x => x.Method).Returns("GET");
            requestMock.Setup(x => x.Body).Returns(new MemoryStream());
            requestMock.Setup(x => x.QueryString).Returns(new QueryString("?_format=invalidFormat"));
            requestMock.Setup(x => x.Headers).Returns(new HeaderDictionary()
            {
                { HeaderNames.Accept, $"{ContentType.JSON_CONTENT_HEADER}; {Encoding.UTF8.WebName}" }
            });


            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(x => x.Request).Returns(requestMock.Object);

            var fhirInputMiddleware = new FhirInputMiddleware(next: (innerHttpContext) => Task.FromResult(0));

            Assert.ThrowsAsync<HttpFhirException>(async delegate
            {
                await fhirInputMiddleware.Invoke(contextMock.Object, _apiSettings);

            });
        }

        [Fact]
        public void Invalid_param_and_header()
        {

            var requestMock = new Mock<HttpRequest>();
            requestMock.Setup(x => x.Scheme).Returns("http");
            requestMock.Setup(x => x.Host).Returns(new HostString("localhost"));
            requestMock.Setup(x => x.Path).Returns(new PathString("/test"));
            requestMock.Setup(x => x.PathBase).Returns(new PathString("/"));
            requestMock.Setup(x => x.Method).Returns("GET");
            requestMock.Setup(x => x.Body).Returns(new MemoryStream());
            requestMock.Setup(x => x.QueryString).Returns(new QueryString("?_format=invalidFormat"));
            requestMock.Setup(x => x.Headers).Returns(new HeaderDictionary()
            {
                { HeaderNames.Accept, "blaa" }
            });


            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(x => x.Request).Returns(requestMock.Object);

            var fhirInputMiddleware = new FhirInputMiddleware(next: (innerHttpContext) => Task.FromResult(0));


            Assert.ThrowsAsync<HttpFhirException>(async delegate
            {
                await fhirInputMiddleware.Invoke(contextMock.Object, _apiSettings);

            });
        }
    }
}
