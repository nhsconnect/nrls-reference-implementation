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
    public class SpineAuthorizationTests : IDisposable
    {
        private IOptions<SpineSetting> _spineSettings;
        private IOptionsSnapshot<NrlsApiSetting> _nrlsSettings;
        private ISdsService _sdsService;
        private readonly INrlsValidation _nrlsValidation;

        public SpineAuthorizationTests()
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
            nrlsValidationMock.Setup(x => x.ValidJwt(It.Is<Tuple<JwtScopes, string>>(q => q.Item1 == JwtScopes.Read), It.IsAny<string>())).Returns(new Response(true));
            nrlsValidationMock.Setup(x => x.ValidJwt(It.Is<Tuple<JwtScopes, string>>(q => q.Item1 == JwtScopes.Write), It.IsAny<string>())).Returns(new Response());
            nrlsValidationMock.Setup(x => x.ValidJwt(It.Is<Tuple<JwtScopes, string>>(q => q.Item1 == JwtScopes.Write), It.Is<string>(s => s.Contains("-for-patch")))).Returns(new Response(true));

            var sdsMock = new Mock<ISdsService>();
            sdsMock.Setup(op => op.GetFor(It.IsAny<string>())).Returns((SdsViewModel)null);
            sdsMock.Setup(op => op.GetFor(It.Is<string>(x => x == "000"))).Returns(SdsViewModels.SdsAsid000);
            sdsMock.Setup(op => op.GetFor(It.Is<string>(x => x == "002"))).Returns(SdsViewModels.SdsAsid002); 
            sdsMock.Setup(op => op.GetFor(It.Is<string>(x => x == "20000000018"))).Returns(SdsViewModels.SdsAsid20000000018);

            _sdsService = sdsMock.Object;
            _spineSettings = spineSettingsMock.Object;
            _nrlsSettings = nrlsSettingsMock.Object;
            _nrlsValidation = nrlsValidationMock.Object;
        }

        public void Dispose()
        {
            _spineSettings = null;
            _nrlsSettings = null;
            _sdsService = null;
        }

        [Fact]
        public async void Valid_Headers()
        {
            var requestMock = new Mock<HttpRequest>();
            requestMock.Setup(x => x.Method).Returns("GET");
            requestMock.Setup(x => x.Path).Returns("/DocumentReference");
            requestMock.Setup(x => x.Headers).Returns(new HeaderDictionary()
            {
                { HeaderNames.Authorization, "we-are-not-validating-jwt-here" },
                { FhirConstants.HeaderFromAsid, "000" },
                { FhirConstants.HeaderToAsid, "999" }
            });


            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(x => x.Request).Returns(requestMock.Object);


            var sspAuthorizationMiddleware = new SpineAuthorizationMiddleware(next: (innerHttpContext) => Task.FromResult(0), spineSettings: _spineSettings, sdsService: _sdsService, nrlsValidation: _nrlsValidation);

            //Test will fail if invalid
            await sspAuthorizationMiddleware.Invoke(contextMock.Object, _nrlsSettings);
        }

        [Fact]
        public void Invalid_Header_Authorization_Missing()
        {
            var requestMock = new Mock<HttpRequest>();
            requestMock.Setup(x => x.Method).Returns("GET");
            requestMock.Setup(x => x.Path).Returns("/DocumentReference");
            requestMock.Setup(x => x.Headers).Returns(new HeaderDictionary() //Removed Authorization Header 
            {
                { FhirConstants.HeaderFromAsid, "000" },
                { FhirConstants.HeaderToAsid, "999" }
            });

            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(x => x.Request).Returns(requestMock.Object);

            var sspAuthorizationMiddleware = new SpineAuthorizationMiddleware(next: (innerHttpContext) => Task.FromResult(0), spineSettings: _spineSettings, sdsService: _sdsService, nrlsValidation: _nrlsValidation);

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
            requestMock.Setup(x => x.Path).Returns("/DocumentReference");
            requestMock.Setup(x => x.Headers).Returns(new HeaderDictionary()
            {
                { HeaderNames.Authorization, "we-are-not-validating-jwt-here" },
                { FhirConstants.HeaderFromAsid, "000" },
                { FhirConstants.HeaderToAsid, "999" }
            });

            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(x => x.Request).Returns(requestMock.Object);

            var sspAuthorizationMiddleware = new SpineAuthorizationMiddleware(next: (innerHttpContext) => Task.FromResult(0), spineSettings: _spineSettings, sdsService: _sdsService, nrlsValidation: _nrlsValidation);

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
            requestMock.Setup(x => x.Path).Returns("/DocumentReference");
            requestMock.Setup(x => x.Headers).Returns(new HeaderDictionary() //Removed fromASID Header 
            {
                { HeaderNames.Authorization, "we-are-not-validating-jwt-here" },
                { FhirConstants.HeaderToAsid, "999" }
            });

            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(x => x.Request).Returns(requestMock.Object);

            var sspAuthorizationMiddleware = new SpineAuthorizationMiddleware(next: (innerHttpContext) => Task.FromResult(0), spineSettings: _spineSettings, sdsService: _sdsService, nrlsValidation: _nrlsValidation);

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
            requestMock.Setup(x => x.Path).Returns("/DocumentReference");
            requestMock.Setup(x => x.Headers).Returns(new HeaderDictionary() //Using an invalid value for fromASID header
            {
                { HeaderNames.Authorization, "we-are-not-validating-jwt-here" },
                { FhirConstants.HeaderFromAsid, "001" },
                { FhirConstants.HeaderToAsid, "999" }
            });

            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(x => x.Request).Returns(requestMock.Object);

            var sspAuthorizationMiddleware = new SpineAuthorizationMiddleware(next: (innerHttpContext) => Task.FromResult(0), spineSettings: _spineSettings, sdsService: _sdsService, nrlsValidation: _nrlsValidation);

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
            requestMock.Setup(x => x.Path).Returns("/DocumentReference");
            requestMock.Setup(x => x.Headers).Returns(new HeaderDictionary() //Removed toASID Header 
            {
                { HeaderNames.Authorization, "we-are-not-validating-jwt-here" },
                { FhirConstants.HeaderFromAsid, "000" }
            });

            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(x => x.Request).Returns(requestMock.Object);

            var sspAuthorizationMiddleware = new SpineAuthorizationMiddleware(next: (innerHttpContext) => Task.FromResult(0), spineSettings: _spineSettings, sdsService: _sdsService, nrlsValidation: _nrlsValidation);

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
            requestMock.Setup(x => x.Path).Returns("/DocumentReference");
            requestMock.Setup(x => x.Headers).Returns(new HeaderDictionary() //Used invalid value for toASID Header 
            {
                { HeaderNames.Authorization, "we-are-not-validating-jwt-here" },
                { FhirConstants.HeaderFromAsid, "000" },
                { FhirConstants.HeaderToAsid, "888" }
            });

            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(x => x.Request).Returns(requestMock.Object);

            var sspAuthorizationMiddleware = new SpineAuthorizationMiddleware(next: (innerHttpContext) => Task.FromResult(0), spineSettings: _spineSettings, sdsService: _sdsService, nrlsValidation: _nrlsValidation);

            Assert.ThrowsAsync<HttpFhirException>(async delegate
            {
                await sspAuthorizationMiddleware.Invoke(contextMock.Object, _nrlsSettings);

            });
        }

        [Fact]
        public async void Valid_Read_Interaction()
        {
            var requestMock = new Mock<HttpRequest>();
            requestMock.Setup(x => x.Method).Returns("GET");
            requestMock.Setup(x => x.Path).Returns("/DocumentReference/abcDEF123");
            requestMock.Setup(x => x.Headers).Returns(new HeaderDictionary()
            {
                { HeaderNames.Authorization, "we-are-not-validating-jwt-here" },
                { FhirConstants.HeaderFromAsid, "000" },
                { FhirConstants.HeaderToAsid, "999" }
            });


            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(x => x.Request).Returns(requestMock.Object);


            var sspAuthorizationMiddleware = new SpineAuthorizationMiddleware(next: (innerHttpContext) => Task.FromResult(0), spineSettings: _spineSettings, sdsService: _sdsService, nrlsValidation: _nrlsValidation);

            //Test will fail if invalid
            await sspAuthorizationMiddleware.Invoke(contextMock.Object, _nrlsSettings);
        }

        [Fact]
        public async void Valid_Update_Interaction()
        {
            var requestMock = new Mock<HttpRequest>();
            requestMock.Setup(x => x.Method).Returns("PATCH");
            requestMock.Setup(x => x.Path).Returns("/DocumentReference");
            requestMock.Setup(x => x.Headers).Returns(new HeaderDictionary()
            {
                { HeaderNames.Authorization, "we-are-not-validating-jwt-here-for-patch" },
                { FhirConstants.HeaderFromAsid, "20000000018" },
                { FhirConstants.HeaderToAsid, "999" }
            });


            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(x => x.Request).Returns(requestMock.Object);


            var sspAuthorizationMiddleware = new SpineAuthorizationMiddleware(next: (innerHttpContext) => Task.FromResult(0), spineSettings: _spineSettings, sdsService: _sdsService, nrlsValidation: _nrlsValidation);

            //Test will fail if invalid
            await sspAuthorizationMiddleware.Invoke(contextMock.Object, _nrlsSettings);
        }

        [Fact]
        public void Invalid_Read_Interaction()
        {
            var requestMock = new Mock<HttpRequest>();
            requestMock.Setup(x => x.Method).Returns("GET");
            requestMock.Setup(x => x.Path).Returns("/DocumentReference/abcDEF123zzz");
            requestMock.Setup(x => x.Headers).Returns(new HeaderDictionary()
            {
                { HeaderNames.Authorization, "we-are-not-validating-jwt-here" },
                { FhirConstants.HeaderFromAsid, "000" },
                { FhirConstants.HeaderToAsid, "999" }
            });


            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(x => x.Request).Returns(requestMock.Object);


            var sspAuthorizationMiddleware = new SpineAuthorizationMiddleware(next: (innerHttpContext) => Task.FromResult(0), spineSettings: _spineSettings, sdsService: _sdsService, nrlsValidation: _nrlsValidation);

            //Test will fail if invalid
            Assert.ThrowsAsync<HttpFhirException>(async delegate
            {
                await sspAuthorizationMiddleware.Invoke(contextMock.Object, _nrlsSettings);

            });
        }

        [Fact]
        public void Invalid_Wrong_Interaction()
        {
            var requestMock = new Mock<HttpRequest>();
            requestMock.Setup(x => x.Method).Returns("POST");
            requestMock.Setup(x => x.Path).Returns("/DocumentReference");
            requestMock.Setup(x => x.Headers).Returns(new HeaderDictionary()
            {
                { HeaderNames.Authorization, "we-are-not-validating-jwt-here" },
                { FhirConstants.HeaderFromAsid, "000" },
                { FhirConstants.HeaderToAsid, "999" }
            });


            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(x => x.Request).Returns(requestMock.Object);


            var sspAuthorizationMiddleware = new SpineAuthorizationMiddleware(next: (innerHttpContext) => Task.FromResult(0), spineSettings: _spineSettings, sdsService: _sdsService, nrlsValidation: _nrlsValidation);

            //Test will fail if invalid
            Assert.ThrowsAsync<HttpFhirException>(async delegate
            {
                await sspAuthorizationMiddleware.Invoke(contextMock.Object, _nrlsSettings);

            });
        }

    }
}
