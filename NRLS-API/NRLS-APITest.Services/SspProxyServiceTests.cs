using Hl7.Fhir.Rest;
using Microsoft.Net.Http.Headers;
using Moq;
using NRLS_API.Core.Interfaces.Helpers;
using NRLS_API.Models.Core;
using NRLS_API.Services;
using NRLS_APITest.Data;
using NRLS_APITest.StubClasses;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using Xunit;

namespace NRLS_APITest.Services
{
    public class SspProxyServiceTests
    {

        [Fact]
        public async void SdsSProxy_GetHtmlOk()
        {
            var command = new CommandRequest
            {
                Method = HttpMethod.Get,
                ForwardUrl = new Uri("https://testtrust.testnhs.uk/path/to/record/abc123"),
                Forwarded = new Forwarded
                {
                    By = "127.0.0.1",
                    For = "127.0.0.1",
                    Host = "esttrust.testnhs.uk",
                    Protocol = "https"
                },
                ClientThumbprint = null,
                ServerThumbprint = null,
                UseSecure = false,
                Headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>(HeaderNames.Accept, $"{ContentType.JSON_CONTENT_HEADER}"),
                    new KeyValuePair<string, string>(HeaderNames.AcceptEncoding, "gzip, deflate"),
                    new KeyValuePair<string, string>(HeaderNames.AcceptLanguage, "en-GB,en"),
                    new KeyValuePair<string, string>(HeaderNames.CacheControl, "no-cache"),
                    new KeyValuePair<string, string>(HeaderNames.Connection, "Keep-Alive"),
                    new KeyValuePair<string, string>(HeaderNames.Host, "testtrust.testnhs.uk")
                }
            };

            var request = new HttpRequestMessage
            {
                RequestUri = command.ForwardUrl,
                Method = command.Method,
                Headers =
                {
                    { "Forwarded", $"by={command.Forwarded.By};for={command.Forwarded.For};host={command.Forwarded.Host};proto={command.Forwarded.Protocol}" }
                }
            };

            var jsonString = FhirBinaries.Html.AsJsonString();

            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Version = HttpVersion.Version11,
                ReasonPhrase = HttpStatusCode.OK.ToString(),
                RequestMessage = request,
                Content = new StringContent(jsonString, Encoding.UTF8, ContentType.JSON_CONTENT_HEADER)
            };

            var stubHandler = HttpMessageStub.GetClientHandler(response);

            var httpRequestHelperMock = new Mock<IHttpRequestHelper>();
            httpRequestHelperMock.Setup(op => op.GetClientHandler(It.IsAny<CommandRequest>())).Returns(stubHandler.Object);
            httpRequestHelperMock.Setup(op => op.GetRequestMessage(It.IsAny<CommandRequest>())).Returns(request);

            var service = new SspProxyService(httpRequestHelperMock.Object);

            var commandResponse = await service.ForwardRequest(command);

            var content = Encoding.UTF8.GetString(commandResponse.Content, 0, commandResponse.Content.Length);

            Assert.Equal(200, commandResponse.StatusCode);
            Assert.NotNull(commandResponse.Headers);
            Assert.Contains(HeaderNames.ContentType, commandResponse.Headers.Keys);
            Assert.Equal(jsonString, content);
        }

        
    }
}
