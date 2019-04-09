using Demonstrator.Models.Nrls;
using Hl7.Fhir.Model;
using SystemTasks = System.Threading.Tasks;

namespace Demonstrator.Core.Interfaces.Services.Fhir
{
    public interface IDocumentsServices
    {
        SystemTasks.Task<Resource> GetPointerDocument(string fromASID, string fromODS, string toODS, string pointerUrl);
    }
}
