using Hl7.Fhir.Model;

namespace NRLS_API.Core.Interfaces.Helpers
{
    public interface IFhirResourceHelper
    {
        OperationOutcome ValidateResource<T>(T resource, string resourceSchema) where T : Resource;

        Resource GetResourceProfile(string profileUrl);

        ValueSet GetValueSet(string uri);
    }
}
