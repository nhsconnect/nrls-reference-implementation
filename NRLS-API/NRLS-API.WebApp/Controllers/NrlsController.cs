using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Mvc;
using NRLS_API.Core.Interfaces.Services;
using NRLS_API.Models.Core;

namespace NRLS_API.WebApp.Controllers
{
    [Route("nrls/fhir/DocumentReference")]
    public class NrlsController : Controller
    {
        private readonly IFhirSearch _fhirSearch;

        public NrlsController(IFhirSearch fhirSearch)
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
            var request = FhirRequest.Create(null, ResourceType.DocumentReference, null, Request);

            var result = await _fhirSearch.Find<DocumentReference>(request);

            return result;
        }

        // GET fhir/DocumentReference/5
        [HttpGet("{resourceType}/{id}")]
        public string Read(string resourceType, string id)
        {
            return "value";
        }

        // POST fhir/DocumentReference
        [HttpPost("{resourceType}")]
        public void Create(string resourceType, [FromBody]Resource resource)
        {
            var request = FhirRequest.Create(null, ResourceType.DocumentReference, resource, Request);
        }

        // PUT fhir/DocumentReference/5
        [HttpPut("{resourceType}/{id}")]
        public void Update(string resourceType, string id, [FromBody]Resource resource)
        {
        }

        // DELETE fhir/DocumentReference/5
        [HttpDelete("{resourceType}/{id}")]
        public void Delete(string resourceType, string id)
        {
        }
    }
}
