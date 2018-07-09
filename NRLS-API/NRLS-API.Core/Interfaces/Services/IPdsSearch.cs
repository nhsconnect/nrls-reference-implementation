using Hl7.Fhir.Model;
using NRLS_API.Models.Core;
using System.Threading.Tasks;

namespace NRLS_API.Core.Interfaces.Services
{
    public interface IPdsSearch : IApiSearch
    {
        Task<Resource> Get<T>(FhirRequest request) where T : Resource;
    }
}
