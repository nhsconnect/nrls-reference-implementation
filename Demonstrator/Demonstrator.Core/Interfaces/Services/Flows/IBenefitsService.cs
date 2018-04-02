using Demonstrator.Models.ViewModels.Flows;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Demonstrator.Core.Interfaces.Services.Flows
{
    public interface IBenefitsService
    {
        Task<List<BenefitViewModel>> GetByIdList(IList<string> idList);
    }
}
