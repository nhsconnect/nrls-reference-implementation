using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Demonstrator.WebApp.Controllers
{
    [Route("api/[controller]")]
    public class NrlsController : Controller
    {
        /// <summary>
        /// Fetches a list of NRLS Pointers.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="nhsNumber"></param>  
        /// <returns>A list of NRLS Pointers (Constrained FHIR DocumentReference) for a patient specificed by the patient nhs number.</returns>
        /// <response code="200">Returns the NRLS Pointers</response>
        [HttpGet("{nhsNumber:int}")]
        [ProducesResponseType(typeof(List<DocumentReference>), 200)]
        public IActionResult Get(int nhsNumber)
        {
            //validate nhs number

            //Service to get NRLS Pointers for patient

            return Ok($"Get all NRLS Pointers for Patient {nhsNumber}");
        }

    }
}
