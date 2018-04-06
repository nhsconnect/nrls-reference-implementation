using Hl7.Fhir.Model;
using NRLS_API.Models.Core;
using SystemTasks = System.Threading.Tasks;

namespace NRLS_API.Core.Interfaces.Services
{
    public interface IFhirMaintain
    {
        SystemTasks.Task<Resource> Create<T>(FhirRequest request) where T : Resource;

        SystemTasks.Task<bool> Delete<T>(FhirRequest request) where T : Resource;
    }
}
