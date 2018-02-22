using Demonstrator.Models.ViewModels.Flows;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Demonstrator.Core.Interfaces.Services.Flows
{
    public interface IActorOrganisationService
    {
        Task<IEnumerable<ActorOrganisationViewModel>> GetAll();

        Task<ActorOrganisationViewModel> GetById(string orgId);

        Task<IEnumerable<PersonnelViewModel>> GetPersonnel(string orgId);
    }
}
