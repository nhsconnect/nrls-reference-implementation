using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using NRLS_API.Core.Exceptions;
using NRLS_API.Core.Factories;
using System.Net;
using System.Text;
using SystemTasks = System.Threading.Tasks;


namespace NRLS_API.WebApp.Core.Middlewares
{
    public class JsonExceptionMiddleware
    {
        private readonly IHostingEnvironment _env;

        public JsonExceptionMiddleware(IHostingEnvironment env)
        {
            _env = env;
        }

        public async SystemTasks.Task Invoke(HttpContext context)
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
                diagnostics = $"Message: {ex.Message}. StackTrace: {ex.StackTrace}";

                if (ex.InnerException != null)
                {
                    diagnostics = $"Message: {ex.Message}. StackTrace: {ex.StackTrace}. Inner Exception Stack Trace: {ex.InnerException.StackTrace}";
                }
            }

            var outcome = ooException != null ? ooException.OperationOutcome : OperationOutcomeFactory.CreateInternalError(diagnostics);

            var outcomeJson = new FhirJsonSerializer().SerializeToString(outcome);
            //context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(outcomeJson, Encoding.UTF8);
        }
    }
}
