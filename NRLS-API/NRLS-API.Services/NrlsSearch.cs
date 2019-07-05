using Hl7.Fhir.Model;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using NRLS_API.Core.Exceptions;
using NRLS_API.Core.Factories;
using NRLS_API.Core.Helpers;
using NRLS_API.Core.Interfaces.Services;
using NRLS_API.Core.Resources;
using NRLS_API.Models.Core;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace NRLS_API.Services
{
    public class NrlsSearch : FhirBase, INrlsSearch
    {
        private readonly IFhirSearch _fhirSearch;
        private readonly ISdsService _sdsService;
        private readonly IFhirValidation _fhirValidation;

        public NrlsSearch(IOptionsSnapshot<ApiSetting> apiSetting, IFhirSearch fhirSearch, ISdsService sdsService, IFhirValidation fhirValidation) : base(apiSetting, "NrlsApiSetting")
        {
            _fhirSearch = fhirSearch;
            _sdsService = sdsService;
            _fhirValidation = fhirValidation;
        }

        /// <summary>
        /// Get a DocumentReference by id
        /// </summary>
        /// <remarks>
        /// Get a single DocumentReference using the DocRef logical id
        /// </remarks>
        public async Task<Resource> Get(FhirRequest request)
        {
            ValidateResource(request.StrResourceType);

            request.ProfileUri = _resourceProfile;

            var result = await _fhirSearch.Get<DocumentReference>(request);

            return result;
        }

        /// <summary>
        /// Search for Documents or Get one by _id
        /// </summary>
        /// <remarks>
        /// As the NRLS is implemented with just a search and not read, to read a document the _id parameter is supplied
        /// </remarks>
        public async Task<Resource> Find(FhirRequest request)
        {
            ValidateResource(request.StrResourceType);

            request.ProfileUri = _resourceProfile;

            var id = request.IdParameter;

            //We are going to be strict hear for search and only proceed if we have valid parameters
            if(request.InvalidParameters.Count() > 0)
            {
                throw new HttpFhirException("Invalid parameters", OperationOutcomeFactory.CreateInvalidParameter($"Invalid parameter: {string.Join(", ", request.InvalidParameters)}"), HttpStatusCode.BadRequest);
            }

            //If we have an _id param it should be the only param so check for that here.
            if (request.HasIdParameter)
            {
                ObjectId mongoId;
                if (!ObjectId.TryParse(id, out mongoId))
                {
                    throw new HttpFhirException("Invalid _id parameter", OperationOutcomeFactory.CreateNotFound(id), HttpStatusCode.NotFound);
                }

                if (request.QueryParameters.Count() > 1)
                {
                    throw new HttpFhirException("Invalid _id parameter combination", OperationOutcomeFactory.CreateInvalidParameter("Invalid parameter: _id"), HttpStatusCode.BadRequest);
                }

                request.Id = id;

                var results = await _fhirSearch.GetAsBundle<DocumentReference>(request);

                var response = ParseRead(results, id);

                return response;
            }

            //If we are here then it is a standard search query
            var patient = request.QueryParameters.FirstOrDefault(x => x.Item1 == "subject");

            if (patient != null)
            {
                var invalidPatient = _fhirValidation.ValidatePatientParameter(patient.Item2);

                if (invalidPatient != null)
                {
                    throw new HttpFhirException("Missing or Invalid patient parameter", invalidPatient, HttpStatusCode.BadRequest);
                }
            }
            else
            {
                throw new HttpFhirException("Missing or Invalid patient parameter", OperationOutcomeFactory.CreateInvalidParameter("Missing parameter: subject"), HttpStatusCode.BadRequest);
            }

            var custodian = request.QueryParameters.FirstOrDefault(x => x.Item1 == "custodian");
            var useCustodianIdentifierValidation = false;

            if (custodian == null)
            {
                //temporarily also support the incorrectly spec'd custodian.identifier parameter
                //custodian is a reference type and should not be used in this way
                custodian = request.QueryParameters.FirstOrDefault(x => x.Item1 == "custodian.identifier");

                useCustodianIdentifierValidation = custodian != null;
            }

            if (custodian != null)
            {
                //temporarily also support the incorrectly spec'd custodian.identifier parameter
                var invalidCustodian = useCustodianIdentifierValidation ? _fhirValidation.ValidateCustodianIdentifierParameter(custodian.Item2) :
                                                                          _fhirValidation.ValidateCustodianParameter(custodian.Item2);

                if (invalidCustodian != null)
                {
                    throw new HttpFhirException("Missing or Invalid custodian parameter", invalidCustodian, HttpStatusCode.BadRequest);
                }

                var custodianOrgCode = useCustodianIdentifierValidation? _fhirValidation.GetOrganizationParameterIdentifierId(custodian.Item2) : 
                                                                         _fhirValidation.GetOrganizationParameterId(custodian.Item2);

                var custodianRequest = NrlsPointerHelper.CreateOrgSearch(request, custodianOrgCode);
                var custodians = await _fhirSearch.Find<Organization>(custodianRequest);

                if (custodians.Entry.Count == 0)
                {
                    var invalidOrg = OperationOutcomeFactory.CreateOrganizationNotFound(custodianOrgCode);

                    throw new HttpFhirException("Missing or Invalid custodian parameter", invalidOrg, HttpStatusCode.NotFound);
                }

                //This is now out of scope
                //var invalidOrgInteraction = ValidateOrganisationInteraction(request.RequestingAsid, custodianOrgCode, true);

                //if (invalidOrgInteraction != null)
                //{
                //    throw new HttpFhirException("Invalid Provider Request Exception", invalidOrgInteraction, HttpStatusCode.Unauthorized);
                //}
            }

            var type = request.QueryParameters.FirstOrDefault(x => x.Item1 == "type.coding");

            if (type != null)
            {
                var invalidType = _fhirValidation.ValidTypeParameter(type.Item2);

                if (invalidType != null)
                {
                    throw new HttpFhirException("Missing or Invalid type parameter", invalidType, HttpStatusCode.BadRequest);
                }
            }

            var summary = request.QueryParameters.FirstOrDefault(x => x.Item1 == "_summary");

            if (summary != null)
            {
                var validSummaryParams = new string[] { "subject", "_format", "_summary" };
                var invalidSummaryParams = request.QueryParameters.Where(x => !validSummaryParams.Contains(x.Item1));
                OperationOutcome invalidSummary = null;

                if (invalidSummaryParams.Any())
                {
                    var invalidSummaryParamsList = string.Join(", ", invalidSummaryParams.Select(x => $"{x.Item1}={x.Item2}"));

                    invalidSummary = OperationOutcomeFactory.CreateInvalidParameter("Invalid parameter", $"Unsupported search parameter - {invalidSummaryParamsList}");

                    throw new HttpFhirException("Missing or Invalid type parameter", invalidSummary, HttpStatusCode.BadRequest);
                }

                invalidSummary = _fhirValidation.ValidSummaryParameter(summary.Item2);

                if (invalidSummary != null)
                {
                    throw new HttpFhirException("Missing or Invalid type parameter", invalidSummary, HttpStatusCode.BadRequest);
                }            

                request.IsSummary = true;
            }

            return await _fhirSearch.Find<DocumentReference>(request);

         }

        private OperationOutcome ValidateOrganisationInteraction(string asid, string orgCode, bool isProviderCheck)
        {
            var providerInteractions = new string[] { FhirConstants.CreateInteractionId, FhirConstants.UpdateInteractionId, FhirConstants.DeleteInteractionId };

            var cache = _sdsService.GetFor(asid);

            if (cache != null)
            {

                var valid = false;

                if (!string.IsNullOrEmpty(orgCode) && !string.IsNullOrEmpty(cache.OdsCode) && cache.OdsCode == orgCode)
                {
                    valid = true;
                }

                if(isProviderCheck && (cache.Interactions == null || !cache.Interactions.Any(x => providerInteractions.Contains(x))))
                {
                    valid = false;
                }

                if (valid)
                {
                    return null;
                }
                
            }

            return OperationOutcomeFactory.CreateAccessDenied();

        }
    }
}
