using System.Linq;
using System.Net;
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
    [MiddlewareFilter(typeof(SpineAuthorizationPipeline))]
    [Route("nrls-ri/SSP")]
    public class SspController : Controller
    {
        private readonly INrlsSearch _nrlsSearch;
        private readonly INrlsMaintain _nrlsMaintain;
        private readonly ApiSetting _nrlsApiSettings;

        public SspController(IOptionsSnapshot<ApiSetting> apiSettings, INrlsSearch nrlsSearch, INrlsMaintain nrlsMaintain)
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
        public async Task<IActionResult> Forward()
        {

            //TODO: sds looup = mapping file
            //TODO: ssl check
            //TODO: url check
            //TODO: headers check (asid, auth, etc)
            //TODO: audit
            var request = FhirRequest.Create(null, ResourceType.DocumentReference, null, Request, RequestingAsid());

            var result = await _nrlsSearch.Find<DocumentReference>(request);

            if (result.ResourceType == ResourceType.OperationOutcome)
            {
                return NotFound(result);
            }

            return Ok(result);
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
