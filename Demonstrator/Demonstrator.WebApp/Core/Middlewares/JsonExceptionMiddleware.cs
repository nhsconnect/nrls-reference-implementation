using Demonstrator.Core.Exceptions;
using Demonstrator.Models.Core.Models;
using Demonstrator.Utilities;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Net;
using SystemTask = System.Threading.Tasks;

namespace Demonstrator.WebApp.Core.Middlewares
{
    public class JsonExceptionMiddleware
    {
        private readonly IHostingEnvironment _env;

        public JsonExceptionMiddleware(IHostingEnvironment env)
        {
            _env = env;
        }

        public async SystemTask.Task Invoke(HttpContext context)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var ex = context.Features.Get<IExceptionHandlerFeature>()?.Error;
            if (ex == null) return;

            var ooException = new HttpFhirException();
            OperationOutcome.IssueComponent fhirIssue = null;
            var issue = new Issue
            {
                Message = "An unkown error has occured."
            };

            if(ex.GetType() == typeof(HttpFhirException))
            {
                ooException = ex as HttpFhirException;
                fhirIssue = ooException.OperationOutcome?.Issue.FirstOrDefault();
            }

            if (fhirIssue != null)
            {
                issue.Message = fhirIssue?.Code.ToString();
                issue.Severity = EnumHelpers.GetEnum<IssueSeverity>(fhirIssue.Severity.ToString());
                issue.Details = fhirIssue.Details;
            }

            if (_env.IsDevelopment())
            {
                issue.Diagnostics = $"Message: {ex.Message}. StackTrace: {ex.StackTrace}.";

                if (ex.InnerException != null)
                {
                    issue.Diagnostics = $"{issue.Diagnostics}. Inner Exception Stack Trace: {ex.InnerException.StackTrace}";
                }

                if(fhirIssue != null)
                {
                    issue.Diagnostics = fhirIssue?.Diagnostics;
                }
            }

            context.Response.ContentType = "application/json";

            using (var writer = new StreamWriter(context.Response.Body))
            {
                new JsonSerializer().Serialize(writer, issue);
                await writer.FlushAsync().ConfigureAwait(false);
            }
        }
    }
}
