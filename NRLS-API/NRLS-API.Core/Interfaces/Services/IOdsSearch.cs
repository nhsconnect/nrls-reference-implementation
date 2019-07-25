
using Hl7.Fhir.Model;
using NRLS_API.Models.Core;
using System.Threading.Tasks;

namespace NRLS_API.Core.Interfaces.Services
{
    public interface IOdsSearch : IApiSearch
    {
        Task<Organization> GetByQuery(FhirRequest request);
    }
}
