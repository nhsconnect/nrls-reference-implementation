using Hl7.Fhir.Model;

namespace NRLS_API.Core.Interfaces.Services
{
    public interface IFhirValidation
    {
        OperationOutcome ValidProfile<T>(T resource, string customProfile) where T : Resource;

        OperationOutcome ValidPointer(DocumentReference pointer);

        OperationOutcome ValidatePatientParameter(string parameterVal);

        OperationOutcome ValidateCustodianParameter(string parameterVal);

        string GetOrganizationReferenceId(ResourceReference reference);
    }
}
