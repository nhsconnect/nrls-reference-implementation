using Demonstrator.Models.Core.Enums;
using Demonstrator.Models.ViewModels.Flows;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Demonstrator.Core.Interfaces.Services.Flows
{
    public interface IActorOrganisationService
    {
        Task<IEnumerable<ActorOrganisationViewModel>> GetAll(ActorType aoType);
    }
}
