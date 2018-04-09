using Hl7.Fhir.Rest;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Net.Http.Headers;
using System.Text;
using SystemTasks = System.Threading.Tasks;


namespace NRLS_API.WebApp.Core.Middlewares
{
    public class FhirOuputMiddleware
    {
        private readonly RequestDelegate _next;

        public FhirOuputMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async SystemTasks.Task Invoke(HttpContext context)
        {
            context.Response.OnStarting(() =>
            {
                if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 300)
                {
                    var contentType = ContentType.XML_CONTENT_HEADER;

                    StringValues format;
                    if(context.Response.Headers.TryGetValue("Accept", out format))
                    {
                        if (format.ToArray().GetValue(0).ToString().Contains("json"))
                        {
                            contentType = ContentType.JSON_CONTENT_HEADER;
                        }
                    }

                    var header = new MediaTypeHeaderValue(contentType);
                    header.CharSet = Encoding.UTF8.WebName;

                    context.Response.ContentType = header.ToString();
                }

                return SystemTasks.Task.CompletedTask;
            });

            await this._next(context);
        }
    }

    public static class FhirOuputMiddlewareExtensions
    {
        public static IApplicationBuilder UseFhirOuputMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<FhirOuputMiddleware>();
        }
    }
}
