using Demonstrator.Core.Interfaces.Services.Nrls;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Demonstrator.WebApp.Controllers
{
    [Route("api/[controller]")]
    public class NrlsController : Controller
    {
        private readonly IPointerService _pointerService;

        public NrlsController(IPointerService pointerService)
        {
            _pointerService = pointerService;
        }

        /// <summary>
        /// Fetches a list of NRLS Pointers.
        /// </summary>
        /// <param name="nhsNumber"></param>  
        /// <returns>A list of NRLS Pointers (Constrained FHIR DocumentReference) for a patient specificed by the patient nhs number.</returns>
        /// <response code="200">Returns the NRLS Pointers</response>
        [HttpGet("{nhsNumber:int}")]
        [ProducesResponseType(typeof(List<DocumentReference>), 200)]
        public async Task<IActionResult> Get(int nhsNumber)
        {
            //validate nhs number

            var pointers = await _pointerService.GetPointers(nhsNumber, null);

            return Ok(pointers);
        }

    }
}
