using Hl7.Fhir.Model;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using NRLS_API.Core.Exceptions;
using NRLS_API.Core.Factories;
using NRLS_API.Core.Interfaces.Services;
using NRLS_API.Core.Resources;
using NRLS_API.Models.Core;
using System.Linq;
using System.Net;
using SystemTasks = System.Threading.Tasks;

namespace NRLS_API.Services
{
    public class NrlsMaintain : FhirBase, INrlsMaintain
    {
        private readonly IFhirMaintain _fhirMaintain;
        private readonly IFhirSearch _fhirSearch;
        private readonly IMemoryCache _cache;

        public NrlsMaintain(IOptions<NrlsApiSetting> nrlsApiSetting, IFhirMaintain fhirMaintain, IFhirSearch fhirSearch, IMemoryCache memoryCache) : base(nrlsApiSetting)
        {
            _fhirMaintain = fhirMaintain;
            _cache = memoryCache;
            _fhirSearch = fhirSearch;
        }

        public async SystemTasks.Task<Resource> Create<T>(FhirRequest request) where T : Resource
        {
            var document = request.Resource as DocumentReference;

            var orgCode = document?.Custodian?.Reference?.Replace(FhirConstants.SystemODS, "");

            var invalidAsid = InvalidAsid(orgCode, request.RequestingAsid);

            if (invalidAsid != null)
            {
                return invalidAsid;
            }

            return await _fhirMaintain.Create<T>(request);
        }


        /// <summary>
        /// Delete a DocumentReference using the id value found in the request _id query parameter
        /// </summary>
        /// <remarks>
        /// First we do a search to get the document, then we check the incoming ASID associated OrgCode against the custodian on the document. 
        /// If valid we can delete.
        /// We use the FhirMaintain service and FhirSearch service to facilitate this
        /// </remarks>
        public async SystemTasks.Task<OperationOutcome> Delete<T>(FhirRequest request) where T : Resource
        {
            ValidateResource(request.StrResourceType);

            var id = request.IdParameter;

            if (string.IsNullOrEmpty(id))
            {
                throw new HttpFhirException("Missing or Invalid _id parameter", OperationOutcomeFactory.CreateInvalidParameter("Invalid parameter: _id"), HttpStatusCode.BadRequest);
            }

            request.Id = id;

            var document = await _fhirSearch.Get<T>(request);

            var documentResponse = ParseRead(document, id);

            if(documentResponse.ResourceType == ResourceType.Bundle)
            {
                var result = documentResponse as Bundle;

                if(result.Entry.FirstOrDefault() != null)
                {
                    var orgDocument = result.Entry.FirstOrDefault().Resource as DocumentReference;

                    var orgCode = orgDocument.Custodian?.Reference?.Replace(FhirConstants.SystemODS, "");

                    var invalidAsid = InvalidAsid(orgCode, request.RequestingAsid);

                    if (invalidAsid != null)
                    {
                        return invalidAsid;
                    }
                }
            }
            else
            {
                return documentResponse as OperationOutcome;
            }

            return await _fhirMaintain.Delete<T>(request);
        }

        private OperationOutcome InvalidAsid(string orgCode, string asid)
        {
            var map = _cache.Get<ClientAsidMap>(ClientAsidMap.Key);

            if (!string.IsNullOrEmpty(asid) && map != null && map.ClientAsids != null)
            {
                var asidMap = map.ClientAsids.FirstOrDefault(x => x.Key == asid);

                if(asidMap.Value != null && !string.IsNullOrEmpty(orgCode) && !string.IsNullOrEmpty(asidMap.Value.OrgCode) && asidMap.Value.OrgCode == orgCode)
                {
                    return null;
                }
            }

            return OperationOutcomeFactory.CreateInvalidResource(FhirConstants.HeaderFromAsid, "Provider system does not own DocumentReference resource.");
        }
    }
}
