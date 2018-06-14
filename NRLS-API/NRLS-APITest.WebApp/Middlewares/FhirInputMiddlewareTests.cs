﻿using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Hl7.Fhir.Rest;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
using NRLS_API.Core.Exceptions;
using NRLS_API.Models.Core;
using NRLS_API.WebApp.Core.Middlewares;
using NRLS_APITest.Data;
using Xunit;

namespace NRLS_APITest.WebApp.Middlewares
{
    public class FhirInputMiddlewareTests
    {

        string _validFormat = "application/fhir%2Bjson";

        [Fact]
        public async void Valid_param_and_header()
        {

            var opts = AppSettings.NrlsApiSettings;

            var settingsMock = new Mock<IOptions<NrlsApiSetting>>();
            settingsMock.Setup(op => op.Value).Returns(opts);

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
                { HttpRequestHeader.Accept.ToString(), $"{_validFormat}; {Encoding.UTF8.WebName}" }
            });


            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(x => x.Request).Returns(requestMock.Object);

            var fhirInputMiddleware = new FhirInputMiddleware(next: (innerHttpContext) => Task.FromResult(0), nrlsApiSettings: settingsMock.Object);

            //Test will fail if invalid
            await fhirInputMiddleware.Invoke(contextMock.Object);
        }

        [Fact]
        public async void Valid_param_and_no_header()
        {

            var opts = AppSettings.NrlsApiSettings;

            var settingsMock = new Mock<IOptions<NrlsApiSetting>>();
            settingsMock.Setup(op => op.Value).Returns(opts);

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

            var fhirInputMiddleware = new FhirInputMiddleware(next: (innerHttpContext) => Task.FromResult(0), nrlsApiSettings: settingsMock.Object);

            //Test will fail if invalid
            await fhirInputMiddleware.Invoke(contextMock.Object);
        }

        [Fact]
        public async void No_param_and_Valid_header()
        {

            var opts = AppSettings.NrlsApiSettings;

            var settingsMock = new Mock<IOptions<NrlsApiSetting>>();
            settingsMock.Setup(op => op.Value).Returns(opts);

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
                { HttpRequestHeader.Accept.ToString(), $"{_validFormat}; {Encoding.UTF8.WebName}" }
            });


            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(x => x.Request).Returns(requestMock.Object);

            var fhirInputMiddleware = new FhirInputMiddleware(next: (innerHttpContext) => Task.FromResult(0), nrlsApiSettings: settingsMock.Object);

            //Test will fail if invalid
            await fhirInputMiddleware.Invoke(contextMock.Object);
        }

        [Fact]
        public async void Valid_param_and_Invalid_header()
        {

            var opts = AppSettings.NrlsApiSettings;

            var settingsMock = new Mock<IOptions<NrlsApiSetting>>();
            settingsMock.Setup(op => op.Value).Returns(opts);

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
                { HttpRequestHeader.Accept.ToString(), $"blaa" }
            });


            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(x => x.Request).Returns(requestMock.Object);

            var fhirInputMiddleware = new FhirInputMiddleware(next: (innerHttpContext) => Task.FromResult(0), nrlsApiSettings: settingsMock.Object);

            await fhirInputMiddleware.Invoke(contextMock.Object);
        }

        [Fact]
        public async void Invalid_param_and_Valid_header()
        {

            var opts = AppSettings.NrlsApiSettings;

            var settingsMock = new Mock<IOptions<NrlsApiSetting>>();
            settingsMock.Setup(op => op.Value).Returns(opts);

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
                { HttpRequestHeader.Accept.ToString(), $"{ContentType.JSON_CONTENT_HEADER}; {Encoding.UTF8.WebName}" }
            });


            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(x => x.Request).Returns(requestMock.Object);

            var fhirInputMiddleware = new FhirInputMiddleware(next: (innerHttpContext) => Task.FromResult(0), nrlsApiSettings: settingsMock.Object);

            await fhirInputMiddleware.Invoke(contextMock.Object);
        }

        [Fact]
        public void Invalid_param_and_header()
        {

            var opts = AppSettings.NrlsApiSettings;

            var settingsMock = new Mock<IOptions<NrlsApiSetting>>();
            settingsMock.Setup(op => op.Value).Returns(opts);

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
                { HttpRequestHeader.Accept.ToString(), "blaa" }
            });


            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(x => x.Request).Returns(requestMock.Object);

            var fhirInputMiddleware = new FhirInputMiddleware(next: (innerHttpContext) => Task.FromResult(0), nrlsApiSettings: settingsMock.Object);


            Assert.ThrowsAsync<HttpFhirException>(async delegate
            {
                await fhirInputMiddleware.Invoke(contextMock.Object);

            });
        }
    }
}