using Demonstrator.Models.Core.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Demonstrator.WebApp.Core.Middlewares
{
    public class JsonExceptionMiddleware
    {
        private readonly IHostingEnvironment _env;

        public JsonExceptionMiddleware(IHostingEnvironment env)
        {
            _env = env;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var ex = context.Features.Get<IExceptionHandlerFeature>()?.Error;
            if (ex == null) return;

            var error = new Issue {
                Details = "An error has occured.",
                SeverityCode = IssueSeverity.Fatal
            };

            if (_env.IsDevelopment())
            {
                error.Details = ex.Message;
                error.Diagnostics = ex.StackTrace;

                if (ex.InnerException != null)
                {
                    error.Details = $"{error.Details}";
                    error.Diagnostics = $"{error.Diagnostics} Inner Exception Stack Trace: {ex.InnerException.StackTrace}";
                }
            }


            context.Response.ContentType = "application/json";

            using (var writer = new StreamWriter(context.Response.Body))
            {
                new JsonSerializer().Serialize(writer, error);
                await writer.FlushAsync().ConfigureAwait(false);
            }
        }
    }
}
