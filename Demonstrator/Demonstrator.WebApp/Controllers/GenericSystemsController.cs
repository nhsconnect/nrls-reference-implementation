using Demonstrator.Core.Interfaces.Services.Flows;
using Demonstrator.WebApp.Core.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

//In reality all end points would be secured
namespace Demonstrator.WebApp.Controllers
{
    [Route("api/[controller]")]
    public class GenericSystemsController : Controller
    {
        private readonly IGenericSystemService _genericSystemService;

        public GenericSystemsController(IGenericSystemService genericSystemService)
        {
            _genericSystemService = genericSystemService;
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

        // POST api/GenericSystems
        [HttpPost("")]
        public async Task<IActionResult> Get([FromBody] ObjectIdList systemIds)
        {
            //Service to get GenericSystem
            var genericSystem = await _genericSystemService.GetByIdList(systemIds.ObjectIds);

            return Ok(genericSystem);
        }

    }
}
