using Hl7.Fhir.Model;
using NRLS_API.Models.Core;
using System.Collections.Generic;

namespace NRLS_API.Core.Interfaces.Services
{
    public interface IFhirValidation
    {
        OperationOutcome ValidProfile<T>(T resource, string customProfile) where T : Resource;

        OperationOutcome ValidPointer(DocumentReference pointer);

        OperationOutcome ValidTypeParameter(string type);

        OperationOutcome ValidSummaryParameter(string summary);

        OperationOutcome ValidatePatientParameter(string parameterVal);

        OperationOutcome ValidateCustodianParameter(string parameterVal);

        OperationOutcome ValidateCustodianIdentifierParameter(string parameterVal);

        OperationOutcome ValidateIdentifierParameter(string paramName, string parameterVal);

        string GetOrganizationParameterId(string parameterVal);

        string GetOrganizationParameterIdentifierId(string parameterVal);

        string GetOrganizationReferenceId(ResourceReference reference);

        string GetSubjectReferenceId(ResourceReference reference);

        (DocumentReference.RelatesToComponent element, string issue) GetValidRelatesTo(IList<DocumentReference.RelatesToComponent> relatesTo);

        DocumentReferenceStatus? GetValidStatus(DocumentReferenceStatus? status);
    }
}
