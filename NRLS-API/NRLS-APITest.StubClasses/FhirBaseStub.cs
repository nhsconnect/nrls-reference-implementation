using Hl7.Fhir.Model;
using Microsoft.Extensions.Options;
using NRLS_API.Models.Core;
using NRLS_API.Services;

namespace NRLS_APITest.StubClasses
{
    public class FhirBaseStub : FhirBase
    {
        public FhirBaseStub(IOptionsSnapshot<ApiSetting> options) : base(options, "NrlsApiSetting") { }

        public void ValidateResourceStub(string resourceType)
        {
            ValidateResource(resourceType);
        }

        public Resource ParseReadStub<T>(T results, string id) where T : Resource
        {
            return ParseRead<T>(results, id);
        }
    }
}
