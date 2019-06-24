using Demonstrator.Core.Exceptions;
using Demonstrator.Core.Interfaces.Services;
using Demonstrator.Core.Interfaces.Services.Fhir;
using Demonstrator.Models.Core.Models;
using Demonstrator.Models.ViewModels.Base;
using Demonstrator.NRLSAdapter.DocumentReferences;
using Demonstrator.NRLSAdapter.Models;
using DemonstratorTest.Data;
using Hl7.Fhir.Model;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using SystemTasks = System.Threading.Tasks;

namespace DemonstratorTest.NRLSAdapter
{
    public class DocumentsServiceTests : IDisposable
    {
        IOptions<ExternalApiSetting> _extApiMock;
        IOptions<ApiSetting> _apiMock;
        ISdsService _sdsMock;
        IFhirConnector _fhirConnectorMock;

        public DocumentsServiceTests()
        {
            var extApiOpts = new ExternalApiSetting
            {
                NrlsServerUrl = new Uri("http://nrl-server.com"),

                NrlsSecureServerUrl = new Uri("http://nrl-secure-server.com"),

                NrlsUseSecure = false,

                NrlsDefaultprofile = "nrl-profile",

                SspServerUrl = new Uri("http://ssp-server.com"),

                SspSecureServerUrl = new Uri("http://ssp-secure-server.com"),

                SspUseSecure = false,

                SspSslThumbprint = "ssp-thumbprint",

                PdsServerUrl = new Uri("http://ssp-server.com"),

                PdsSecureServerUrl = new Uri("http://ssp-secure-server.com"),

                PdsUseSecure = false,

                PdsDefaultprofile = "pds-profile",

                OdsServerUrl = new Uri("http://ssp-server.com"),

                OdsSecureServerUrl = new Uri("http://ssp-secure-server.com"),

                OdsUseSecure = false,

                OdsDefaultprofile = "ods-profile",

                SpineAsid = "9999999999",

                SpineThumbprint = "SpineThumbprint",
            };

            var apiOpts = new ApiSetting
            {
                BaseUrl = "://localhost",
                Secure = false,
                SecureOnly = false,
                DefaultPort = "55448",
                SecurePort = "55443",
                SupportedContentTypes = new List<string> { "application/fhir+xml", "application/fhir+json" }
            };

            var extApiMock = new Mock<IOptions<ExternalApiSetting>>();
            extApiMock.Setup(op => op.Value).Returns(extApiOpts);

            var apiMock = new Mock<IOptions<ApiSetting>>();
            apiMock.Setup(op => op.Value).Returns(apiOpts);

            var sdsMock = new Mock<ISdsService>();
            sdsMock.Setup(op => op.GetFor(It.IsAny<string>())).Returns((SdsViewModel)null);
            sdsMock.Setup(op => op.GetFor(It.Is<string>(x => x == "20000000017"))).Returns(SdsViewModels.SdsAsid20000000017);
            sdsMock.Setup(op => op.GetFor(It.Is<string>(x => x == "002"))).Returns(SdsViewModels.SdsAsid002);

            sdsMock.Setup(op => op.GetFor(It.IsAny<string>(), null)).Returns((SdsViewModel)null);
            sdsMock.Setup(op => op.GetFor(It.Is<string>(x => x == "X27"), It.IsAny<string>())).Returns(SdsViewModels.SdsAsid888);

            var resource = new Binary
            {
                ContentType = "application/pdf",
                Content = Encoding.UTF8.GetBytes("pdf.bytes")
            };

            var fhirConnectorMock = new Mock<IFhirConnector>();
            fhirConnectorMock.Setup(op => op.RequestOneFhir<CommandRequest, Resource>(It.IsAny<CommandRequest>())).Returns(SystemTasks.Task.Run(() => resource as Resource));

            _extApiMock = extApiMock.Object;
            _apiMock = apiMock.Object;
            _sdsMock = sdsMock.Object;
            _fhirConnectorMock = fhirConnectorMock.Object;
        }

        public void Dispose()
        {

        }

        [Fact]
        public async void GetPointerDocument_Valid()
        {
            //this does not really prove much   

            var docSvc = new DocumentsServices(_extApiMock, _apiMock, _sdsMock, _fhirConnectorMock);

            var actual = await docSvc.GetPointerDocument("20000000017", "fromODS", "X27", "http://pointer.url");

            Assert.IsType<Binary>(actual);
            Assert.NotNull(actual);

            var binary = actual as Binary;

            Assert.Equal("application/pdf", binary.ContentType);
            Assert.Equal(Encoding.UTF8.GetBytes("pdf.bytes"), binary.Content);
        }

        [Fact]
        public void GetPointerDocument_NullFromAsid()
        {
            var docSvc = new DocumentsServices(_extApiMock, _apiMock, _sdsMock, _fhirConnectorMock);

            Assert.ThrowsAsync<HttpFhirException>(async () => await docSvc.GetPointerDocument(null, "fromODS", "X27", "http://pointer.url"));
        }

        [Fact]
        public void GetPointerDocument_NullToODS()
        {
            var docSvc = new DocumentsServices(_extApiMock, _apiMock, _sdsMock, _fhirConnectorMock);

            Assert.ThrowsAsync<HttpFhirException>(async () => await docSvc.GetPointerDocument("20000000017", "fromODS", null, "http://pointer.url"));
        }

    }
}
