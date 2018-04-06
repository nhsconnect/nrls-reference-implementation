using Demonstrator.Models.Nrls;
using Hl7.Fhir.Model;
using SystemTasks = System.Threading.Tasks;

namespace Demonstrator.Core.Interfaces.Services.Fhir
{
    public interface IDocumentReferenceServices
    {
        SystemTasks.Task<Bundle> GetPointersAsBundle(NrlsPointerRequest pointerRequest);

        SystemTasks.Task<DocumentReference> GenerateAndCreatePointer(NrlsPointerRequest pointerRequest);

        SystemTasks.Task<DocumentReference> CreatePointer(NrlsPointerRequest pointerRequest, DocumentReference pointer);

        SystemTasks.Task<DocumentReference> DeletePointer(NrlsPointerRequest pointerRequest);
    }
}
