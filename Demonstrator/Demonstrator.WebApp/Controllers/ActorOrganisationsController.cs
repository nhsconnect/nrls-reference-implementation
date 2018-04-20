using Demonstrator.Core.Interfaces.Services.Flows;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

//In reality all end points would be secured
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
        public async Task<IActionResult> GetAll()
        {
             var actorOrgs = await _actorOrgService.GetAll();

            return Ok(actorOrgs);
        }

        // GET api/ActorOrganisations/5a82c6cecb969daa58d32dkfl9
        [HttpGet("{actorOrgId:regex(^[[A-Fa-f0-9]]{{1,1024}}$)}")]
        public async Task<IActionResult> GetOne(string actorOrgId)
        {
            var organisation = await _actorOrgService.GetById(actorOrgId);

            if (organisation == null)
            {
                return NotFound($"ActorOrganisation of id {actorOrgId} not found.");
            }

            return Ok(organisation);
        }

        // GET api/ActorOrganisations/5a82c6cecb969daa58d32dkfl9/Personnel
        [HttpGet("{actorOrgId:regex(^[[A-Fa-f0-9]]{{1,1024}}$)}/Personnel")]
        public async Task<IActionResult> GetPersonnel(string actorOrgId)
        {
            var organisation = await _actorOrgService.GetById(actorOrgId);

            if(organisation == null)
            {
                return NotFound($"ActorOrganisation of id {actorOrgId} not found.");
            }

            var personnel = await _actorOrgService.GetPersonnel(actorOrgId);

            return Ok(personnel);
        }

    }
}
