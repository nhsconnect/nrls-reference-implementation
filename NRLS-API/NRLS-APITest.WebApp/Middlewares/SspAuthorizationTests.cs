using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using NRLS_API.Core.Enums;
using NRLS_API.Core.Exceptions;
using NRLS_API.Core.Interfaces.Services;
using NRLS_API.Core.Resources;
using NRLS_API.Models.Core;
using NRLS_API.WebApp.Core.Middlewares;
using NRLS_APITest.Data;
using NRLS_APITest.StubClasses;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace NRLS_APITest.WebApp.Middlewares
{
    public class SspAuthorizationTests : IDisposable
    {
        private IOptions<SpineSetting> _spineSettings;
        private IOptionsSnapshot<NrlsApiSetting> _nrlsSettings;
        private IMemoryCache _cache;
        private readonly INrlsValidation _nrlsValidation;

        public SspAuthorizationTests()
        {
            var spineSettings = new SpineSetting
            {
                Asid = "999"
            };

            var spineSettingsMock = new Mock<IOptions<SpineSetting>>();
            spineSettingsMock.Setup(op => op.Value).Returns(spineSettings);

            var nrlsSettingsMock = new Mock<IOptionsSnapshot<NrlsApiSetting>>();
            nrlsSettingsMock.Setup(op => op.Get(It.IsAny<string>())).Returns(AppSettings.NrlsApiSettings);


            var nrlsValidationMock = new Mock<INrlsValidation>();
            nrlsValidationMock.Setup(x => x.ValidJwt(It.Is<JwtScopes>(q => q == JwtScopes.Read), It.IsAny<string>())).Returns(new Response(true));
            nrlsValidationMock.Setup(x => x.ValidJwt(It.Is<JwtScopes>(q => q == JwtScopes.Write), It.IsAny<string>())).Returns(new Response());

            var clientMapCache = new ClientAsidMap
            {
                ClientAsids = new Dictionary<string, ClientAsid>()
                {
                    { "000", new ClientAsid { Interactions = new List<string>{ "urn:nhs:names:services:nrls:fhir:rest:read:documentreference" }, OrgCode = "TestOrgCode", Thumbprint = "TestThumbprint" } },
                    { "002", new ClientAsid { Interactions = new List<string>(), OrgCode = "TestOrgCode2", Thumbprint = "TestThumbprint" } }

                }
            };

            _cache = MemoryCacheStub.MockMemoryCacheService.GetMemoryCache(clientMapCache);

            _spineSettings = spineSettingsMock.Object;
            _nrlsSettings = nrlsSettingsMock.Object;
            _nrlsValidation = nrlsValidationMock.Object;
        }

        public void Dispose()
        {
            _spineSettings = null;
            _nrlsSettings = null;
            _cache = null;
        }

        [Fact]
        public async void Valid_Headers()
        {
            var requestMock = new Mock<HttpRequest>();
            requestMock.Setup(x => x.Method).Returns("GET");
            requestMock.Setup(x => x.Headers).Returns(new HeaderDictionary()
            {
                { HttpRequestHeader.Authorization.ToString(), "we-are-not-validating-jwt-here" },
                { FhirConstants.HeaderFromAsid, "000" },
                { FhirConstants.HeaderToAsid, "999" },
                { FhirConstants.HeaderSspInterationId, FhirConstants.ReadInteractionId }
            });


            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(x => x.Request).Returns(requestMock.Object);


            var sspAuthorizationMiddleware = new SspAuthorizationMiddleware(next: (innerHttpContext) => Task.FromResult(0), spineSettings: _spineSettings, memoryCache: _cache, nrlsValidation: _nrlsValidation);

            //Test will fail if invalid
            await sspAuthorizationMiddleware.Invoke(contextMock.Object, _nrlsSettings);
        }

        [Fact]
        public void Invalid_Header_Authorization_Missing()
        {
            var requestMock = new Mock<HttpRequest>();
            requestMock.Setup(x => x.Method).Returns("GET");
            requestMock.Setup(x => x.Headers).Returns(new HeaderDictionary() //Removed Authorization Header 
            {
                { FhirConstants.HeaderFromAsid, "000" },
                { FhirConstants.HeaderToAsid, "999" },
                { FhirConstants.HeaderSspInterationId, FhirConstants.ReadInteractionId }
            });

            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(x => x.Request).Returns(requestMock.Object);

            var sspAuthorizationMiddleware = new SspAuthorizationMiddleware(next: (innerHttpContext) => Task.FromResult(0), spineSettings: _spineSettings, memoryCache: _cache, nrlsValidation: _nrlsValidation);

            Assert.ThrowsAsync<HttpFhirException>(async delegate
            {
                await sspAuthorizationMiddleware.Invoke(contextMock.Object, _nrlsSettings);

            });
        }

        [Fact]
        public void Invalid_Header_Authorization_Invalid()
        {
            var requestMock = new Mock<HttpRequest>();
            requestMock.Setup(x => x.Method).Returns("POST");  //Using post returns false from jwt check
            requestMock.Setup(x => x.Headers).Returns(new HeaderDictionary()
            {
                { HttpRequestHeader.Authorization.ToString(), "we-are-not-validating-jwt-here" },
                { FhirConstants.HeaderFromAsid, "000" },
                { FhirConstants.HeaderToAsid, "999" },
                { FhirConstants.HeaderSspInterationId, FhirConstants.CreateInteractionId }
            });

            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(x => x.Request).Returns(requestMock.Object);

            var sspAuthorizationMiddleware = new SspAuthorizationMiddleware(next: (innerHttpContext) => Task.FromResult(0), spineSettings: _spineSettings, memoryCache: _cache, nrlsValidation: _nrlsValidation);

            Assert.ThrowsAsync<HttpFhirException>(async delegate
            {
                await sspAuthorizationMiddleware.Invoke(contextMock.Object, _nrlsSettings);

            });
        }

        [Fact]
        public void Invalid_Header_FromASID_Missing()
        {
            var requestMock = new Mock<HttpRequest>();
            requestMock.Setup(x => x.Method).Returns("GET");
            requestMock.Setup(x => x.Headers).Returns(new HeaderDictionary() //Removed fromASID Header 
            {
                { HttpRequestHeader.Authorization.ToString(), "we-are-not-validating-jwt-here" },
                { FhirConstants.HeaderToAsid, "999" },
                { FhirConstants.HeaderSspInterationId, FhirConstants.ReadInteractionId }
            });

            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(x => x.Request).Returns(requestMock.Object);

            var sspAuthorizationMiddleware = new SspAuthorizationMiddleware(next: (innerHttpContext) => Task.FromResult(0), spineSettings: _spineSettings, memoryCache: _cache, nrlsValidation: _nrlsValidation);

            Assert.ThrowsAsync<HttpFhirException>(async delegate
            {
                await sspAuthorizationMiddleware.Invoke(contextMock.Object, _nrlsSettings);

            });
        }

        [Fact]
        public void Invalid_Header_FromASID_Invalid()
        {
            var requestMock = new Mock<HttpRequest>();
            requestMock.Setup(x => x.Method).Returns("GET");
            requestMock.Setup(x => x.Headers).Returns(new HeaderDictionary() //Using an invalid value for fromASID header
            {
                { HttpRequestHeader.Authorization.ToString(), "we-are-not-validating-jwt-here" },
                { FhirConstants.HeaderFromAsid, "001" },
                { FhirConstants.HeaderToAsid, "999" },
                { FhirConstants.HeaderSspInterationId, FhirConstants.ReadInteractionId }
            });

            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(x => x.Request).Returns(requestMock.Object);

            var sspAuthorizationMiddleware = new SspAuthorizationMiddleware(next: (innerHttpContext) => Task.FromResult(0), spineSettings: _spineSettings, memoryCache: _cache, nrlsValidation: _nrlsValidation);

            Assert.ThrowsAsync<HttpFhirException>(async delegate
            {
                await sspAuthorizationMiddleware.Invoke(contextMock.Object, _nrlsSettings);

            });
        }

        [Fact]
        public void Invalid_Header_ToASID_Missing()
        {
            var requestMock = new Mock<HttpRequest>();
            requestMock.Setup(x => x.Method).Returns("GET");
            requestMock.Setup(x => x.Headers).Returns(new HeaderDictionary() //Removed toASID Header 
            {
                { HttpRequestHeader.Authorization.ToString(), "we-are-not-validating-jwt-here" },
                { FhirConstants.HeaderFromAsid, "000" },
                { FhirConstants.HeaderSspInterationId, FhirConstants.ReadInteractionId }
            });

            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(x => x.Request).Returns(requestMock.Object);

            var sspAuthorizationMiddleware = new SspAuthorizationMiddleware(next: (innerHttpContext) => Task.FromResult(0), spineSettings: _spineSettings, memoryCache: _cache, nrlsValidation: _nrlsValidation);

            Assert.ThrowsAsync<HttpFhirException>(async delegate
            {
                await sspAuthorizationMiddleware.Invoke(contextMock.Object, _nrlsSettings);

            });
        }

        [Fact]
        public void Invalid_Header_ToASID_Invalid()
        {
            var requestMock = new Mock<HttpRequest>();
            requestMock.Setup(x => x.Method).Returns("GET");
            requestMock.Setup(x => x.Headers).Returns(new HeaderDictionary() //Used invalid value for toASID Header 
            {
                { HttpRequestHeader.Authorization.ToString(), "we-are-not-validating-jwt-here" },
                { FhirConstants.HeaderFromAsid, "000" },
                { FhirConstants.HeaderToAsid, "888" },
                { FhirConstants.HeaderSspInterationId, FhirConstants.ReadInteractionId }
            });

            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(x => x.Request).Returns(requestMock.Object);

            var sspAuthorizationMiddleware = new SspAuthorizationMiddleware(next: (innerHttpContext) => Task.FromResult(0), spineSettings: _spineSettings, memoryCache: _cache, nrlsValidation: _nrlsValidation);

            Assert.ThrowsAsync<HttpFhirException>(async delegate
            {
                await sspAuthorizationMiddleware.Invoke(contextMock.Object, _nrlsSettings);

            });
        }

        [Fact]
        public void Invalid_Header_SspInterationId_Missing()
        {
            var requestMock = new Mock<HttpRequest>();
            requestMock.Setup(x => x.Method).Returns("GET");
            requestMock.Setup(x => x.Headers).Returns(new HeaderDictionary() //Removed SspInterationId Header 
            {
                { HttpRequestHeader.Authorization.ToString(), "we-are-not-validating-jwt-here" },
                { FhirConstants.HeaderFromAsid, "000" },
                { FhirConstants.HeaderToAsid, "999" }
            });

            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(x => x.Request).Returns(requestMock.Object);

            var sspAuthorizationMiddleware = new SspAuthorizationMiddleware(next: (innerHttpContext) => Task.FromResult(0), spineSettings: _spineSettings, memoryCache: _cache, nrlsValidation: _nrlsValidation);

            Assert.ThrowsAsync<HttpFhirException>(async delegate
            {
                await sspAuthorizationMiddleware.Invoke(contextMock.Object, _nrlsSettings);

            });
        }

        [Fact]
        public void Invalid_Header_SspInterationId_Invalid()
        {
            var requestMock = new Mock<HttpRequest>();
            requestMock.Setup(x => x.Method).Returns("GET");
            requestMock.Setup(x => x.Headers).Returns(new HeaderDictionary() // Used invalid value for the SspInterationId header
            {
                { HttpRequestHeader.Authorization.ToString(), "we-are-not-validating-jwt-here" },
                { FhirConstants.HeaderFromAsid, "000" },
                { FhirConstants.HeaderToAsid, "999" },
                { FhirConstants.HeaderSspInterationId, FhirConstants.SearchInteractionId }
            });

            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(x => x.Request).Returns(requestMock.Object);

            var sspAuthorizationMiddleware = new SspAuthorizationMiddleware(next: (innerHttpContext) => Task.FromResult(0), spineSettings: _spineSettings, memoryCache: _cache, nrlsValidation: _nrlsValidation);

            Assert.ThrowsAsync<HttpFhirException>(async delegate
            {
                await sspAuthorizationMiddleware.Invoke(contextMock.Object, _nrlsSettings);

            });
        }
    }
}
