using Demonstrator.Core.Exceptions;
using Demonstrator.Core.Interfaces.Helpers;
using Demonstrator.Core.Resources;
using Demonstrator.NRLSAdapter.Helpers;
using Demonstrator.NRLSAdapter.Models;
using DemonstratorTest.Data;
using DemonstratorTest.StubClasses;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Microsoft.Net.Http.Headers;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using Xunit;

namespace DemonstratorTest.NRLSAdapter
{
    public class FhirConnectorTests : IDisposable
    {
        ILoggingHelper _loggingHelper;
        IHttpRequestHelper _httpRequestHelper;

        Uri _requestUri;

        string _jwtToken;

        public FhirConnectorTests()
        {
            var loggerMock = new Mock<ILoggingHelper>();
            //apiMock.Setup(op => op.Value).Returns(apiOpts);
            _loggingHelper = loggerMock.Object;

            _requestUri = new Uri("http://outbound.com/binary");

            var httpRequest = new HttpRequestMessage()
            {
                RequestUri = _requestUri,
                Method = HttpMethod.Get,
                Headers =
                {
                    { HeaderNames.AcceptEncoding, "gzip, deflate" },
                    { HeaderNames.AcceptLanguage, "en-GB,en" },
                    { HeaderNames.CacheControl, "no-cache" },
                    { HeaderNames.Connection, "Keep-Alive" },
                    { HeaderNames.Host, _requestUri.Host },
                }
            };

            var binaryHtmlAsJson = FhirBinaries.Html.AsJsonString();

            var okBinaryResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Version = HttpVersion.Version11,
                ReasonPhrase = HttpStatusCode.OK.ToString(),
                RequestMessage = httpRequest,
                Content = new StringContent(binaryHtmlAsJson, Encoding.UTF8, ContentType.JSON_CONTENT_HEADER)
            };

            var okBinaryResponseWithLocation = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Version = HttpVersion.Version11,
                ReasonPhrase = HttpStatusCode.OK.ToString(),
                RequestMessage = httpRequest,
                Content = new StringContent(binaryHtmlAsJson, Encoding.UTF8, ContentType.JSON_CONTENT_HEADER),
                Headers =
                {
                    { HeaderNames.Location, "http://reposiroty.genhos.nhs.uk/binary/abc" }
                }
            };

            var binaryBundleAsJson = FhirBinaries.AsBundle.AsJsonString();

