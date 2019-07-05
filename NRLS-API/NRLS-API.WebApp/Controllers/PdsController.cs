﻿using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Mvc;
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

        public PdsController(IPdsSearch pdsSearch)
        {
            _pdsSearch = pdsSearch;
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
            var request = FhirRequest.Create(null, ResourceType.Patient, null, Request, null);

            var result = await _pdsSearch.Find(request);

            return Ok(result);
        }

        /// <summary>
        /// Gets a single resource.
        /// </summary>
        /// <returns>A FHIR Resource</returns>
        /// <response code="200">Returns the FHIR Resource</response>
        [HttpGet("{id}")]
        public async Task<IActionResult> Read(string id)
        {
            var request = FhirRequest.Create(id, ResourceType.Patient, null,  Request, null);

            var result = await _pdsSearch.Get(request);

            if (result == null)
            {
                return NotFound(OperationOutcomeFactory.CreateNotFound(id));
            }

            return Ok(result);
        }

    }
}
