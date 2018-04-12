using Hl7.Fhir.Model;
using NRLS_API.Models.Core;
using SystemTasks = System.Threading.Tasks;

namespace NRLS_API.Core.Interfaces.Services
{
    public interface INrlsMaintain
    {
        SystemTasks.Task<Resource> Create<T>(FhirRequest request) where T : Resource;

        SystemTasks.Task<OperationOutcome> Delete<T>(FhirRequest request) where T : Resource;
    }
}
