using Demonstrator.Models.ViewModels.Flows;
using System.Threading.Tasks;

namespace Demonstrator.Core.Interfaces.Services.Flows
{
    public interface IPersonnelService
    {
        Task<PersonnelViewModel> GetById(string personnelId);

    }
}
