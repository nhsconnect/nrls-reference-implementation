using Demonstrator.Models.DataModels.Flows;
using Demonstrator.Models.ViewModels.Flows;
using System.Threading.Tasks;

namespace Demonstrator.Core.Interfaces.Services.Flows
{
    public interface IActorOrganisationService
    {
        Task<ActorOrganisationViewModel> GetById(string orgId);

        Task<ActorOrganisation> GetModelById(string orgId);
    }
}
