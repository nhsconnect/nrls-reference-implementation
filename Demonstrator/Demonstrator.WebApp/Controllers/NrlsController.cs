using Demonstrator.Core.Interfaces.Services.Nrls;
using Demonstrator.Models.ViewModels.Base;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

//In reality all end points would be secured
namespace Demonstrator.WebApp.Controllers
{
    [Route("api/[controller]")]
    public class NrlsController : FhirBaseController
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
        [HttpGet("{nhsNumber:regex(^[[0-9]]{{10}}$)}")]
        [ProducesResponseType(typeof(List<DocumentReference>), 200)]
        public async Task<IActionResult> Get(string nhsNumber)
        {
            //validate nhs number

            var request = RequestViewModel.Create(nhsNumber);

            SetHeaders(request);

            var pointers = await _pointerService.GetPointers(request);

            return Ok(pointers);
        }

    }
}
