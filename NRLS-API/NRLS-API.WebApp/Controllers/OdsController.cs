using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Mvc;
using NRLS_API.Core.Interfaces.Services;
using NRLS_API.Models.Core;

namespace NRLS_API.WebApp.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("ods/fhir/Organization")]
    public class OdsController : Controller
    {
        private readonly IFhirSearch _fhirSearch;

        public OdsController(IFhirSearch fhirSearch)
        {
            _fhirSearch = fhirSearch;
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
            var request = FhirRequest.Create(null, ResourceType.Organization, null, Request, null);

            var result = await _fhirSearch.Find<Organization>(request);

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
            var request = FhirRequest.Create(id, ResourceType.Organization, null, Request, null);

            var result = await _fhirSearch.Find<Organization>(request);

            return result;
        }

    }
}
