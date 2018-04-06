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
using System.Collections.Generic;
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

        public DocumentReferenceServices(IOptions<ExternalApiSetting> externalApiSetting)
        {
            _documentReferenceUrlBase = $"{externalApiSetting.Value.NrlsServerUrl}";
            _spineAsid = externalApiSetting.Value.SpineAsid;
        }

        public async SystemTasks.Task<Bundle> GetPointersAsBundle(NrlsPointerRequest pointerRequest)
        {
            var pointers = await new FhirConnector().RequestOne<Bundle>(BuildGetRequest(pointerRequest.Asid, pointerRequest.Interaction, pointerRequest.NhsNumber, pointerRequest.OrgCode));

            return pointers;
        }

        public async SystemTasks.Task<DocumentReference> GenerateAndCreatePointer(NrlsPointerRequest pointerRequest)
        {
            var pointer = NrlsPointer.Generate(pointerRequest.OrgCode, pointerRequest.NhsNumber, pointerRequest.RecordUrl, pointerRequest.RecordContentType, pointerRequest.TypeCode, pointerRequest.TypeDisplay);

            var newPointer = await CreatePointer(pointerRequest, pointer);

            return newPointer;
        }

        public async SystemTasks.Task<DocumentReference> CreatePointer(NrlsPointerRequest pointerRequest, DocumentReference pointer)
        {
            var pointerJson = new FhirJsonSerializer().SerializeToString(pointer);
            var content = new StringContent(pointerJson, Encoding.UTF8, "application/fhir+json");

            var newPointer = await new FhirConnector().RequestOne<DocumentReference>(BuildPostRequest(pointerRequest.Asid, pointerRequest.Interaction, content));

            return newPointer;
        }

        public async SystemTasks.Task<DocumentReference> DeletePointer(NrlsPointerRequest pointerRequest)
        {
            var pointer = await new FhirConnector().RequestOne<DocumentReference>(BuildDeleteRequest(pointerRequest.Asid, pointerRequest.Interaction, pointerRequest.PointerId));

            return pointer;
        }

        private CommandRequest BuildGetRequest(string asid, string interaction, string nhsNumber, string orgCode)
        {
            return BuildRequest(asid, interaction, null, nhsNumber, orgCode, HttpMethod.Get, null);
        }

        private CommandRequest BuildPostRequest(string asid, string interaction, HttpContent content)
        {
            
            return BuildRequest(asid, interaction, null, null, null, HttpMethod.Post, content);
        }

        private CommandRequest BuildDeleteRequest(string asid, string interaction, string pointerId)
        {
            return BuildRequest(asid, interaction, pointerId, null, null, HttpMethod.Delete, null);
        }

        private CommandRequest BuildRequest(string asid, string interaction, string resourceId, string nhsNumber, string orgCode, HttpMethod method, HttpContent content)
        {
            var command = new CommandRequest
            {
                BaseUrl = _documentReferenceUrlBase,
                ResourceId = resourceId,
                ResourceType = ResourceType.DocumentReference,
                SearchParams = GetParams(nhsNumber, orgCode),
                Method = method,
                Content = content
            };

            command.Headers.Add("fromASID", asid);
            command.Headers.Add("toASID", _spineAsid);
            command.Headers.Add("Ssp-InteractionID", interaction);
            command.Headers.Add("Ssp-Version", "1");
            command.Headers.Add("Ssp-TraceID", Guid.NewGuid().ToString());

            return command;
        }

        private SearchParams GetParams(string nhsNumber, string orgCode)
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

            return searchParams;
        }
    }
}
