using Demonstrator.Core.Interfaces.Services.Epr;
using Demonstrator.Models.ViewModels.Base;
using Demonstrator.Models.ViewModels.Epr;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

//In reality all end points would be secured
namespace Demonstrator.WebApp.Controllers
{
    [Route("api/[controller]")]
    public class EprController : FhirBaseController
    {
        private readonly ICrisisPlanService _crisisPlanService;

        public EprController(ICrisisPlanService crisisPlanService)
        {
            _crisisPlanService = crisisPlanService;
        }

        // GET api/Epr/CrisisPlan/Patient/000000000
        [HttpGet("CrisisPlan/Patient/{nhsNumber:regex(^[[0-9]]{{10}}$)}")]
        public async Task<IActionResult> GetForPatient(string nhsNumber)
        {
            var crisisPlan = await _crisisPlanService.GetForPatient(nhsNumber);

            if(crisisPlan == null)
            {
                return Ok("null");
            }

            return Ok(crisisPlan);
        }

        // GET api/Epr/CrisisPlan/5ac23c52511b5a4df6450ee8
        [HttpGet("CrisisPlan/{planId:regex(^[[A-Fa-f0-9]]{{1,1024}}$)}")]
        public async Task<IActionResult> GetById(string planId)
        {
            var crisisPlan = await _crisisPlanService.GetById(planId);

            if(crisisPlan == null)
            {
                return NotFound($"Crisis Plan by ID {planId} not found.");
            }

            return Ok(crisisPlan);
        }

        // POST api/Epr/CrisisPlan
        [HttpPost("CrisisPlan")]
        public async Task<IActionResult> Create([FromBody] CrisisPlanViewModel crisisPlan)
        {
            SetHeaders(crisisPlan);

           var newCrisisPlan = await _crisisPlanService.SavePlan(crisisPlan);

            return Created($"api/Epr/CrisisPlan/{newCrisisPlan.Id}", newCrisisPlan);
        }

        // PUT api/Epr/CrisisPlan/5ac23c52511b5a4df6450ee8
        [HttpPut("CrisisPlan/{planId:regex(^[[A-Fa-f0-9]]{{1,1024}}$)}")]
        public async Task<IActionResult> Update([FromBody] CrisisPlanViewModel crisisPlan)
        {
            SetHeaders(crisisPlan);

            //Update just creates a new one (versioning)
            var newCrisisPlan = await _crisisPlanService.SavePlan(crisisPlan);

            return Created($"api/Epr/CrisisPlan/{newCrisisPlan.Id}", newCrisisPlan);
        }

        // DELETE api/Epr/CrisisPlan/5ac23c52511b5a4df6450ee8
        [HttpDelete("CrisisPlan/{planId:regex(^[[A-Fa-f0-9]]{{1,1024}}$)}")]
        public async Task<IActionResult> Delete(string planId)
        {
            var request = RequestViewModel.Create(planId);

            SetHeaders(request);

            //This is a soft delete
            var isDeleted = await _crisisPlanService.DeletePlan(request);

            return Ok(isDeleted);
        }

    }
}
