using Microsoft.AspNetCore.Mvc;

namespace Demonstrator.WebApp.Controllers
{
    [Route("api/[controller]")]
    public class PersonnelController : Controller
    {
        // GET api/Personnel/AmbulanceTrust
        [HttpGet("{actorOrgName:regex(^[[A-Fa-f0-9]]{{1,1024}}$)}")]
        public IActionResult Get(string actorOrgName)
        {
            if (string.IsNullOrWhiteSpace(actorOrgName))
            {
                return BadRequest("Url Parameter actorOrgName is missing.");
            }

            //Service to get Personnel

            return Ok($"Get all Personnel for {actorOrgName}");
        }

    }
}
