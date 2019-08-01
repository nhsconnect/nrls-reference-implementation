using Demonstrator.Core.Interfaces.Services.Flows;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

//In reality all end points would be secured
namespace Demonstrator.WebApp.Controllers
{
    [Route("api/[controller]")]
    public class GenericSystemsController : Controller
    {
        private readonly IGenericSystemService _genericSystemService;
        private readonly IPersonnelService _personnelService;

        public GenericSystemsController(IGenericSystemService genericSystemService, IPersonnelService personnelService)
        {
            _genericSystemService = genericSystemService;
            _personnelService = personnelService;
        }

        // GET api/GenericSystems/5a82c6cecb969daa58d32cdk9
        [HttpGet("{systemId:regex(^[[A-Fa-f0-9]]{{1,1024}}$)}")]
        public async Task<IActionResult> Get(string systemId)
        {
            //Service to get GenericSystem
            var genericSystem = await _genericSystemService.GetById(systemId);

            if (genericSystem == null)
            {
                return NotFound($"GenericSystem of id {systemId} could not be found.");
            }

            return Ok(genericSystem);
        }

        // GET api/GenericSystems/5a82c6cecb969daa58d32cdk9/Personnel
        [HttpGet("{systemId:regex(^[[A-Fa-f0-9]]{{1,1024}}$)}/Personnel")]
        public async Task<IActionResult> GetPersonnel(string systemId)
        {
            //Service to get GenericSystem
            var personnel = await _personnelService.GetModelBySystemId(systemId);

            if (personnel == null)
            {
                return NotFound($"Personnel of id {systemId} could not be found.");
            }

            return Ok(personnel);
        }

        // GET api/GenericSystems
        [HttpGet("")]
        public async Task<IActionResult> Get()
        {
            //Service to get GenericSystem
            var genericSystem = await _genericSystemService.GetAll();

            return Ok(genericSystem);
        }

    }
}
