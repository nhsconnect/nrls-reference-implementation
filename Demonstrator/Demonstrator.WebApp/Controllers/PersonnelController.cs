using Demonstrator.Models.ViewModels.Flows;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Demonstrator.WebApp.Controllers
{
    [Route("api/[controller]")]
    public class PersonnelController : Controller
    {
        // GET api/Personnel/5a8417338317338c8e0809e5
        [HttpGet("{actorOrgName:regex(^[[A-Fa-f0-9]]{{1,1024}}$)}")]
        public IActionResult Get(string actorOrgName)
        {
            if (string.IsNullOrWhiteSpace(actorOrgName))
            {
                return BadRequest("Url Parameter actorOrgName is missing.");
            }

            //Service to get Personnel
            var personnel = new PersonnelViewModel
            {
                Id = "5a8417338317338c8e0809e5",
                Name = "999 Call Handler",
                ImageUrl = "...",
                Context = "...",
                UsesNrls = true,
                SystemIds = new List<string> { "5a82c6cecb969daa58d32cdk9" },
                ActorOrganisationId = "5a82f9ffcb969daa58d33377"
            };

            return Ok(personnel);
        }

    }
}
