using Hl7.Fhir.Model;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
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
    public class PdsMaintain : FhirBase, IPdsMaintain
    {
        private readonly IFhirMaintain _fhirMaintain;
        private readonly IFhirValidation _fhirValidation;

        public PdsMaintain(IOptionsSnapshot<ApiSetting> nrlsApiSetting, IFhirMaintain fhirMaintain, IFhirValidation fhirValidation) : base(nrlsApiSetting, "NrlsApiSetting")
        {
            _fhirMaintain = fhirMaintain;
            _fhirValidation = fhirValidation;
        }

        public async SystemTasks.Task<Resource> Create<T>(FhirRequest request) where T : Resource
        {
            ValidateResource(request.StrResourceType);

            request.ProfileUri = _resourceProfile;

            var patient = request.Resource as Patient;

            var validPatient = _fhirValidation.ValidProfile<Patient>(patient, null);

            if (validPatient != null && !validPatient.Success)
            {
                return validPatient;
            }

            var nhsNumberIdentifer = patient.Identifier?.FirstOrDefault(x => x.System == FhirConstants.SystemNhsNumber);

            var validIdentifer = _fhirValidation.ValidatePatientIdentifier(nhsNumberIdentifer);

            if(validIdentifer != null && !validIdentifer.Success)
            {
                return validIdentifer;
            }

            var response = await _fhirMaintain.Create<T>(request);

            if (response == null)
            {
                return OperationOutcomeFactory.CreateInvalidResource("Unknown");
            }

            return response;
        }


        /// <summary>
        /// Delete a Patient using the id value
        /// </summary>
        /// <remarks>
        /// If valid we can delete.
        /// We use the FhirMaintain service to facilitate this
        /// </remarks>
        public async SystemTasks.Task<OperationOutcome> Delete<T>(FhirRequest request) where T : Resource
        {
            ValidateResource(request.StrResourceType);

            request.ProfileUri = _resourceProfile;

            var idError = false;

            if (!string.IsNullOrEmpty(request.Id))
            {
                ObjectId mongoId;

                if (!ObjectId.TryParse(request.Id, out mongoId))
                {
                    idError = true;
                }
            }

            if (string.IsNullOrEmpty(request.Id))
            {
                idError = true;
            }

            if (idError)
            {
                throw new HttpFhirException("Invalid id parameter", OperationOutcomeFactory.CreateInvalidParameter("Invalid parameter", $"The Logical ID format does not apply to the given Logical ID - {request.Id}"), HttpStatusCode.BadRequest);
            }

            var deleted = await _fhirMaintain.Delete<T>(request);

            if (!deleted)
            {
                return OperationOutcomeFactory.CreateNotFound(ResourceType.Patient.ToString(), request.Id);
            }

            return OperationOutcomeFactory.CreateDelete(request.RequestUrl?.AbsoluteUri, request.AuditId);

        }

    }
}
