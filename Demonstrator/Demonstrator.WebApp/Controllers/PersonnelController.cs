using Demonstrator.Core.Interfaces.Services.Flows;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Demonstrator.WebApp.Controllers
{
    [Route("api/[controller]")]
    public class PersonnelController : Controller
    {
        private readonly IPersonnelService _personnelService;

        public PersonnelController(IPersonnelService personnelService)
        {
            _personnelService = personnelService;
        }

        // GET api/Personnel/5a8417338317338c8e0809e5
        [HttpGet("{personnelId:regex(^[[A-Fa-f0-9]]{{1,1024}}$)}")]
        public async Task<IActionResult> Get(string personnelId)
        {
            //Service to get Personnel
            var personnel = await _personnelService.GetById(personnelId);

            if(personnel == null)
            {
                return NotFound($"Personnel of id {personnelId} could not be found.");
            }

            return Ok(personnel);
        }

    }
}
