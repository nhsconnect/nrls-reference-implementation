using Hl7.Fhir.Model;
using Microsoft.Extensions.Options;
using NRLS_API.Core.Interfaces.Services;
using NRLS_API.Models.Core;
using System.Linq;
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
        
        public Task<Bundle> Find(FhirRequest request)
        {
            ValidateResource(request.StrResourceType);

            request.ProfileUri = _resourceProfile;
         

            return _fhirSearch.Find<Organization>(request);
        }

        public async Task<Organization> GetByQuery(FhirRequest request)
        {
            ValidateResource(request.StrResourceType);

            request.ProfileUri = _resourceProfile;


            var bundle = await _fhirSearch.GetAsBundle<Organization>(request);

            if (bundle == null || (bundle.Total != 1))
            {
                return null;
            }

            return bundle.Entry.FirstOrDefault()?.Resource as Organization;
        }

        public async Task<Resource> Get(FhirRequest request)
        {
            ValidateResource(request.StrResourceType);

            request.ProfileUri = _resourceProfile;

            return await _fhirSearch.Get<Organization>(request);
        }
    }
}
