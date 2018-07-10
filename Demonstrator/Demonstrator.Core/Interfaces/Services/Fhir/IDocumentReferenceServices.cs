using Demonstrator.Models.Nrls;
using Hl7.Fhir.Model;
using SystemTasks = System.Threading.Tasks;

namespace Demonstrator.Core.Interfaces.Services.Fhir
{
    public interface IDocumentReferenceServices
    {
        SystemTasks.Task<Resource> GetPointersBundle(NrlsPointerRequest pointerRequest);

        SystemTasks.Task<NrlsCreateResponse> GenerateAndCreatePointer(NrlsPointerRequest pointerRequest);

        SystemTasks.Task<NrlsCreateResponse> CreatePointer(NrlsPointerRequest pointerRequest, DocumentReference pointer);

        SystemTasks.Task<OperationOutcome> DeletePointer(NrlsPointerRequest pointerRequest);
    }
}
