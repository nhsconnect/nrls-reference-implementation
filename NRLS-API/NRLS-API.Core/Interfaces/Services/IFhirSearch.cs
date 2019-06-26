using Hl7.Fhir.Model;
using NRLS_API.Models.Core;
using System.Threading.Tasks;

namespace NRLS_API.Core.Interfaces.Services
{
    public interface IFhirSearch
    {
        Task<Resource> GetAsBundle<T>(FhirRequest request) where T : Resource;

        Task<Resource> Get<T>(FhirRequest request) where T : Resource;

        Task<Resource> GetByMasterId<T>(FhirRequest request) where T : Resource;

        Task<Resource> Find<T>(FhirRequest request) where T : Resource;
    }
}
