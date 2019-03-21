using Demonstrator.Models.ViewModels.Base;
using Demonstrator.Models.ViewModels.Nrls;
using Hl7.Fhir.Model;
using System.Collections.Generic;
using SystemTasks = System.Threading.Tasks;

namespace Demonstrator.Core.Interfaces.Services.Nrls
{
    public interface IPointerService
    {
        SystemTasks.Task<IEnumerable<PointerViewModel>> GetPointers(RequestViewModel request);

        PointerViewModel GetCachedPointer(string nhsNumber, string pointerId);

        SystemTasks.Task<Binary> GetPointerDocument(string pointerUrl);
    }
}
