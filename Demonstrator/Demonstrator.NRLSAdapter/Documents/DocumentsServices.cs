using Demonstrator.Core.Interfaces.Services.Fhir;
using Demonstrator.Models.Core.Models;
using Demonstrator.Models.Nrls;
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
        private readonly IMemoryCache _cache;

        public DocumentsServices(IOptions<ExternalApiSetting> externalApiSetting, IOptions<ApiSetting> apiSetting, IMemoryCache cache)
        {
            _spineSettings = externalApiSetting.Value;
            _apiSettings = apiSetting.Value;
            _cache = cache;
        }

        public async SystemTasks.Task<Resource> GetPointerDocument(string pointerUrl)
        {
            var request = BuildGetRequest("200000000117", "AMS01", "MHT01");

            request.BaseUrl = pointerUrl;

            var document = await new FhirConnector().RequestOne<Resource>(request);

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
                ClientThumbprint = ClientSettings(asid)?.Thumbprint,
                ServerThumbprint = _spineSettings.SspSslThumbprint,
                RegenerateUrl = false
            };

            var jwt = JwtFactory.Generate(JwtScopes.Read, jwtOrgCode, "fakeRoleId", asid, command.FullUrl.AbsoluteUri, SystemUrlBase);

            command.Headers.Add(HeaderNames.Authorization, $"Bearer {jwt}");
            command.Headers.Add(FhirConstants.HeaderSspFrom, asid); // GET consumer ASID
            command.Headers.Add(FhirConstants.HeaderSspTo, SDSLookup(providerOds, FhirConstants.ReadBinaryInteractionId)); // GET provider asid
            command.Headers.Add(FhirConstants.HeaderSspInterationId, FhirConstants.ReadBinaryInteractionId);
            command.Headers.Add(FhirConstants.HeaderSspTraceId, Guid.NewGuid().ToString());

            return command;
        }

        private ClientAsid ClientSettings(string asid)
        {
            var map = _cache.Get<ClientAsidMap>(ClientAsidMap.Key);

            return map.ClientAsids.FirstOrDefault(x => !string.IsNullOrEmpty(asid) && x.Key == asid).Value;
        }

        private string SDSLookup(string odsCode, string interactionId)
        {
            //$"Filter: (&(nhsassvcia={interactionId})(nhsidcode={odsCode})(objectclass=nhsas))"
            var map = _cache.Get<ClientAsidMap>(ClientAsidMap.Key);

            return map.ClientAsids.FirstOrDefault(x => !string.IsNullOrEmpty(odsCode) && x.Value.OrgCode == odsCode 
                                                    && !string.IsNullOrEmpty(interactionId) && x.Value.Interactions.Contains(interactionId)).Key;
        }

        private string SystemUrlBase
        {
            get
            {
                return $"{(_apiSettings.Secure ? "https" : "http")}{_apiSettings.BaseUrl}:{(_apiSettings.Secure ? _apiSettings.SecurePort : _apiSettings.DefaultPort)}";
            }
        }
    }
}
