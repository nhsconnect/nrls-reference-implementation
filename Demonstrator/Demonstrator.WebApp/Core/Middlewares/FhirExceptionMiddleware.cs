using Demonstrator.Core.Exceptions;
using Demonstrator.Core.Factories;
using Demonstrator.Core.Resources;
using Demonstrator.Models.Core.Models;
using Demonstrator.Models.Extensions;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using SystemTask = System.Threading.Tasks;

namespace Demonstrator.WebApp.Core.Middlewares
{
    public class FhirExceptionMiddleware
    {
        private readonly IHostingEnvironment _env;
        private ApiSetting _nrlsApiSettings;

        public FhirExceptionMiddleware(IHostingEnvironment env, IOptionsSnapshot<ApiSetting> nrlsApiSettings)
        {
            _env = env;
            _nrlsApiSettings = nrlsApiSettings.Get("NrlsApiSetting");
        }

        public async SystemTask.Task Invoke(HttpContext context)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            HttpFhirException ooException = null;

            var ex = context.Features.Get<IExceptionHandlerFeature>()?.Error;
            if (ex == null) return;

            if (ex.GetType() == typeof(HttpFhirException))
            {
                ooException = ex as HttpFhirException;
                context.Response.StatusCode = (int)ooException.StatusCode;
            }

            var diagnostics = "An unknown error has occured.";

            if (_env.IsDevelopment())
            {
                diagnostics = $"Message: {ex.Message}. StackTrace: {ex.StackTrace}.";

                if (ex.InnerException != null)
                {
                    diagnostics = $"{diagnostics}. Inner Exception Stack Trace: {ex.InnerException.StackTrace}";
                }
            }

            var outcome = ooException?.OperationOutcome != null ? ooException.OperationOutcome : OperationOutcomeFactory.CreateInternalError(diagnostics);

            //NRL default is XML but we are switching to JSON as majority req is for demo web apps
            string output;

            if (UseXmlOutput(context))
            {
                output = new FhirXmlSerializer().SerializeToString(outcome);
                output = Regex.Replace(output, @"((\s){1})(/>)", "/>", RegexOptions.Compiled);
                context.Response.ContentType = ContentType.XML_CONTENT_HEADER;
            }
            else
            {
                output = new FhirJsonSerializer().SerializeToString(outcome);
                context.Response.ContentType = ContentType.JSON_CONTENT_HEADER;
            }

            await context.Response.WriteAsync(output, Encoding.UTF8);
        }

        private bool UseXmlOutput(HttpContext context)
        {
            var parameters = context.Request.QueryString.Value.GetParameters();

            string formatParam = parameters?.GetParameter("_format");
            string acceptHeader = context.Request.Headers.FirstOrDefault(x => x.Key == HeaderNames.Accept).Value;
            string fhirDefault = context.Request.Headers.FirstOrDefault(x => x.Key == FhirConstants.HeaderXFhirDefault).Value;


            var xmlFormat = ValidContentType(formatParam) && formatParam.ToLowerInvariant().Contains("xml");
            var xmlAccept = ValidContentType(acceptHeader) && acceptHeader.ToLowerInvariant().Contains("xml");
            var xmlDefault = ValidContentType(fhirDefault) && fhirDefault.ToLowerInvariant().Contains("xml");

            return xmlFormat || xmlAccept || xmlDefault;
        }

        private bool ValidContentType(string contentType)
        {
            return !string.IsNullOrWhiteSpace(contentType) && _nrlsApiSettings.SupportedContentTypes.Contains(contentType);
        }
    }
}
