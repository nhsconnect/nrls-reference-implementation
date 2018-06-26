using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Mvc;
using NRLS_API.Core.Interfaces.Services;

namespace NRLS_API.WebApp.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("nrls/fhir/metadata")]
    public class NrlsConformanceController : Controller
    {
        private readonly INrlsConformance _nrlsConformance;

        public NrlsConformanceController(INrlsConformance nrlsConformance)
        {
            _nrlsConformance = nrlsConformance;
        }

        /// <summary>
        /// Returns Server Conformance
        /// </summary>
        /// <returns>A FHIR Conformance Statement</returns>
        /// <response code="200">Returns the FHIR Conformance Statement</response>
        [ProducesResponseType(typeof(Resource), 200)]
        [HttpGet()]
        public IActionResult Conformance()
        {
            var conformance = _nrlsConformance.GetConformance();

            return Ok(conformance);
        }


    }
}
