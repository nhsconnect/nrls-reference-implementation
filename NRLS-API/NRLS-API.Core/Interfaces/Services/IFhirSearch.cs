using Hl7.Fhir.Model;
using NRLS_API.Models.Core;
using System.Threading.Tasks;

namespace NRLS_API.Core.Interfaces.Services
{
    public interface IFhirSearch
    {
        Task<Bundle> GetAsBundle<T>(FhirRequest request) where T : Resource;

        Task<T> Get<T>(FhirRequest request) where T : Resource;

        Task<Bundle> GetByMasterId<T>(FhirRequest request) where T : Resource;

        Task<Bundle> Find<T>(FhirRequest request, bool returnFirst = false) where T : Resource;
    }
}
