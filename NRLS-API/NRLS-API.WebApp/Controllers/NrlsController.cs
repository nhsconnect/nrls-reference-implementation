using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Mvc;
using NRLS_API.Core.Interfaces.Services;
using NRLS_API.Models.Core;
using NRLS_API.WebApp.Core.Configuration;

namespace NRLS_API.WebApp.Controllers
{
    [MiddlewareFilter(typeof(SspAuthorizationPipeline))]
    [Route("nrls/fhir/DocumentReference")]
    public class NrlsController : Controller
    {
        private readonly IFhirSearch _fhirSearch;
        private readonly IFhirMaintain _fhirMaintain;

        public NrlsController(IFhirSearch fhirSearch, IFhirMaintain fhirMaintain)
        {
            _fhirSearch = fhirSearch;
            _fhirMaintain = fhirMaintain;
        }

        /// <summary>
        /// Searches for the requested resource type.
        /// </summary>
        /// <returns>A FHIR Bundle Resource</returns>
        /// <response code="200">Returns the FHIR Resource</response>
        [ProducesResponseType(typeof(Resource), 200)]
        [HttpGet]
        public async Task<IActionResult> Search()
        {
            var request = FhirRequest.Create(null, ResourceType.DocumentReference, null, Request);

            var result = await _fhirSearch.Find<DocumentReference>(request);

            return Ok(result);
        }

        /// <summary>
        /// Gets a resource by the supplied id.
        /// </summary>
        /// <returns>A FHIR Resource</returns>
        /// <response code="200">Returns the FHIR Resource</response>
        [ProducesResponseType(typeof(Resource), 200)]
        [HttpGet("{id}")]
        public async Task<IActionResult> Read(string id)
        {
            var request = FhirRequest.Create(id, ResourceType.DocumentReference, null, Request);

            var result = await _fhirSearch.Get<DocumentReference>(request);

            if(result == null)
            {
                //return error
            }

            return Ok(result);
        }

        /// <summary>
        /// Creates and Persists a new record the requested resource type into a datastore.
        /// </summary>
        /// <returns>The created FHIR Resource</returns>
        /// <response code="201">Returns the FHIR Resource</response>
        [ProducesResponseType(typeof(Resource), 201)]
        [HttpPost()]
        public async Task<IActionResult> Create([FromBody]Resource resource)
        {
            var request = FhirRequest.Create(null, ResourceType.DocumentReference, resource, Request);

            var result = await _fhirMaintain.Create<DocumentReference>(request);

            if(result == null)
            {
                //return error
            }

            return Created($"{request.RequestUrl.AbsoluteUri}/{result.Id}", result);
        }

        // PUT fhir/DocumentReference/5
        [ProducesResponseType(typeof(Resource), 200)]
        [HttpPut("{id}")]
        public IActionResult Update(string id, [FromBody]Resource resource)
        {
            //not part of BETA : In BETA to update -> send a delete and then a create request
            return NotFound();
        }

        /// <summary>
        /// Deletes a record that was previously persisted into a datastore.
        /// </summary>
        /// <returns>The Empty Result</returns>
        /// <response code="204">Returns No Content</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var request = FhirRequest.Create(id, ResourceType.DocumentReference, null, Request);

            var result = await _fhirMaintain.Delete<DocumentReference>(request);

            if (!result)
            {
                //return error
            }

            return NoContent();
        }
    }
}
