using Demonstrator.Models.ViewModels.Base;
using Demonstrator.Models.ViewModels.Epr;
using System.Threading.Tasks;

namespace Demonstrator.Core.Interfaces.Services.Epr
{
    public interface ICrisisPlanService
    {
        Task<CrisisPlanViewModel> GetForPatient(string nhsNumber, bool ifActive = true);

        Task<CrisisPlanViewModel> GetById(string planId);

        Task<CrisisPlanViewModel> SavePlan(CrisisPlanViewModel crisisPlan);

        Task<bool> DeletePlan(RequestViewModel request);
    }
}