            var okBinaryBundleResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Version = HttpVersion.Version11,
                ReasonPhrase = HttpStatusCode.OK.ToString(),
                RequestMessage = httpRequest,
                Content = new StringContent(binaryBundleAsJson, Encoding.UTF8, ContentType.JSON_CONTENT_HEADER)
            };

            var okEmptyContentResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Version = HttpVersion.Version11,
                ReasonPhrase = HttpStatusCode.OK.ToString(),
                RequestMessage = httpRequest,
                Content = new StringContent(string.Empty, Encoding.UTF8, ContentType.JSON_CONTENT_HEADER)
            };

            var okNullContentResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Version = HttpVersion.Version11,
                ReasonPhrase = HttpStatusCode.OK.ToString(),
                RequestMessage = httpRequest,
                Content = null
            };

            var operationOutcomeAsJson = FhirOperationOutcomes.NotFound.AsJsonString();

            var notFoundBinaryResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound,
                Version = HttpVersion.Version11,
                ReasonPhrase = HttpStatusCode.NotFound.ToString(),
                RequestMessage = httpRequest,
                Content = new StringContent(operationOutcomeAsJson, Encoding.UTF8, ContentType.JSON_CONTENT_HEADER)
            };

            var okBinaryStubHandler = HttpMessageStub.GetClientHandler(okBinaryResponse);
            var okBinaryLocationStubHandler = HttpMessageStub.GetClientHandler(okBinaryResponseWithLocation);
            var okBinaryBundleStubHandler = HttpMessageStub.GetClientHandler(okBinaryBundleResponse);
            var okNullContentStubHandler = HttpMessageStub.GetClientHandler(okNullContentResponse);
            var okEmptyContentStubHandler = HttpMessageStub.GetClientHandler(okEmptyContentResponse);
            var notFoundStubHandler = HttpMessageStub.GetClientHandler(notFoundBinaryResponse);

            var httpRequestHelperMock = new Mock<IHttpRequestHelper>();
            httpRequestHelperMock.Setup(op => op.GetClientHandler(It.IsAny<CommandRequest>())).Returns(okBinaryStubHandler.Object);
            httpRequestHelperMock.Setup(op => op.GetClientHandler(It.Is<CommandRequest>(x => x.Headers.FirstOrDefault(y => y.Key == FhirConstants.HeaderSspFrom).Value == "20000000016"))).Returns(okBinaryBundleStubHandler.Object);
            httpRequestHelperMock.Setup(op => op.GetClientHandler(It.Is<CommandRequest>(x => x.Headers.FirstOrDefault(y => y.Key == FhirConstants.HeaderSspFrom).Value == "20000000015"))).Returns(okNullContentStubHandler.Object);
            httpRequestHelperMock.Setup(op => op.GetClientHandler(It.Is<CommandRequest>(x => x.Headers.FirstOrDefault(y => y.Key == FhirConstants.HeaderSspFrom).Value == "20000000014"))).Returns(okEmptyContentStubHandler.Object);
            httpRequestHelperMock.Setup(op => op.GetClientHandler(It.Is<CommandRequest>(x => x.Headers.FirstOrDefault(y => y.Key == FhirConstants.HeaderSspFrom).Value == "20000000013"))).Returns(notFoundStubHandler.Object);
            httpRequestHelperMock.Setup(op => op.GetClientHandler(It.Is<CommandRequest>(x => x.Headers.FirstOrDefault(y => y.Key == FhirConstants.HeaderSspFrom).Value == "20000000012"))).Returns(okBinaryLocationStubHandler.Object);
            httpRequestHelperMock.Setup(op => op.GetRequestMessage(It.IsAny<CommandRequest>())).Returns(httpRequest);
            _httpRequestHelper = httpRequestHelperMock.Object;

            _jwtToken = "eyJhbGciOiJub25lIiwidHlwIjoiSldUIn0.eyJpc3MiOiJodHRwczovL2RlbW9uc3RyYXRvci5jb20iLCJzdWIiOiJodHRwczovL2ZoaXIubmhzLnVrL0lkL3Nkcy1yb2xlLXByb2ZpbGUtaWR8ZmFrZVJvbGVJZCIsImF1ZCI6Imh0dHBzOi8vbnJscy5jb20vZmhpci9kb2N1bWVudHJlZmVyZW5jZSIsImV4cCI6MTUyMjU3NzEzMCwiaWF0IjoxNTIyNTc2ODMwLCJyZWFzb25fZm9yX3JlcXVlc3QiOiJkaXJlY3RjYXJlIiwic2NvcGUiOiJwYXRpZW50L0RvY3VtZW50UmVmZXJlbmNlLndyaXRlIiwicmVxdWVzdGluZ19zeXN0ZW0iOiJodHRwczovL2ZoaXIubmhzLnVrL0lkL2FjY3JlZGl0ZWQtc3lzdGVtfDIwMDAwMDAwMDE3IiwicmVxdWVzdGluZ19vcmdhbml6YXRpb24iOiJodHRwczovL2ZoaXIubmhzLnVrL0lkL29kcy1vcmdhbml6YXRpb24tY29kZXxPUkcxIiwicmVxdWVzdGluZ191c2VyIjoiaHR0cHM6Ly9maGlyLm5ocy51ay9JZC9zZHMtcm9sZS1wcm9maWxlLWlkfGZha2VSb2xlSWQifQ.";

        }

        public void Dispose()
        {

        }

        [Fact]
        public async void ValidRequest_RequestOneFhir()
        {

            var command = new CommandRequest
            {
                BaseUrl = _requestUri.AbsoluteUri,
                ResourceType = ResourceType.Binary,
                Method = HttpMethod.Get,
                UseSecure = false,
                ClientThumbprint = null,
                ServerThumbprint = null,
                RegenerateUrl = false,
                Headers =
                {
                    { HeaderNames.Authorization, $"Bearer {_jwtToken}" },
                    { FhirConstants.HeaderSspFrom, "20000000017" },
                    { FhirConstants.HeaderSspTo, "20000000018" },
                    { FhirConstants.HeaderSspInterationId, FhirConstants.ReadBinaryInteractionId },
                    { FhirConstants.HeaderSspTraceId, Guid.NewGuid().ToString() }
                }
            };

            var connector = new FhirConnector(_loggingHelper, _httpRequestHelper);

            var actual = await connector.RequestOneFhir<CommandRequest, Binary>(command);

            Assert.IsType<Binary>(actual);
            Assert.NotNull(actual);

            var binary = actual as Binary;

            Assert.Equal("text/html", binary.ContentType);
            Assert.Equal(Encoding.UTF8.GetBytes("<p>Hello</p>"), binary.Content);
        }

        [Fact]
        public async void ValidRequest_RequestOneFhir_WithLocation()
        {

            var command = new CommandRequest
            {
                BaseUrl = _requestUri.AbsoluteUri,
                ResourceType = ResourceType.Binary,
                Method = HttpMethod.Get,
                UseSecure = false,
                ClientThumbprint = null,
                ServerThumbprint = null,
                RegenerateUrl = false,
                Headers =
                {
                    { HeaderNames.Authorization, $"Bearer {_jwtToken}" },
                    { FhirConstants.HeaderSspFrom, "20000000012" },
                    { FhirConstants.HeaderSspTo, "20000000018" },
                    { FhirConstants.HeaderSspInterationId, FhirConstants.ReadBinaryInteractionId },
                    { FhirConstants.HeaderSspTraceId, Guid.NewGuid().ToString() }
                }
            };

            var connector = new FhirConnector(_loggingHelper, _httpRequestHelper);

            var actual = await connector.RequestOne<CommandRequest, FhirResponse>(command);

            Assert.IsType<FhirResponse>(actual);
            Assert.NotNull(actual);

            Assert.IsType<Binary>(actual.Resource);
            Assert.NotNull(actual.ResponseLocation);
        }

        [Fact]
        public async void ValidRequest_RequestManyFhir()
        {

            var command = new CommandRequest
            {
                BaseUrl = _requestUri.AbsoluteUri,
                ResourceType = ResourceType.Binary,
                Method = HttpMethod.Get,
                UseSecure = false,
                ClientThumbprint = null,
                ServerThumbprint = null,
                RegenerateUrl = false,
                Headers =
                {
                    { HeaderNames.Authorization, $"Bearer {_jwtToken}" },
                    { FhirConstants.HeaderSspFrom, "20000000016" },
                    { FhirConstants.HeaderSspTo, "20000000018" },
                    { FhirConstants.HeaderSspInterationId, FhirConstants.ReadBinaryInteractionId },
                    { FhirConstants.HeaderSspTraceId, Guid.NewGuid().ToString() }
                }
            };

            var connector = new FhirConnector(_loggingHelper, _httpRequestHelper);

            var actual = await connector.RequestMany<CommandRequest, Binary>(command);

            Assert.IsType<List<Binary>>(actual);
            Assert.NotEmpty(actual);

            Assert.Collection(actual, item =>
            {
                Assert.Equal("text/html", item.ContentType);
                Assert.Equal(Encoding.UTF8.GetBytes("<p>Hello</p>"), item.Content);
            },
            item =>
            {
                Assert.Equal("application/pdf", item.ContentType);
                Assert.Equal(Encoding.UTF8.GetBytes("pdf.file"), item.Content);
            });

        }

        [Fact]
        public async void ValidRequest_RequestOneFhir_EmptyContent()
        {

            var command = new CommandRequest
            {
                BaseUrl = _requestUri.AbsoluteUri,
                ResourceType = ResourceType.Binary,
                Method = HttpMethod.Get,
                UseSecure = false,
                ClientThumbprint = null,
                ServerThumbprint = null,
                RegenerateUrl = false,
                Headers =
                {
                    { HeaderNames.Authorization, $"Bearer {_jwtToken}" },
                    { FhirConstants.HeaderSspFrom, "20000000014" },
                    { FhirConstants.HeaderSspTo, "20000000018" },
                    { FhirConstants.HeaderSspInterationId, FhirConstants.ReadBinaryInteractionId },
                    { FhirConstants.HeaderSspTraceId, Guid.NewGuid().ToString() }
                }
            };

            var connector = new FhirConnector(_loggingHelper, _httpRequestHelper);

            var actual = await connector.RequestOneFhir<CommandRequest, Binary>(command);

            Assert.Null(actual);

        }

        [Fact]
        public void ValidRequest_RequestOneFhir_NoContent()
        {

            var command = new CommandRequest
            {
                BaseUrl = _requestUri.AbsoluteUri,
                ResourceType = ResourceType.Binary,
                Method = HttpMethod.Get,
                UseSecure = false,
                ClientThumbprint = null,
                ServerThumbprint = null,
                RegenerateUrl = false,
                Headers =
                {
                    { HeaderNames.Authorization, $"Bearer {_jwtToken}" },
                    { FhirConstants.HeaderSspFrom, "20000000015" },
                    { FhirConstants.HeaderSspTo, "20000000018" },
                    { FhirConstants.HeaderSspInterationId, FhirConstants.ReadBinaryInteractionId },
                    { FhirConstants.HeaderSspTraceId, Guid.NewGuid().ToString() }
                }
            };

            var connector = new FhirConnector(_loggingHelper, _httpRequestHelper);

            Assert.ThrowsAsync<HttpRequestException>(async () => await connector.RequestOneFhir<CommandRequest, Binary>(command));

        }

        [Fact]
        public void ValidRequest_RequestOneFhir_NotFoundError()
        {

            var command = new CommandRequest
            {
                BaseUrl = _requestUri.AbsoluteUri,
                ResourceType = ResourceType.Binary,
                Method = HttpMethod.Get,
                UseSecure = false,
                ClientThumbprint = null,
                ServerThumbprint = null,
                RegenerateUrl = false,
                Headers =
                {
                    { HeaderNames.Authorization, $"Bearer {_jwtToken}" },
                    { FhirConstants.HeaderSspFrom, "20000000013" },
                    { FhirConstants.HeaderSspTo, "20000000018" },
                    { FhirConstants.HeaderSspInterationId, FhirConstants.ReadBinaryInteractionId },
                    { FhirConstants.HeaderSspTraceId, Guid.NewGuid().ToString() }
                }
            };

            var connector = new FhirConnector(_loggingHelper, _httpRequestHelper);

            Assert.ThrowsAsync<HttpFhirException>(async () => await connector.RequestOneFhir<CommandRequest, Binary>(command));

        }
    }
}
