using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Moq;
using NRLS_API.Core.Enums;
using NRLS_API.Core.Exceptions;
using NRLS_API.Core.Interfaces.Services;
using NRLS_API.Core.Resources;
using NRLS_API.Models.Core;
using NRLS_API.Models.ViewModels.Core;
using NRLS_API.WebApp.Core.Middlewares;
using NRLS_APITest.Data;
using System;
using System.Threading.Tasks;
using Xunit;

namespace NRLS_APITest.WebApp.Middlewares
{
    public class SspProxyGateMiddlewareTests : IDisposable
    {
        private ISdsService _sdsService;
        private INrlsValidation _nrlsValidation;

        public SspProxyGateMiddlewareTests()
        {
            var nrlsValidationMock = new Mock<INrlsValidation>();
            nrlsValidationMock.Setup(x => x.ValidJwt(It.Is<Tuple<JwtScopes, string>>(q => q.Item1 == JwtScopes.Read), It.IsAny<string>())).Returns(new Response(true));
            nrlsValidationMock.Setup(x => x.ValidJwt(It.Is<Tuple<JwtScopes, string>>(q => q.Item1 == JwtScopes.Read), It.Is<string>(j => j == "invalid-jwt"))).Returns(new Response());

            var sdsMock = new Mock<ISdsService>();
            sdsMock.Setup(op => op.GetFor(It.IsAny<string>())).Returns((SdsViewModel)null);
            sdsMock.Setup(op => op.GetFor(It.Is<string>(x => x == "000"))).Returns(SdsViewModels.SdsAsid000);
            sdsMock.Setup(op => op.GetFor(It.Is<string>(x => x == "002"))).Returns(SdsViewModels.SdsAsid002);
            sdsMock.Setup(op => op.GetFor(It.Is<string>(x => x == "999"))).Returns(SdsViewModels.SdsAsid999);
            sdsMock.Setup(op => op.GetFor(It.Is<string>(x => x == SdsViewModels.SdsAsid999.OdsCode), It.Is<string>(x => x == FhirConstants.ReadInteractionId))).Returns(SdsViewModels.SdsAsid000);
            sdsMock.Setup(op => op.GetFor(It.Is<string>(x => x == SdsViewModels.SdsAsid888.OdsCode), It.Is<string>(x => x == FhirConstants.ReadInteractionId))).Returns(delegate { return null; });
            sdsMock.Setup(op => op.GetFor(It.Is<string>(x => x == SdsViewModels.SdsAsid888.OdsCode), It.Is<string>(x => x == "valid-interaction"))).Returns(SdsViewModels.SdsAsid888);

            _sdsService = sdsMock.Object;
            _nrlsValidation = nrlsValidationMock.Object;
        }

        public void Dispose()
        {
            _sdsService = null;
            _nrlsValidation = null;
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
                { FhirConstants.HeaderSspFromAsid, "000" },
                { FhirConstants.HeaderSspToAsid, "999" },
                { FhirConstants.HeaderSspInterationId, FhirConstants.ReadInteractionId },
                { FhirConstants.HeaderSspTradeId, Guid.NewGuid().ToString() }
            });


            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(x => x.Request).Returns(requestMock.Object);


            var sspAProxyGateMiddleware = new SspProxyGateMiddleware(next: (innerHttpContext) => Task.FromResult(0), sdsService: _sdsService, nrlsValidation: _nrlsValidation);

