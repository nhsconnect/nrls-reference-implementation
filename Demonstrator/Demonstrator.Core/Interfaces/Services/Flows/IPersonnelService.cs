using Demonstrator.Models.DataModels.Flows;
using Demonstrator.Models.ViewModels.Flows;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Demonstrator.Core.Interfaces.Services.Flows
{
    public interface IPersonnelService
    {
        Task<IEnumerable<PersonnelViewModel>> GetAll();

        Task<PersonnelViewModel> GetById(string personnelId);

        Task<Personnel> GetModelById(string personnelId);

        Task<PersonnelViewModel> GetModelBySystemId(string systemId);
    }
}
