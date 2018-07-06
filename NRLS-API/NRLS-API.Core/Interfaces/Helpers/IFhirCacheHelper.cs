using Hl7.Fhir.Model;
using Hl7.Fhir.Specification.Source;

namespace NRLS_API.Core.Interfaces.Helpers
{
    public interface IFhirCacheHelper
    {
        IResourceResolver GetSource();

        Resource GetResourceProfile(string profileUrl);

        ValueSet GetValueSet(string uri);
    }
}
