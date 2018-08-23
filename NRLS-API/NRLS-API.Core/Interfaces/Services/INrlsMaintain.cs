using Hl7.Fhir.Model;
using NRLS_API.Models.Core;
using SystemTasks = System.Threading.Tasks;

namespace NRLS_API.Core.Interfaces.Services
{
    public interface INrlsMaintain
    {
        SystemTasks.Task<OperationOutcome> ValidateCreate<T>(FhirRequest request) where T : Resource;

        SystemTasks.Task<Resource> ValidateConditionalUpdate(FhirRequest request);

        SystemTasks.Task<Resource> CreateWithoutValidation<T>(FhirRequest request) where T : Resource;

        SystemTasks.Task<Resource> SupersedeWithoutValidation<T>(FhirRequest request, string oldDocumentId, string oldVersion) where T : Resource;

        SystemTasks.Task<OperationOutcome> Delete<T>(FhirRequest request) where T : Resource;

        FhirRequest SetMetaValues(FhirRequest request, string oldVersion);
    }
}
