﻿using System.Net;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NRLS_API.Core.Exceptions;
using NRLS_API.Core.Factories;
using NRLS_API.Core.Interfaces.Services;
using NRLS_API.Core.Resources;
using NRLS_API.Models.Core;
using NRLS_API.WebApp.Core.Configuration;

namespace NRLS_API.WebApp.Controllers
{
    [MiddlewareFilter(typeof(ClientCertificateCheckPipeline))]
    [MiddlewareFilter(typeof(SspAuthorizationPipeline))]
    [Route("nrls-ri/DocumentReference")]
    public class NrlsController : Controller
    {
        private readonly INrlsSearch _nrlsSearch;
        private readonly INrlsMaintain _nrlsMaintain;
        private readonly ApiSetting _nrlsApiSettings;

        public NrlsController(IOptionsSnapshot<ApiSetting> apiSettings, INrlsSearch nrlsSearch, INrlsMaintain nrlsMaintain)
        {
            _nrlsSearch = nrlsSearch;
            _nrlsMaintain = nrlsMaintain;
            _nrlsApiSettings = apiSettings.Get("NrlsApiSetting");
        }

        /// <summary>
        /// Searches for the requested resource type.
        /// OR
        /// Gets a resource by the supplied _id search parameter.
        /// </summary>
        /// <returns>A FHIR Bundle Resource</returns>
        /// <response code="200">Returns the FHIR Resource</response>
        /// <param name="subject" required="true" dataType="reference" paramType="query" example="https%3A%2F%2Fdemographics.spineservices.nhs.uk%2FSTU3%2FPatient%2F2686033207">Who/what is the subject of the document</param>
        /// <param name="custodian" required="false" dataType="reference" paramType="query">Organization which maintains the document reference</param>
        /// <param name="type" required="false" dataType="token" paramType="query">Kind of document (SNOMED CT)</param>
        /// <param name="_id" required="false" dataType="token" paramType="query">The logical id of the resource</param>

        [ProducesResponseType(typeof(Resource), 200)]
        [HttpGet]
        public async Task<IActionResult> Search()
        {
            var request = FhirRequest.Create(null, ResourceType.DocumentReference, null, Request, null);

            var result = await _nrlsSearch.Find<DocumentReference>(request);

            if (result.ResourceType == ResourceType.OperationOutcome)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Gets a resource by the supplied id.
        /// </summary>
        /// <returns>A FHIR Resource</returns>
        /// <response code="200">Returns the FHIR Resource</response>
        //[ProducesResponseType(typeof(Resource), 200)]
        //[HttpGet]
        //public async Task<IActionResult> Read()
        //{
        //    //TODO: Update to reflect new ID parameter
        //    var request = FhirRequest.Create(null, ResourceType.DocumentReference, null, Request, null);

        //    var result = await _nrlsSearch.Get<DocumentReference>(request);

        //    if (result.ResourceType == ResourceType.OperationOutcome)
        //    {
        //        return NotFound(result);
        //    }

        //    return Ok(result);
        //}

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
            if(resource.ResourceType.Equals(ResourceType.OperationOutcome))
            {
                throw new HttpFhirException("Invalid Fhir Request", (OperationOutcome) resource, HttpStatusCode.BadRequest);
            }

            var request = FhirRequest.Create(null, ResourceType.DocumentReference, resource, Request, RequestingAsid());

            var result = await _nrlsMaintain.Create<DocumentReference>(request);

            if (result == null)
            {
                return BadRequest(OperationOutcomeFactory.CreateInvalidResource("Unknown"));
            }

            if (result.ResourceType == ResourceType.OperationOutcome)
            {
                return BadRequest(result);
            }

            var response = OperationOutcomeFactory.CreateSuccess();

            return Created($"{(_nrlsApiSettings.Secure ? "https" : "http")}{_nrlsApiSettings.BaseUrl}:{(_nrlsApiSettings.Secure ? _nrlsApiSettings.SecurePort : _nrlsApiSettings.DefaultPort)}{Request.Path}?_id={result.Id}", response);
        }

        // PUT fhir/DocumentReference/5
        [ApiExplorerSettings(IgnoreApi = true)]
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
        /// <returns>The OperationOutcome</returns>
        /// <response code="200">Returns OperationOutcome</response>
        [HttpDelete()]
        public async Task<IActionResult> Delete()
        {
            var request = FhirRequest.Create(null, ResourceType.DocumentReference, null, Request, RequestingAsid());

            var result = await _nrlsMaintain.Delete<DocumentReference>(request);

            if (result != null && result.Success)
            {
                //Assume success
                return Ok(result);
            }

            return NotFound(result);
        }

        private string RequestingAsid()
        {
            if (Request.Headers.ContainsKey(FhirConstants.HeaderFromAsid))
            {
                return Request.Headers[FhirConstants.HeaderFromAsid];
            }

            return null;
        }
    }
}
