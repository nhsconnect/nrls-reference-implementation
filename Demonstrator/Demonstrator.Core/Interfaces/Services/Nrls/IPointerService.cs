using Demonstrator.Models.ViewModels.Nrls;
using System.Collections.Generic;
using SystemTasks = System.Threading.Tasks;

namespace Demonstrator.Core.Interfaces.Services.Nrls
{
    public interface IPointerService
    {
        SystemTasks.Task<IEnumerable<PointerViewModel>> GetPointers(string nhsNumber);
    }
}
