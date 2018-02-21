using Demonstrator.Core.Interfaces.Services.Flows;
using Demonstrator.Models.ViewModels.Flows;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
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

        // GET api/ActorOrganisations
        [HttpGet("")]
        public async Task<IActionResult> Get()
        {
             var actorOrgs = await _actorOrgService.GetAll();

            return Ok(actorOrgs);
        }

        // GET api/ActorOrganisations/5a82c6cecb969daa58d32dkfl9/Personnel
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
                    SystemIds = new List<string>{"5a82c6cecb969daa58d32cdk9" },
                    ActorOrganisationId = "5a82c6cecb969daa58d32dkfl9",
                    UsesNrls = false
                },
                new PersonnelViewModel
                {
                    Id = "ob0f9fkdi37fjfos9jkwuis84jfjd",
                    Name = "Paramedic",
                    ImageUrl = "...",
                    Context = "...",
                    SystemIds = new List<string>{"5a82c6cecb969daa58d32cdk9" },
                    ActorOrganisationId = "5a82c6cecb969daa58d32dkfl9",
                    UsesNrls = true
                }
            };

            return Ok(personnel);
        }

    }
}
