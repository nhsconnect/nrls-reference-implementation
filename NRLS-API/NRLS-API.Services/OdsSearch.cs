using Hl7.Fhir.Model;
using Microsoft.Extensions.Options;
using NRLS_API.Core.Interfaces.Services;
using NRLS_API.Models.Core;
using System.Threading.Tasks;

namespace NRLS_API.Services
{
    public class OdsSearch : FhirBase, IOdsSearch
    {
        private readonly IFhirSearch _fhirSearch;


        public OdsSearch(IOptionsSnapshot<ApiSetting> apiSetting, IFhirSearch fhirSearch) : base(apiSetting, "OdsApiSetting")
        {
            _fhirSearch = fhirSearch;
        }
        
        public async Task<Resource> Find(FhirRequest request)
        {
            ValidateResource(request.StrResourceType);

            request.ProfileUri = _resourceProfile;
         

            return await _fhirSearch.Find<Organization>(request);
        }

        public async Task<Resource> Get(FhirRequest request)
        {
            ValidateResource(request.StrResourceType);

            request.ProfileUri = _resourceProfile;

            return await _fhirSearch.Get<Organization>(request);
        }
    }
}
