using Hl7.Fhir.Model;
using System.Collections.Generic;
using SystemTasks = System.Threading.Tasks;

namespace Demonstrator.Core.Interfaces.Services.Fhir
{
    public interface IDocumentReferenceServices
    {
        SystemTasks.Task<IEnumerable<DocumentReference>> GetPointers(int? nhsNumber = null, string orgCode = null);
        SystemTasks.Task<Bundle> GetPointersAsBundle(bool includeReferences, int? nhsNumber = null, string orgCode = null);
    }
}