            //Test will fail if invalid
            await sspAProxyGateMiddleware.Invoke(contextMock.Object);
        }

        [Fact]
        public void Invalid_Header_Authorization_Missing()
        {
            var requestMock = new Mock<HttpRequest>();
            requestMock.Setup(x => x.Method).Returns("GET");
            requestMock.Setup(x => x.Headers).Returns(new HeaderDictionary(){});

            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(x => x.Request).Returns(requestMock.Object);

            var sspAProxyGateMiddleware = new SspProxyGateMiddleware(next: (innerHttpContext) => Task.FromResult(0), sdsService: _sdsService, nrlsValidation: _nrlsValidation);

            //Test will fail if invalid
            Assert.ThrowsAsync<HttpFhirException>(async delegate
            {
                await sspAProxyGateMiddleware.Invoke(contextMock.Object);
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

            var sspAProxyGateMiddleware = new SspProxyGateMiddleware(next: (innerHttpContext) => Task.FromResult(0), sdsService: _sdsService, nrlsValidation: _nrlsValidation);

            //Test will fail if invalid
            Assert.ThrowsAsync<HttpFhirException>(async delegate
            {
                await sspAProxyGateMiddleware.Invoke(contextMock.Object);
            });
        }

        [Fact]
        public void Invalid_Header_FromASID_Missing()
        {
            var requestMock = new Mock<HttpRequest>();
            requestMock.Setup(x => x.Method).Returns("GET");
            requestMock.Setup(x => x.Headers).Returns(new HeaderDictionary()
            {
                { HeaderNames.Authorization, "valid-jwt" }
            });

            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(x => x.Request).Returns(requestMock.Object);

            var sspAProxyGateMiddleware = new SspProxyGateMiddleware(next: (innerHttpContext) => Task.FromResult(0), sdsService: _sdsService, nrlsValidation: _nrlsValidation);

            //Test will fail if invalid
            Assert.ThrowsAsync<HttpFhirException>(async delegate
            {
                await sspAProxyGateMiddleware.Invoke(contextMock.Object);
            });
        }

        [Fact]
        public void Invalid_Header_FromASID_Invalid()
        {
            var requestMock = new Mock<HttpRequest>();
            requestMock.Setup(x => x.Method).Returns("GET");
            requestMock.Setup(x => x.Headers).Returns(new HeaderDictionary()
            {
                { HeaderNames.Authorization, "valid-jwt" },
                { FhirConstants.HeaderSspFromAsid, "001" }
            });

            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(x => x.Request).Returns(requestMock.Object);

            var sspAProxyGateMiddleware = new SspProxyGateMiddleware(next: (innerHttpContext) => Task.FromResult(0), sdsService: _sdsService, nrlsValidation: _nrlsValidation);

            //Test will fail if invalid
            Assert.ThrowsAsync<HttpFhirException>(async delegate
            {
                await sspAProxyGateMiddleware.Invoke(contextMock.Object);
            });
        }

        [Fact]
        public void Invalid_Header_ToASID_Missing()
        {
            var requestMock = new Mock<HttpRequest>();
            requestMock.Setup(x => x.Method).Returns("GET");
            requestMock.Setup(x => x.Headers).Returns(new HeaderDictionary()
            {
                { HeaderNames.Authorization, "valid-jwt" },
                { FhirConstants.HeaderSspFromAsid, "000" }
            });

            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(x => x.Request).Returns(requestMock.Object);

            var sspAProxyGateMiddleware = new SspProxyGateMiddleware(next: (innerHttpContext) => Task.FromResult(0), sdsService: _sdsService, nrlsValidation: _nrlsValidation);

            //Test will fail if invalid
            Assert.ThrowsAsync<HttpFhirException>(async delegate
            {
                await sspAProxyGateMiddleware.Invoke(contextMock.Object);
            });
        }

        [Fact]
        public void Invalid_Header_ToASID_Invalid()
        {
            var requestMock = new Mock<HttpRequest>();
            requestMock.Setup(x => x.Method).Returns("GET");
            requestMock.Setup(x => x.Headers).Returns(new HeaderDictionary()
            {
                { HeaderNames.Authorization, "valid-jwt" },
                { FhirConstants.HeaderSspFromAsid, "000" },
                { FhirConstants.HeaderSspToAsid, "990" }
            });

            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(x => x.Request).Returns(requestMock.Object);

            var sspAProxyGateMiddleware = new SspProxyGateMiddleware(next: (innerHttpContext) => Task.FromResult(0), sdsService: _sdsService, nrlsValidation: _nrlsValidation);

            //Test will fail if invalid
            Assert.ThrowsAsync<HttpFhirException>(async delegate
            {
                await sspAProxyGateMiddleware.Invoke(contextMock.Object);
            });
        }

        [Fact]
        public void Invalid_Header_TraceID_Mssing()
        {
            var requestMock = new Mock<HttpRequest>();
            requestMock.Setup(x => x.Method).Returns("GET");
            requestMock.Setup(x => x.Headers).Returns(new HeaderDictionary()
            {
                { HeaderNames.Authorization, "valid-jwt" },
                { FhirConstants.HeaderSspFromAsid, "000" },
                { FhirConstants.HeaderSspToAsid, "999" }
            });

            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(x => x.Request).Returns(requestMock.Object);

            var sspAProxyGateMiddleware = new SspProxyGateMiddleware(next: (innerHttpContext) => Task.FromResult(0), sdsService: _sdsService, nrlsValidation: _nrlsValidation);

            //Test will fail if invalid
            Assert.ThrowsAsync<HttpFhirException>(async delegate
            {
                await sspAProxyGateMiddleware.Invoke(contextMock.Object);
            });
        }

        [Fact]
        public void Invalid_Header_TraceID_Invalid()
        {
            var requestMock = new Mock<HttpRequest>();
            requestMock.Setup(x => x.Method).Returns("GET");
            requestMock.Setup(x => x.Headers).Returns(new HeaderDictionary()
            {
                { HeaderNames.Authorization, "valid-jwt" },
                { FhirConstants.HeaderSspFromAsid, "000" },
                { FhirConstants.HeaderSspToAsid, "999" },
                { FhirConstants.HeaderSspTradeId, "" }
            });

            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(x => x.Request).Returns(requestMock.Object);

            var sspAProxyGateMiddleware = new SspProxyGateMiddleware(next: (innerHttpContext) => Task.FromResult(0), sdsService: _sdsService, nrlsValidation: _nrlsValidation);

            //Test will fail if invalid
            Assert.ThrowsAsync<HttpFhirException>(async delegate
            {
                await sspAProxyGateMiddleware.Invoke(contextMock.Object);
            });
        }

        [Fact]
        public void Invalid_Header_InteractionID_Missing()
        {
            var requestMock = new Mock<HttpRequest>();
            requestMock.Setup(x => x.Method).Returns("GET");
            requestMock.Setup(x => x.Headers).Returns(new HeaderDictionary()
            {
                { HeaderNames.Authorization, "valid-jwt" },
                { FhirConstants.HeaderSspFromAsid, "000" },
                { FhirConstants.HeaderSspToAsid, "999" },
                { FhirConstants.HeaderSspTradeId, "valid-trace" }
            });

            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(x => x.Request).Returns(requestMock.Object);

            var sspAProxyGateMiddleware = new SspProxyGateMiddleware(next: (innerHttpContext) => Task.FromResult(0), sdsService: _sdsService, nrlsValidation: _nrlsValidation);

            //Test will fail if invalid
            Assert.ThrowsAsync<HttpFhirException>(async delegate
            {
                await sspAProxyGateMiddleware.Invoke(contextMock.Object);
            });
        }

        [Fact]
        public void Invalid_Header_InteractionID_Invalid()
        {
            var requestMock = new Mock<HttpRequest>();
            requestMock.Setup(x => x.Method).Returns("GET");
            requestMock.Setup(x => x.Headers).Returns(new HeaderDictionary()
            {
                { HeaderNames.Authorization, "valid-jwt" },
                { FhirConstants.HeaderSspFromAsid, "000" },
                { FhirConstants.HeaderSspToAsid, "999" },
                { FhirConstants.HeaderSspTradeId, "valid-trace" },
                { FhirConstants.HeaderSspInterationId, "invalid-interaction" }
            });

            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(x => x.Request).Returns(requestMock.Object);

            var sspAProxyGateMiddleware = new SspProxyGateMiddleware(next: (innerHttpContext) => Task.FromResult(0), sdsService: _sdsService, nrlsValidation: _nrlsValidation);

            //Test will fail if invalid
            Assert.ThrowsAsync<HttpFhirException>(async delegate
            {
                await sspAProxyGateMiddleware.Invoke(contextMock.Object);
            });
        }

        [Fact]
        public void Invalid_Header_Provider_Invalid()
        {
            var requestMock = new Mock<HttpRequest>();
            requestMock.Setup(x => x.Method).Returns("GET");
            requestMock.Setup(x => x.Headers).Returns(new HeaderDictionary()
            {
                { HeaderNames.Authorization, "valid-jwt" },
                { FhirConstants.HeaderSspFromAsid, "000" },
                { FhirConstants.HeaderSspToAsid, "888" },
                { FhirConstants.HeaderSspTradeId, "valid-trace" },
                { FhirConstants.HeaderSspInterationId, FhirConstants.ReadInteractionId }
            });

            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(x => x.Request).Returns(requestMock.Object);

            var sspAProxyGateMiddleware = new SspProxyGateMiddleware(next: (innerHttpContext) => Task.FromResult(0), sdsService: _sdsService, nrlsValidation: _nrlsValidation);

            //Test will fail if invalid
            Assert.ThrowsAsync<HttpFhirException>(async delegate
            {
                await sspAProxyGateMiddleware.Invoke(contextMock.Object);
            });
        }


        [Fact]
        public void Invalid_Header_Provider_FQDN_Invalid()
        {
            var requestMock = new Mock<HttpRequest>();
            requestMock.Setup(x => x.Method).Returns("GET");
            requestMock.Setup(x => x.Path).Returns(new PathString("/nrls-ri/SSP/https%3A%2F%2Ftest888-invalid.domain.com%2Fbinary%2Ffile%2F0001"));
            requestMock.Setup(x => x.Headers).Returns(new HeaderDictionary()
            {
                { HeaderNames.Authorization, "valid-jwt" },
                { FhirConstants.HeaderSspFromAsid, "000" },
                { FhirConstants.HeaderSspToAsid, "888" },
                { FhirConstants.HeaderSspTradeId, "valid-trace" },
                { FhirConstants.HeaderSspInterationId, "valid-interaction" }
            });

            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(x => x.Request).Returns(requestMock.Object);

            var sspAProxyGateMiddleware = new SspProxyGateMiddleware(next: (innerHttpContext) => Task.FromResult(0), sdsService: _sdsService, nrlsValidation: _nrlsValidation);

            //Test will fail if invalid
            Assert.ThrowsAsync<HttpFhirException>(async delegate
            {
                await sspAProxyGateMiddleware.Invoke(contextMock.Object);
            });
        }

    }
}
