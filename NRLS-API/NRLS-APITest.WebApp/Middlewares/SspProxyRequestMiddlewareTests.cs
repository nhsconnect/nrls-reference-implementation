using Hl7.Fhir.Rest;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
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
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NRLS_APITest.WebApp.Middlewares
{
    public class SspProxyRequestMiddlewareTests : IDisposable
    {
        private IOptionsSnapshot<ApiSetting> _sspApiSettings;
        private ISspProxyService _sspProxyService;

        public SspProxyRequestMiddlewareTests()
        {

            var sspApiSettingsMock = new Mock<IOptionsSnapshot<ApiSetting>>();
            sspApiSettingsMock.Setup(op => op.Get(It.IsAny<string>())).Returns(AppSettings.SspApiSettings);

            var responseContent = FhirBinaries.Html.AsJsonString();
            var response = new CommandResponse
            {
                StatusCode = 200,
                Headers = new Dictionary<string, IEnumerable<string>>
                {
                    { HeaderNames.ContentType, new StringValues(new string[]{ "application/fhir+json" }) },
                    { HeaderNames.ContentLength, new StringValues(new string[]{ "80" }) }
                },
                Content = Encoding.UTF8.GetBytes(responseContent)
            };

            var sspServiceMock = new Mock<ISspProxyService>();
            sspServiceMock.Setup(op => op.ForwardRequest(It.IsAny<CommandRequest>())).Returns(Task.FromResult(response));


            _sspProxyService = sspServiceMock.Object;
            _sspApiSettings = sspApiSettingsMock.Object;
        }

        public void Dispose()
        {
            _sspApiSettings = null;
            _sspProxyService = null;
        }

        [Fact]
        public async void ProxyRequest_Valid()
        {
            var requestMock = new Mock<HttpRequest>();
            requestMock.Setup(x => x.Method).Returns("GET");
            requestMock.Setup(x => x.Path).Returns(new PathString("/nrls-ri/SSP/http%3A%2F%2Flocalhost%3A55448%2Fprovider%2F00003X%2Ffhir%2FSTU3%2Fcareconnect%2Fbinary%2F5aba0f464f02ced4c7eb16c4"));
            requestMock.Setup(x => x.Headers).Returns(new HeaderDictionary()
            {
                //{ HeaderNames.Authorization, "we-are-not-validating-jwt-here" },
                //{ FhirConstants.HeaderFromAsid, "000" },
                //{ FhirConstants.HeaderToAsid, "999" },
                //{ FhirConstants.HeaderSspInterationId, FhirConstants.ReadInteractionId }
                { HeaderNames.Accept, ContentType.JSON_CONTENT_HEADER }
            });

            var connectionMock = new Mock<ConnectionInfo>();
            connectionMock.Setup(x => x.LocalIpAddress).Returns(new IPAddress(1270001));
            connectionMock.Setup(x => x.RemoteIpAddress).Returns(new IPAddress(1270001));

            var responseMock = new Mock<HttpResponse>();
            responseMock.SetupAllProperties();
            responseMock.Setup(x => x.Headers).Returns(new HeaderDictionary());
            responseMock.Setup(x => x.Body).Returns(new MemoryStream());

            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(x => x.Request).Returns(requestMock.Object);
            contextMock.Setup(x => x.Connection).Returns(connectionMock.Object);
            contextMock.Setup(x => x.Response).Returns(responseMock.Object);

            var middleware = new SspProxyRequestMiddleware(next: (innerHttpContext) => Task.FromResult(0));

            HttpContext context = contextMock.Object;

            await middleware.Invoke(context, _sspApiSettings, _sspProxyService);

            var response = context.Response;

            Assert.NotNull(response.Headers);
            Assert.NotEmpty(response.Headers);
            Assert.Contains(HeaderNames.ContentType, response.Headers.Keys);
            Assert.Contains(HeaderNames.ContentLength, response.Headers.Keys);
            Assert.Equal(200, response.StatusCode);

            response.Body.Seek(0, SeekOrigin.Begin);
            var actualBody = await new StreamReader(response.Body).ReadToEndAsync();

            var expectBody = FhirBinaries.Html.AsJsonString();

            Assert.Equal(expectBody, actualBody);
        }
                
    }
}
