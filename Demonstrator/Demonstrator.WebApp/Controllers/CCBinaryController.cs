using Demonstrator.Core.Exceptions;
using Demonstrator.Core.Factories;
using Demonstrator.Core.Helpers;
using Demonstrator.Core.Interfaces.Services.Nrls;
using Demonstrator.Models.Core.Models;
using Demonstrator.WebApp.Core.Configuration;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.NodeServices;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

//In reality all end points would be secured
namespace Demonstrator.WebApp.Controllers
{
    [Route("provider")]
    public class BinaryController : FhirBaseController
    {
        private readonly IPointerService _pointerService;
        private readonly ApiSetting _apiSettings;

        public BinaryController(IPointerService pointerService, IOptions<ApiSetting> apiSetting)
        {
            _pointerService = pointerService;
            _apiSettings = apiSetting.Value;
        }

        /// <summary>
        /// Fetches a single file.
        /// </summary>
        /// <param name="documentId">e.g. 5c5361b31ff30670e312198c</param>  
        /// <returns>A single file.</returns>
        /// <response code="200">Returns the file</response>
        [MiddlewareFilter(typeof(ProviderBinaryOutputMiddlewarePipeline))]
        [HttpGet("{providerOdsCode:regex(^[[A-Za-z0-9]]+$)}/fhir/STU3/careconnect/binary/{documentType}/{documentId}")]
        //[ProducesResponseType(typeof(DocumentReference), 200)]
        public async Task<IActionResult> Get([FromServices] INodeServices nodeServices, string documentId, string documentType)
        {
            //Not supporting 410 errors
            var regex = new Regex("^[A-Fa-f0-9-]{1,1024}$");

            var documentTypeTemplate = GetDocumentType(documentType);

            //TODO: check pointers cache
            if (documentTypeTemplate == null || string.IsNullOrWhiteSpace(documentId) || !regex.IsMatch(documentId))
            {
                throw new HttpFhirException("Not Found", OperationOutcomeFactory.CreateNotFound(documentId), HttpStatusCode.NotFound);
                //return NotFound(OperationOutcomeFactory.CreateNotFound(documentId));
            }


            //TODO: switch to other types
            var outputType = "application/pdf";

            var responseOutputType = GetOutputType();

            var template = GetTemplate(documentTypeTemplate);
            var model = JsonConvert.SerializeObject(new { documentId = documentId });

            var data = await nodeServices.InvokeAsync<byte[]>("./Documents/Parsers/pdf", template, model);
            var result = data;


            if (!string.IsNullOrEmpty(responseOutputType) && responseOutputType.ToLowerInvariant().Contains("fhir"))
            {
                var binary = new Binary
                {
                    ContentType = outputType,
                    Content = data
                };

                result = new FhirJsonSerializer().SerializeToBytes(binary);

                if (!responseOutputType.Contains("charset"))
                {
                    responseOutputType = $"{responseOutputType}; charset={Encoding.UTF8.WebName}";
                }
            }
            else
            {
                responseOutputType = outputType;
            }


            return new FileContentResult(result, responseOutputType);
        }

        private string GetDocumentType(string documentType)
        {
            if (string.IsNullOrWhiteSpace(documentType))
            {
                return null;
            }

            switch(documentType.ToUpperInvariant())
            {
                case "MHCP":
                    return "mental-health-crisis-plan.tmpl";
                case "CCP":
                    return "cancer-care-plan.tmpl";
                default:
                    return null;
            }
        }

        private string GetTemplate(string documentTypeTemplate)
        {
            var basePath = DirectoryHelper.GetBaseDirectory();

            var filePath = Path.Combine(basePath, "Documents", documentTypeTemplate);

            var template = System.IO.File.ReadAllText(filePath);

            return template;
        }

        private string GetOutputType()
        {

            if (Request.Headers.ContainsKey(HeaderNames.Accept))
            {
                return Request.Headers[HeaderNames.Accept];
            }

            return null;
        }

    }
}
