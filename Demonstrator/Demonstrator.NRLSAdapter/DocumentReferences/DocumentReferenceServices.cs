using Demonstrator.Core.Interfaces.Services.Fhir;
using Demonstrator.Models.Core.Models;
using Demonstrator.NRLSAdapter.Helpers;
using Hl7.Fhir.Model;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Net;
using System.Text;
using SystemTasks = System.Threading.Tasks;

namespace Demonstrator.NRLSAdapter.DocumentReferences
{
    public class DocumentReferenceServices : IDocumentReferenceServices
    {
        private string _documentReferenceUrlBase;

        public DocumentReferenceServices(IOptions<NrlsApiSetting> nrlsApiSetting)
        {
            _documentReferenceUrlBase = $"{nrlsApiSetting.Value.ServerUrl}/DocumentReference";
        }

        public async SystemTasks.Task<IEnumerable<DocumentReference>> GetPointers(int? nhsNumber, string orgCode)
        {
            var pointers = await new FhirConnector().RequestMany<DocumentReference>(GetSearchUrl(false, nhsNumber, orgCode));

            return pointers;
        }

        public async SystemTasks.Task<Bundle> GetPointersAsBundle(bool includeReferences, int? nhsNumber, string orgCode)
        {
            var pointers = await new FhirConnector().RequestOne<Bundle>(GetSearchUrl(includeReferences, nhsNumber, orgCode));

            return pointers;
        }

        private string GetSearchUrl(bool includeReferences, int? nhsNumber, string orgCode)
        {
            var parameters = new StringBuilder();

            if (nhsNumber.HasValue)
            {
                parameters.Append($"&patient={WebUtility.UrlEncode(FhirConstants.SystemPDS)}{nhsNumber}");

            }

            if (!string.IsNullOrWhiteSpace(orgCode))
            {
                parameters.Append($"{(parameters.Length == 0 ? "" : "&")}custodian.identifier={WebUtility.UrlEncode(FhirConstants.SystemOrgCode)}|{orgCode}");
            }

            if (includeReferences)
            {
                parameters.Append($"{(parameters.Length == 0 ? "" : "&")}_include=DocumentReference:custodian&_include=DocumentReference:subject&_include=DocumentReference.author");
            }

            return GetDocumentReferenceUrl(null, parameters.ToString());
        }

        private string GetDocumentReferenceUrl(string id = null, string parameters = null)
        {
            return $"{_documentReferenceUrlBase}{("/" + id ?? "")}?_format=json{parameters}";
        }
    }
}
