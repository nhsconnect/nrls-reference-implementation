using Demonstrator.Core.Interfaces.Services;
using Demonstrator.Core.Interfaces.Services.Fhir;
using Demonstrator.Models.Core.Models;
using Demonstrator.Models.Nrls;
using Demonstrator.Models.ViewModels.Base;
using Demonstrator.NRLSAdapter.Helpers;
using Demonstrator.NRLSAdapter.Helpers.Models;
using Demonstrator.NRLSAdapter.Models;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Serialization;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using SystemTasks = System.Threading.Tasks;

namespace Demonstrator.NRLSAdapter.DocumentReferences
{
    public class DocumentsServices : IDocumentsServices
    {
        private readonly ExternalApiSetting _spineSettings;
        private readonly ApiSetting _apiSettings;
        private readonly ISdsService _sdsService;
        private readonly IFhirConnector _fhirConnector;

        public DocumentsServices(IOptions<ExternalApiSetting> externalApiSetting, IOptions<ApiSetting> apiSetting, ISdsService sdsService, IFhirConnector fhirConnector)
        {
            _spineSettings = externalApiSetting.Value;
            _apiSettings = apiSetting.Value;
            _sdsService = sdsService;
            _fhirConnector = fhirConnector;
        }

        public async SystemTasks.Task<Resource> GetPointerDocument(string fromASID, string fromODS, string toODS, string pointerUrl)
        {
            var request = BuildGetRequest(fromASID, fromODS, toODS);

            //SSP base normally retrieved from SDS, but can be cached
            request.BaseUrl = $"{SspUrlBase}{WebUtility.UrlEncode((pointerUrl))}";

            var document = await _fhirConnector.RequestOneFhir<CommandRequest, Resource>(request);

            return document;
        }
        

        private CommandRequest BuildGetRequest(string asid, string jwtOrgCode, string providerOds)
        {
            return BuildRequest(asid, jwtOrgCode, providerOds);
        }

        private CommandRequest BuildRequest(string asid, string jwtOrgCode, string providerOds)
        {
            var command = new CommandRequest
            {
                BaseUrl = $"{(_spineSettings.SspUseSecure ? _spineSettings.SspSecureServerUrl : _spineSettings.SspServerUrl)}",
                ResourceType = ResourceType.Binary,
                Method = HttpMethod.Get,
                UseSecure = _spineSettings.SspUseSecure,
                ClientThumbprint = _sdsService.GetFor(asid)?.Thumbprint,
                ServerThumbprint = _spineSettings.SspSslThumbprint,
                RegenerateUrl = false
            };

            var jwt = JwtFactory.Generate(JwtScopes.Read, jwtOrgCode, "fakeRoleId", asid, command.FullUrl.AbsoluteUri, SystemUrlBase);

            command.Headers.Add(HeaderNames.Authorization, $"Bearer {jwt}");
            command.Headers.Add(FhirConstants.HeaderSspFrom, asid); // GET consumer ASID
            command.Headers.Add(FhirConstants.HeaderSspTo, _sdsService.GetFor(providerOds, FhirConstants.ReadBinaryInteractionId)?.Asid); // GET provider asid
            command.Headers.Add(FhirConstants.HeaderSspInterationId, FhirConstants.ReadBinaryInteractionId);
            command.Headers.Add(FhirConstants.HeaderSspTraceId, Guid.NewGuid().ToString());

            return command;
        }

        private string SystemUrlBase
        {
            get
            {
                return $"{(_apiSettings.Secure ? "https" : "http")}{_apiSettings.BaseUrl}:{(_apiSettings.Secure ? _apiSettings.SecurePort : _apiSettings.DefaultPort)}";
            }
        }

        private string SspUrlBase
        {
            get
            {
                return $"{(_spineSettings.SspUseSecure ? _spineSettings.SspSecureServerUrl : _spineSettings.SspServerUrl)}/";
            }
        }
    }
}
