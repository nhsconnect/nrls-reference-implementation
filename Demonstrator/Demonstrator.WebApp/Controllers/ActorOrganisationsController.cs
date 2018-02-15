using Demonstrator.Core.Interfaces.Services.Flows;
using Demonstrator.Models.Core.Enums;
using Demonstrator.Models.DataModels.Flows;
using Demonstrator.Models.ViewModels.Flows;
using Demonstrator.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demonstrator.WebApp.Controllers
{
    [Route("api/[controller]")]
    public class ActorOrganisationsController : Controller
    {
        private readonly IActorOrganisationService _actorOrgService;

        public ActorOrganisationsController(IActorOrganisationService actorOrgService)
        {
            _actorOrgService = actorOrgService;
        }

        // GET api/ActorOrganisations/Consumer
        [HttpGet("{actorType}")]
        public async Task<IActionResult> Get(ActorType? actorType)
        {
            if (actorType == null || !EnumHelpers.IsValidName<ActorType>(actorType.ToString()))
            {
                return BadRequest("Url Parameter is an invalid ActorType");
            }

             var actorOrgs = await _actorOrgService.GetAll((ActorType)actorType);

            return Ok(actorOrgs);
        }

        // GET api/ActorOrganisations/dje8f94id0sk4uf8s73jd95k/Personnel
        [HttpGet("{actorOrgId:regex(^[[A-Fa-f0-9]]{{1,1024}}$)}/Personnel")]
        public async Task<IActionResult> GetPersonnel(string actorOrgId)
        {

            //var actorOrgs = await _actorOrgService.GetAll((ActorType)actorType);

            var personnel = new List<PersonnelViewModel>
            {
                new PersonnelViewModel
                {
                    Id = "dk49skeif94kf8si4kw8sj8dki",
                    Name = "999 Call Handler",
                    ImageUrl = "...",
                    Context = "...",
                    SystemId = "5a82c6cecb969daa58d32cdk9",
                    ActorOrganisationId = "5a82c6cecb969daa58d32dkfl9",
                    UsesNrls = false
                },
                new PersonnelViewModel
                {
                    Id = "ob0f9fkdi37fjfos9jkwuis84jfjd",
                    Name = "Paramedic",
                    ImageUrl = "...",
                    Context = "...",
                    SystemId = "5a82c6cecb969daa58d32cdk9",
                    ActorOrganisationId = "5a82c6cecb969daa58d32dkfl9",
                    UsesNrls = true
                }
            };

            return Ok(personnel);
        }

    }
}
