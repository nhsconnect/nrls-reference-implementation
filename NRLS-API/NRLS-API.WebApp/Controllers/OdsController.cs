using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Mvc;
using NRLS_API.Core.Factories;
using NRLS_API.Core.Interfaces.Services;
using NRLS_API.Core.Resources;
using NRLS_API.Models.Core;

namespace NRLS_API.WebApp.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("nrls-ri/Organization")]
    public class OdsController : Controller
    {
        private readonly IOdsSearch _odsSearch;

        public OdsController(IOdsSearch odsSearch)
        {
            _odsSearch = odsSearch;
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
            var request = FhirRequest.Create(null, ResourceType.Organization, null, Request, null);

            var result = await _odsSearch.Find(request);

            return Ok(result);
        }

        /// <summary>
        /// Gets a single resource.
        /// </summary>
        /// <returns>A FHIR Resource</returns>
        /// <response code="200">Returns the FHIR Resource</response>
        [HttpGet("{odsCode}")]
        public async Task<IActionResult> Read(string odsCode)
        {
            var request = FhirRequest.Create(null, ResourceType.Organization, null, Request, null);

            request.QueryParameters = new List<Tuple<string, string>> { new Tuple<string, string>("identifier", $"{FhirConstants.SystemOrgCode}|{odsCode?.ToUpperInvariant()}") };

            var result = await _odsSearch.GetByQuery(request);

            if (result == null)
            {
                return NotFound(OperationOutcomeFactory.CreateNotFound(odsCode));
            }

            return Ok(result);
        }

    }
}
