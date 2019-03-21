using Demonstrator.Core.Helpers;
using Demonstrator.Core.Interfaces.Services.Nrls;
using Demonstrator.Models.Core.Models;
using Demonstrator.Models.ViewModels.Base;
using Demonstrator.WebApp.Core.Configuration;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.NodeServices;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

//In reality all end points would be secured
namespace Demonstrator.WebApp.Controllers
{
    [MiddlewareFilter(typeof(ContentRequestedMiddlewarePipeline))]
    [Route("api/careconnect/[controller]")]
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
        [HttpGet("{documentId:regex(^[[A-Fa-f0-9-]]{{1,1024}}$)}")]
        //[ProducesResponseType(typeof(DocumentReference), 200)]
        public async Task<IActionResult> Get([FromServices] INodeServices nodeServices, string documentId)
        {
            var outputType = GetOutputType();

            var template = GetTemplate();
            var model = JsonConvert.SerializeObject(new { documentId = documentId });

            var data = await nodeServices.InvokeAsync<byte[]>("./Documents/Parsers/pdf", template, model);
            var result = data;


            if (outputType.ToLowerInvariant().Contains("fhir"))
            {
                var binary = new Binary
                {
                    ContentType = outputType,
                    Content = data
                };

                result = new FhirJsonSerializer().SerializeToBytes(binary);
            }

            return new FileContentResult(result, outputType);
        }

        private string GetTemplate()
        {
            var basePath = DirectoryHelper.GetBaseDirectory();

            var filePath = Path.Combine(basePath, "Documents", "care-plan.tmpl");

            var template = System.IO.File.ReadAllText(filePath);

            return template;
        }

        private string GetOutputType()
        {
            var outputType = "application/pdf";

            if (Request.Headers.ContainsKey(HeaderNames.Accept))
            {
                outputType = Request.Headers[HeaderNames.Accept];
            }

            return outputType;
        }

    }
}
