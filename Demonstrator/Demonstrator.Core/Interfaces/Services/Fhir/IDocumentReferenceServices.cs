using Hl7.Fhir.Model;
using System.Collections.Generic;
using SystemTasks = System.Threading.Tasks;

namespace Demonstrator.Core.Interfaces.Services.Fhir
{
    public interface IDocumentReferenceServices
    {
        SystemTasks.Task<Bundle> GetPointersAsBundle(string nhsNumber = null, string orgCode = null);
    }
}
