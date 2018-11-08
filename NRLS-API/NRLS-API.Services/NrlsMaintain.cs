using Hl7.Fhir.Model;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using NRLS_API.Core.Exceptions;
using NRLS_API.Core.Factories;
using NRLS_API.Core.Helpers;
using NRLS_API.Core.Interfaces.Services;
using NRLS_API.Core.Resources;
using NRLS_API.Models.Core;
using System;
using System.Collections.Generic;
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

        public NrlsMaintain(IOptionsSnapshot<ApiSetting> nrlsApiSetting, IFhirMaintain fhirMaintain, IFhirSearch fhirSearch, IMemoryCache memoryCache, IFhirValidation fhirValidation) : base(nrlsApiSetting, "NrlsApiSetting")
        {
            _fhirMaintain = fhirMaintain;
            _cache = memoryCache;
            _fhirSearch = fhirSearch;
            _fhirValidation = fhirValidation;
        }

        public async SystemTasks.Task<OperationOutcome> ValidateCreate<T>(FhirRequest request) where T : Resource
        {
            ValidateResource(request.StrResourceType);

            request.ProfileUri = _resourceProfile;

            // NRLS Layers of validation before Fhir Search Call
            var document = request.Resource as DocumentReference;

            //Pointer Validation
            var validProfile = _fhirValidation.ValidPointer(document);

            if (!validProfile.Success)
            {
                throw new HttpFhirException("Invalid NRLS Pointer", validProfile, HttpStatusCode.BadRequest);
            }


            //Now we need to do some additional validation on ODS codes && Master Ids
            //We need to use an external source (in reality yes but we are just going to do an internal query to fake ods & pointer search)

            if(document.MasterIdentifier != null)
            {
                var nhsNumber = _fhirValidation.GetSubjectReferenceId(document.Subject);
                var masterIdentifierRequest = NrlsPointerHelper.CreateMasterIdentifierSearch(request, document.MasterIdentifier, nhsNumber);
                var miPointers = await _fhirSearch.GetByMasterId<DocumentReference>(masterIdentifierRequest) as Bundle;

                if (miPointers.Entry.Count > 0)
                {
                    return OperationOutcomeFactory.CreateDuplicateRequest(document.MasterIdentifier);
                }
            }


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
                return OperationOutcomeFactory.CreateOrganizationNotFound(custodianOrgCode);
            }

            var authorOrgCode = _fhirValidation.GetOrganizationReferenceId(document.Author?.FirstOrDefault());
            var authorRequest = NrlsPointerHelper.CreateOrgSearch(request, authorOrgCode);
            var authors = await _fhirSearch.Find<Organization>(authorRequest) as Bundle;

            if (authors.Entry.Count == 0)
            {
                return OperationOutcomeFactory.CreateOrganizationNotFound(authorOrgCode);
            }

            return null;
        }

        public async SystemTasks.Task<Resource> ValidateConditionalUpdate(FhirRequest request)
        {
            if (!request.Resource.ResourceType.Equals(ResourceType.DocumentReference))
            {
                return OperationOutcomeFactory.CreateInvalidResource("relatesTo");
            }

            var document = request.Resource as DocumentReference;

            if (document.RelatesTo == null || document.RelatesTo.Count == 0)
            {
                return null;
            }

            var relatesTo = _fhirValidation.GetValidRelatesTo(document.RelatesTo);

            if (relatesTo.element == null)
            {
                return OperationOutcomeFactory.CreateInvalidResource(relatesTo.issue);
            }

            //Subject already validated during ValidateCreate
            //relatesTo Identifier already validated during ValidateCreate => validPointer
            var subjectNhsNumber = _fhirValidation.GetSubjectReferenceId(document.Subject);
            var pointerRequest = NrlsPointerHelper.CreateMasterIdentifierSearch(request, relatesTo.element.Target.Identifier, subjectNhsNumber);
            var pointers = await _fhirSearch.Find<DocumentReference>(pointerRequest) as Bundle;

            if (pointers.Entry.Count != 1)
            {
                //Cant find related document
                return OperationOutcomeFactory.CreateInvalidResource("relatesTo.target");
            }

            //Custodian already validated against incoming ASID during ValidateCreate
            var custodianOdsCode = _fhirValidation.GetOrganizationReferenceId(document.Custodian);

            var oldDocument = pointers.Entry.First().Resource as DocumentReference;

            if (oldDocument.Custodian == null || string.IsNullOrEmpty(oldDocument.Custodian.Reference) || oldDocument.Custodian.Reference != $"{FhirConstants.SystemODS}{custodianOdsCode}")
            {
                //related document does not have same custodian
                return OperationOutcomeFactory.CreateInvalidResource("relatesTo.target");
            }

            if(oldDocument.Status != DocumentReferenceStatus.Current)
            {
                //Only allowed to transition to superseded from current
                return OperationOutcomeFactory.CreateInvalidResource("relatesTo.code");
            }

            return oldDocument;
        }

        public async SystemTasks.Task<Resource> CreateWithoutValidation<T>(FhirRequest request) where T : Resource
        {

            SetMetaValues(request);

            var response = await _fhirMaintain.Create<T>(request);

            if (response == null)
            {
                return OperationOutcomeFactory.CreateInvalidResource("Unknown");
            }

            return response;
        }

        public async SystemTasks.Task<Resource> SupersedeWithoutValidation<T>(FhirRequest request, string oldDocumentId, string oldVersionId) where T : Resource
        {
            UpdateDefinition<BsonDocument> updates = null;
            FhirRequest updateRequest = null;

            SetMetaValues(request);
            BuildSupersede(oldDocumentId, oldVersionId, out updates, out updateRequest);

            Resource created;
            bool updated;

            try
            {
                (created, updated) = await _fhirMaintain.CreateWithUpdate<T>(request, updateRequest, updates);
            }
            catch
            {
                throw new HttpFhirException("Error Updating DocumentReference", OperationOutcomeFactory.CreateInternalError($"There has been an internal error when attempting to persist the DocumentReference. Please contact the national helpdesk quoting - {Guid.NewGuid()}"));
            }

            var response = created;

            if (response == null)
            {
                response = OperationOutcomeFactory.CreateInvalidResource("Unknown");
            }

            if (!updated)
            {
                response = OperationOutcomeFactory.CreateInvalidResource("relatesTo");
            }

            return response;
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

            request.ProfileUri = _resourceProfile;

            // NRLS Layers of validation before Fhir Delete Call
            var id = request.IdParameter;
            var identifier = request.IdentifierParameter;
            var subject = request.SubjectParameter;

            if (string.IsNullOrEmpty(id) && _fhirValidation.ValidateIdentifierParameter("identifier", identifier) != null && _fhirValidation.ValidatePatientParameter(subject) != null)
            {
                throw new HttpFhirException("Missing or Invalid _id parameter", OperationOutcomeFactory.CreateInvalidParameter("Invalid parameter: _id"), HttpStatusCode.BadRequest);
            }

            if (string.IsNullOrEmpty(id) && _fhirValidation.ValidateIdentifierParameter("identifier", identifier) == null && _fhirValidation.ValidatePatientParameter(subject) != null)
            {
                throw new HttpFhirException("Missing or Invalid subject parameter", OperationOutcomeFactory.CreateInvalidParameter("Invalid parameter: subject"), HttpStatusCode.BadRequest);
            }

            if (string.IsNullOrEmpty(id) && _fhirValidation.ValidateIdentifierParameter("identifier", identifier)  != null && _fhirValidation.ValidatePatientParameter(subject) == null)
            {
                throw new HttpFhirException("Missing or Invalid identifier parameter", OperationOutcomeFactory.CreateInvalidParameter("Invalid parameter: identifier"), HttpStatusCode.BadRequest);
            }


            request.Id = id;

            Resource document;

            if (!string.IsNullOrEmpty(id))
            {
                ObjectId mongoId;

                if (!ObjectId.TryParse(id, out mongoId))
                {
                    throw new HttpFhirException("Invalid _id parameter", OperationOutcomeFactory.CreateInvalidParameter("Invalid parameter", $"The Logical ID format does not apply to the given Logical ID - {id}"), HttpStatusCode.BadRequest);
                }

                document = await _fhirSearch.Get<T>(request);
            }
            else
            {
                document = await _fhirSearch.GetByMasterId<T>(request);
            }

            var documentResponse = ParseRead(document, id);

            if(documentResponse.ResourceType == ResourceType.Bundle)
            {
                var result = documentResponse as Bundle;

                if(!result.Total.HasValue || result.Total.Value < 1 || result.Entry.FirstOrDefault() == null)
                {
                    return OperationOutcomeFactory.CreateNotFound(id);
                }

                var orgDocument = result.Entry.FirstOrDefault().Resource as DocumentReference;

                var orgCode = _fhirValidation.GetOrganizationReferenceId(orgDocument.Custodian);

                var invalidAsid = InvalidAsid(orgCode, request.RequestingAsid, false);

                if (invalidAsid != null)
                {
                    return invalidAsid;
                }
                
            }
            else
            {
                return documentResponse as OperationOutcome;
            }

            bool deleted;

            if (!string.IsNullOrEmpty(id))
            {
                deleted = await _fhirMaintain.Delete<T>(request);

            }
            else
            {
                //Add identifier on the fly as it is not a standard search parameter
                request.AllowedParameters = request.AllowedParameters.Concat(new[] { "identifier" }).ToArray();

                deleted = await _fhirMaintain.DeleteConditional<T>(request);
            }

            if (!deleted)
            {
                return OperationOutcomeFactory.CreateNotFound(request.Id);
            }

            return OperationOutcomeFactory.CreateDelete(request.RequestUrl?.AbsoluteUri, request.AuditId);

        }

        public FhirRequest SetMetaValues(FhirRequest request)
        {

            var document = request.Resource as DocumentReference;

            //At present NRLS spec states updates are performed by delete and create so version will always be 1
            document.Meta = request.Resource.Meta ?? new Meta();
            document.Meta.LastUpdated = DateTime.UtcNow;
            document.Meta.VersionId = "1";
            document.Meta.Profile = new List<string> { _resourceProfile };

            //Overwrite indexed value as this should not be changed by clients
            document.Indexed = new DateTimeOffset(DateTime.UtcNow);

            request.Resource = document;

            return request;
        }

        public void BuildSupersede(string oldDocumentId, string oldVersion, out UpdateDefinition<BsonDocument> updates, out FhirRequest updateRequest)
        {
            int version = 0;
            int newVersion = 1;
            var validVersion = int.TryParse(oldVersion, out version);

            if (validVersion)
            {
                newVersion = version + 1;
            }

            if (!string.IsNullOrEmpty(oldVersion) && !validVersion)
            {
                throw new HttpFhirException("Bad update values", OperationOutcomeFactory.CreateInvalidResource("relatesTo"), HttpStatusCode.BadRequest);
            }

            updates = new UpdateDefinitionBuilder<BsonDocument>()
                .Set("status", DocumentReferenceStatus.Superseded.ToString().ToLowerInvariant())
                .Set("meta.lastUpdated", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz"))
                .Set("meta.versionId", $"{newVersion}");

            updateRequest = FhirRequest.Create(oldDocumentId, ResourceType.DocumentReference);
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

            return OperationOutcomeFactory.CreateInvalidResource(FhirConstants.HeaderFromAsid, "The Custodian ODS code is not affiliated with the sender ASID.");

        }

    }
}
