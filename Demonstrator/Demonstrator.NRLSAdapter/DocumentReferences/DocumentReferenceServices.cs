using Demonstrator.Core.Interfaces.Services.Fhir;
using Demonstrator.Models.Core.Models;
using Demonstrator.Models.Nrls;
using Demonstrator.NRLSAdapter.Helpers;
using Demonstrator.NRLSAdapter.Helpers.Models;
using Demonstrator.NRLSAdapter.Models;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Serialization;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using SystemTasks = System.Threading.Tasks;

namespace Demonstrator.NRLSAdapter.DocumentReferences
{
    public class DocumentReferenceServices : IDocumentReferenceServices
    {
        private string _documentReferenceUrlBase;
        private string _spineAsid;
        private string _systemUrlBase;

        public DocumentReferenceServices(IOptions<ExternalApiSetting> externalApiSetting, IOptions<ApiSetting> apiSetting)
        {
            _documentReferenceUrlBase = $"{externalApiSetting.Value.NrlsServerUrl}";
            _spineAsid = externalApiSetting.Value.SpineAsid;
            _systemUrlBase = $"{(apiSetting.Value.Secure ? "https" : "http")}{apiSetting.Value.BaseUrl}";
        }

        public async SystemTasks.Task<Bundle> GetPointersAsBundle(NrlsPointerRequest pointerRequest)
        {
            var pointers = await new FhirConnector().RequestOne<Bundle>(BuildGetRequest(pointerRequest.Asid, pointerRequest.Interaction, pointerRequest.NhsNumber, pointerRequest.OrgCode, pointerRequest.PointerId));

            return pointers;
        }

        public async SystemTasks.Task<NrlsCreateResponse> GenerateAndCreatePointer(NrlsPointerRequest pointerRequest)
        {
            var pointer = NrlsPointer.Generate(pointerRequest.OrgCode, pointerRequest.NhsNumber, pointerRequest.RecordUrl, pointerRequest.RecordContentType, pointerRequest.TypeCode, pointerRequest.TypeDisplay);

            var newPointer = await CreatePointer(pointerRequest, pointer);

            return newPointer;
        }

        public async SystemTasks.Task<NrlsCreateResponse> CreatePointer(NrlsPointerRequest pointerRequest, DocumentReference pointer)
        {
            var pointerJson = new FhirJsonSerializer().SerializeToString(pointer);
            var content = new StringContent(pointerJson, Encoding.UTF8, "application/fhir+json");

            var newPointer = await new FhirConnector().RequestOne(BuildPostRequest(pointerRequest.Asid, pointerRequest.Interaction, pointerRequest.OrgCode, content));

            var createResponse = new NrlsCreateResponse
            {
                Resource = newPointer.GetResource<OperationOutcome>(),
                ResponseLocation = newPointer.ResponseLocation
            };

            return createResponse;
        }

        public async SystemTasks.Task<OperationOutcome> DeletePointer(NrlsPointerRequest pointerRequest)
        {
            var pointer = await new FhirConnector().RequestOne<OperationOutcome>(BuildDeleteRequest(pointerRequest.Asid, pointerRequest.Interaction, pointerRequest.OrgCode, pointerRequest.PointerId));

            return pointer;
        }

        private CommandRequest BuildGetRequest(string asid, string interaction, string nhsNumber, string orgCode, string pointerId)
        {
            return BuildRequest(asid, interaction, pointerId, nhsNumber, orgCode, HttpMethod.Get, null);
        }

        private CommandRequest BuildPostRequest(string asid, string interaction, string orgCode, HttpContent content)
        {
            
            return BuildRequest(asid, interaction, null, null, orgCode, HttpMethod.Post, content);
        }

        private CommandRequest BuildDeleteRequest(string asid, string interaction, string orgCode, string pointerId)
        {
            return BuildRequest(asid, interaction, pointerId, null, orgCode, HttpMethod.Delete, null);
        }

        private CommandRequest BuildRequest(string asid, string interaction, string resourceId, string nhsNumber, string orgCode, HttpMethod method, HttpContent content)
        {
            var command = new CommandRequest
            {
                BaseUrl = _documentReferenceUrlBase,
                ResourceId = resourceId,
                ResourceType = ResourceType.DocumentReference,
                SearchParams = GetParams(nhsNumber, orgCode, resourceId),
                Method = method,
                Content = content
            };

            var jwt = JwtFactory.Generate(method == HttpMethod.Get ? JwtScopes.Read : JwtScopes.Write, orgCode, "fakeRoleId", asid, command.FullUrl.AbsoluteUri, _systemUrlBase);

            command.Headers.Add(HttpRequestHeader.Authorization.ToString(), $"Bearer {jwt}");
            command.Headers.Add(FhirConstants.HeaderFromAsid, asid);
            command.Headers.Add(FhirConstants.HeaderToAsid, _spineAsid);
            command.Headers.Add(FhirConstants.HeaderSspInterationId, interaction);
            command.Headers.Add(FhirConstants.HeaderFSspVersion, "1");
            command.Headers.Add(FhirConstants.HeaderSspTradeId, Guid.NewGuid().ToString());

            return command;
        }

        private SearchParams GetParams(string nhsNumber, string orgCode, string id)
        {
            var searchParams = new SearchParams();

            if (!string.IsNullOrWhiteSpace(orgCode))
            {
                searchParams.Add("custodian", $"{WebUtility.UrlEncode(FhirConstants.SystemODS)}{orgCode}");
            }

            if (!string.IsNullOrWhiteSpace(nhsNumber))
            {
                searchParams.Add("patient", $"{WebUtility.UrlEncode(FhirConstants.SystemPDS)}{nhsNumber}");
            }

            if (!string.IsNullOrWhiteSpace(id))
            {
                searchParams.Add("_id", id);
            }

            return searchParams;
        }
    }
}
