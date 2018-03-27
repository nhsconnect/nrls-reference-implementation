using Demonstrator.Core.Interfaces.Services.Fhir;
using Demonstrator.Models.Core.Models;
using Demonstrator.NRLSAdapter.Helpers;
using Demonstrator.NRLSAdapter.Models;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Microsoft.Extensions.Options;
using System.Net;
using SystemTasks = System.Threading.Tasks;

namespace Demonstrator.NRLSAdapter.DocumentReferences
{
    public class DocumentReferenceServices : IDocumentReferenceServices
    {
        private string _documentReferenceUrlBase;

        public DocumentReferenceServices(IOptions<ExternalApiSetting> externalApiSetting)
        {
            _documentReferenceUrlBase = $"{externalApiSetting.Value.NrlsServerUrl}";
        }

        public async SystemTasks.Task<Bundle> GetPointersAsBundle(string nhsNumber, string orgCode)
        {
            var pointers = await new FhirConnector().RequestOne<Bundle>(BuildRequest(nhsNumber, orgCode));

            return pointers;
        }

        private CommandRequest BuildRequest(string nhsNumber, string orgCode)
        {
            var command = new CommandRequest
            {
                BaseUrl = _documentReferenceUrlBase,
                ResourceType = ResourceType.DocumentReference,
                SearchParams = GetParams(nhsNumber, orgCode)
            };

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
