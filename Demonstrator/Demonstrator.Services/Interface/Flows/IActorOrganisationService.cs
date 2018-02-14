using Demonstrator.Models.Core.Enums;
using Demonstrator.Models.ViewModels.Flows;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Demonstrator.Services.Interface.Flows
{
    public interface IActorOrganisationService
    {
        Task<IEnumerable<ActorOrganisationViewModel>> GetAll(ActorType aoType);
    }
}
