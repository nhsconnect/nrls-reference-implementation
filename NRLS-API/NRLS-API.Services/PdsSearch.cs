using Hl7.Fhir.Model;
using Microsoft.Extensions.Options;
using NRLS_API.Core.Interfaces.Services;
using NRLS_API.Models.Core;
using System.Threading.Tasks;

namespace NRLS_API.Services
{
    public class PdsSearch : FhirBase, IPdsSearch
    {
        private readonly IFhirSearch _fhirSearch;


        public PdsSearch(IOptionsSnapshot<ApiSetting> apiSetting, IFhirSearch fhirSearch) : base(apiSetting, "PdsApiSetting")
        {
            _fhirSearch = fhirSearch;
        }
        
        public async Task<Resource> Find<T>(FhirRequest request) where T : Resource
        {
            ValidateResource(request.StrResourceType);

            request.ProfileUri = _resourceProfile;
         

            return await _fhirSearch.Find<T>(request);
        }

        public async Task<Resource> Get<T>(FhirRequest request) where T : Resource
        {
            ValidateResource(request.StrResourceType);

            request.ProfileUri = _resourceProfile;

            return await _fhirSearch.Get<T>(request);
        }
    }
}
