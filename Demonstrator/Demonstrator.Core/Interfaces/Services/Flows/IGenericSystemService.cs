using Demonstrator.Models.ViewModels.Flows;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Demonstrator.Core.Interfaces.Services.Flows
{
    public interface IGenericSystemService
    {
        Task<GenericSystemViewModel> GetById(string systemId);

        Task<IEnumerable<GenericSystemViewModel>> GetByIdList(List<string> systemIds);

        Task<IEnumerable<GenericSystemViewModel>> GetAll();
    }
}
