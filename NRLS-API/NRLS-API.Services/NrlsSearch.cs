using Hl7.Fhir.Model;
using Microsoft.Extensions.Options;
using NRLS_API.Core.Exceptions;
using NRLS_API.Core.Factories;
using NRLS_API.Core.Interfaces.Services;
using NRLS_API.Models.Core;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace NRLS_API.Services
{
    public class NrlsSearch : FhirBase, INrlsSearch
    {
        private readonly IFhirSearch _fhirSearch;

        private readonly IFhirValidation _fhirValidation;

        public NrlsSearch(IOptions<NrlsApiSetting> nrlsApiSetting, IFhirSearch fhirSearch, IFhirValidation fhirValidation) : base(nrlsApiSetting)
        {
            _fhirSearch = fhirSearch;
            _fhirValidation = fhirValidation;
        }

        //public async Task<Resource> Get<T>(FhirRequest request) where T : Resource
        //{
        //    ValidateResource(request.StrResourceType);

        //    var id = request.IdParameter;

        //    if (id == null || string.IsNullOrEmpty(id.Item2) || request.QueryParameters.Count() > 1)
        //    {
        //        throw new HttpFhirException("Missing or Invalid _id parameter", OperationOutcomeFactory.CreateInvalidParameter("Invalid parameter: _id"), HttpStatusCode.BadRequest);
        //    }

        //    request.Id = id.Item2;


        //    var results = await _fhirSearch.Get<T>(request);

        //    var response = ParseRead(results, id.Item2);

        //    return response;
        //}

        /// <summary>
        /// Search for Documents or Get one by _id
        /// </summary>
        /// <remarks>
        /// As the NRLS is implemented with just a search and not read, to read a document the _id parameter is supplied
        /// </remarks>
        public async Task<Resource> Find<T>(FhirRequest request) where T : Resource
        {
            ValidateResource(request.StrResourceType);

            request.ProfileUri = _resourceProfile;

            var id = request.IdParameter;

            if (request.HasIdParameter)
            {

                if (string.IsNullOrEmpty(id))
                {
                    return OperationOutcomeFactory.CreateNotFound("");
                }

                if (request.QueryParameters.Count() > 1)
                {
                    throw new HttpFhirException("Invalid _id parameter", OperationOutcomeFactory.CreateInvalidParameter("Invalid parameter: _id"), HttpStatusCode.BadRequest);
                }

                request.Id = id;

                var results = await _fhirSearch.Get<T>(request);

                var response = ParseRead(results, id);

                return response;
            }

            var patient = request.QueryParameters.FirstOrDefault(x => x.Item1 == "subject");

            if (patient != null)
            {
               var validPatient =  _fhirValidation.ValidatePatientParameter(patient.Item2);

                if(validPatient != null)
                {
                    throw new HttpFhirException("Missing or Invalid patient parameter", validPatient, HttpStatusCode.BadRequest);
                }
            }
            else
            {
                throw new HttpFhirException("Missing or Invalid patient parameter", OperationOutcomeFactory.CreateInvalidParameter("Missing parameter: subject"), HttpStatusCode.BadRequest);
            }

            var custodian = request.QueryParameters.FirstOrDefault(x => x.Item1 == "custodian");

            if (custodian != null)
            {
                var validCustodian = _fhirValidation.ValidateCustodianParameter(custodian.Item2);

                if (validCustodian != null)
                {
                    throw new HttpFhirException("Missing or Invalid custodian parameter", validCustodian, HttpStatusCode.BadRequest);
                }
            }

            var type = request.QueryParameters.FirstOrDefault(x => x.Item1 == "type.coding");

            if (type != null)
            {
                var validType = _fhirValidation.ValidTypeParameter(type.Item2);

                if (validType != null)
                {
                    throw new HttpFhirException("Missing or Invalid type parameter", validType, HttpStatusCode.BadRequest);
                }
            }

            return await _fhirSearch.Find<T>(request);
        }
    }
}
