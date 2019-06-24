using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;

namespace DemonstratorTest.Data
{
    public static class FhirResourceExtensions
    {
        public static string AsJsonString(this Resource resource)
        {
            return new FhirJsonSerializer().SerializeToString(resource);
        }
    }
}
