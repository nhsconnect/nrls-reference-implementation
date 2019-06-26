using System.Net;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NRLS_API.Core.Exceptions;
using NRLS_API.Core.Factories;
using NRLS_API.Core.Interfaces.Services;
using NRLS_API.Models.Core;

namespace NRLS_API.WebApp.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("nrls-ri/Patient")]
    public class PdsController : Controller
    {
        private readonly IPdsSearch _pdsSearch;
        private readonly IPdsMaintain _pdsMaintain;
        private readonly ApiSetting _nrlsApiSettings;

        public PdsController(IOptionsSnapshot<ApiSetting> apiSettings, IPdsSearch pdsSearch, IPdsMaintain pdsMaintain)
        {
            _pdsSearch = pdsSearch;
            _pdsMaintain = pdsMaintain;
            _nrlsApiSettings = apiSettings.Get("NrlsApiSetting");
        }

        /// <summary>
        /// Searches for the requested resource type.
        /// </summary>
        /// <returns>A FHIR Bundle Resource</returns>
        /// <response code="200">Returns the FHIR Resource</response>
        [ProducesResponseType(typeof(Resource), 200)]
        [HttpGet]
        public async Task<Resource> Search()
        {
            var request = FhirRequest.Create(null, ResourceType.Patient, null, Request, null);

            var result = await _pdsSearch.Find<Patient>(request);

            return result;
        }

        /// <summary>
        /// Gets a single resource.
        /// </summary>
        /// <returns>A FHIR Resource</returns>
        /// <response code="200">Returns the FHIR Resource</response>
        [HttpGet("{id}")]
        public async Task<Resource> Read(string id)
        {
            var request = FhirRequest.Create(id, ResourceType.Patient, null, Request, null);

            var result = await _pdsSearch.Get<Patient>(request);

            return result;
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
            //TODO: Remove temp code
            if (resource.ResourceType.Equals(ResourceType.OperationOutcome))
            {
                throw new HttpFhirException("Invalid Fhir Request", (OperationOutcome)resource, HttpStatusCode.BadRequest);
            }

            var request = FhirRequest.Create(null, ResourceType.Patient, resource, Request, null);

            var createIssue = await _pdsMaintain.Create<Patient>(request);

            if (createIssue.ResourceType == ResourceType.OperationOutcome)
            {
                return BadRequest(createIssue);
            }

            var response = OperationOutcomeFactory.CreateSuccess(ResourceType.Patient.ToString());

            var newResource = $"{_nrlsApiSettings.ResourceLocation}/{ResourceType.Patient}/{createIssue.Id}";

            return Created(newResource, response);
        }

        /// <summary>
        /// Deletes a record that was previously persisted into a datastore.
        /// </summary>
        /// <returns>The OperationOutcome</returns>
        /// <response code="200">Returns OperationOutcome</response>
        [HttpDelete("{logicalId}")]
        public async Task<IActionResult> Delete(string logicalId)
        {
            var request = FhirRequest.Create(logicalId, ResourceType.Patient, null, Request, null);

            var result = await _pdsMaintain.Delete<Patient>(request);

            if (result != null && result.Success)
            {
                //Assume success
                return Ok(result);
            }

            return NotFound(result);
        }

    }
}
