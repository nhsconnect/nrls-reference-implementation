using Hl7.Fhir.Model;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using NRLS_API.Core.Exceptions;
using NRLS_API.Core.Factories;
using NRLS_API.Core.Helpers;
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
        private readonly IFhirValidation _fhirValidation;

        public NrlsMaintain(IOptions<NrlsApiSetting> nrlsApiSetting, IFhirMaintain fhirMaintain, IFhirSearch fhirSearch, IMemoryCache memoryCache, IFhirValidation fhirValidation) : base(nrlsApiSetting)
        {
            _fhirMaintain = fhirMaintain;
            _cache = memoryCache;
            _fhirSearch = fhirSearch;
            _fhirValidation = fhirValidation;
        }

        public async SystemTasks.Task<Resource> Create<T>(FhirRequest request) where T : Resource
        {
            ValidateResource(request.StrResourceType);


            // NRLS Layers of validation before Fhir Search Call
            var document = request.Resource as DocumentReference;

            //Pointer Validation
            var validProfile = _fhirValidation.ValidPointer(document);

            if (!validProfile.Success)
            {
                throw new HttpFhirException("Invalid NRLS Pointer", validProfile, HttpStatusCode.BadRequest);
            }


            //Now we need to do some additional validation on ODS codes
            //We need to use an external source (in reality yes but we are just going to do an internal query to fake ods search)

            var custodianOrgCode = _fhirValidation.GetOrganizationReferenceId(document.Custodian);

            var invalidAsid = InvalidAsid(custodianOrgCode, request.RequestingAsid, true);

            if (invalidAsid != null)
            {
                return invalidAsid;
            }

            var custodianRequest = NrlsPointerHelper.CreateOrgSearch(request, custodianOrgCode);
            var custodians = await _fhirSearch.Find<Organization>(custodianRequest) as Bundle;

            if (custodians.Entry.Count == 0)
            {
                return OperationOutcomeFactory.CreateInvalidResource(FhirConstants.HeaderFromAsid, "Provider system does not own DocumentReference resource.");
            }

            //var authorOrgCode = _fhirValidation.GetOrganizationReferenceId(document.Author?.FirstOrDefault());
            //var authorRequest = NrlsPointerHelper.CreateOrgSearch(request, authorOrgCode);
            //var authors = await _fhirSearch.Find<Organization>(authorRequest) as Bundle;

            //if (authors.Entry.Count == 0)
            //{
            //    return OperationOutcomeFactory.CreateInvalidResource(FhirConstants.HeaderFromAsid, "Provider system does not own DocumentReference resource.");
            //}

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


            // NRLS Layers of validation before Fhir Delete Call
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

                    var orgCode = _fhirValidation.GetOrganizationReferenceId(orgDocument.Custodian);

                    var invalidAsid = InvalidAsid(orgCode, request.RequestingAsid, false);

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

        private OperationOutcome InvalidAsid(string orgCode, string asid, bool isCreate)
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

            if (isCreate)
            {
                return OperationOutcomeFactory.CreateInvalidResource(FhirConstants.HeaderFromAsid, "The Custodian ODS code is not affiliated with the sender ASID.");
            }

            return OperationOutcomeFactory.CreateInvalidResource(FhirConstants.HeaderFromAsid, "Provider system does not own DocumentReference resource.");
        }

    }
}
