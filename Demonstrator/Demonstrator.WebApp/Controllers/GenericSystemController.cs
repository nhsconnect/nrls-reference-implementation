using Demonstrator.Models.ViewModels.Flows;
using Microsoft.AspNetCore.Mvc;

namespace Demonstrator.WebApp.Controllers
{
    [Route("api/[controller]")]
    public class GenericSystemController : Controller
    {
        // GET api/GenericSystem/fo49dle0-f0ro-dok4-sow9-eod84jf93ks0
        [HttpGet("{systemId:regex(^[[A-Fa-f0-9]]{{1,1024}}$)}")]
        public IActionResult Get(string systemId)
        {
            //Service to get GenericSystem
            var genericSystem = new GenericSystemViewModel
            {
                Id = "5a82c6cecb969daa58d32cdk9",
                Name = "Ambulance Service Call Handler",
                Asid = "200000000115",
                FModule = "Ambulance_Service_Call_Handler"
            };

            return Ok(genericSystem);
        }

    }
}
