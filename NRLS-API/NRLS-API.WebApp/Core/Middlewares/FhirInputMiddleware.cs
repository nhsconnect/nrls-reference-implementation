using Hl7.Fhir.Rest;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using NRLS_API.Models.Helpers;
using System.Net.Http.Headers;
using System.Text;
using SystemTasks = System.Threading.Tasks;


namespace NRLS_API.WebApp.Core.Middlewares
{
    public class FhirInputMiddleware
    {
        private readonly RequestDelegate _next;

        public FhirInputMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async SystemTasks.Task Invoke(HttpContext context)
        {
            string formatParam = context.Request.QueryString.Value.GetParameters()?.GetParameter("_format");

            if (!string.IsNullOrEmpty(formatParam))
            {
                var accepted = ContentType.GetResourceFormatFromFormatParam(formatParam);
                if (accepted != ResourceFormat.Unknown)
                {
                    context.Request.Headers.Remove("Accept");

                    var acceptHeader = ContentType.XML_CONTENT_HEADER;

                    if (accepted == ResourceFormat.Json)
                    {
                        acceptHeader = ContentType.JSON_CONTENT_HEADER;
                    }

                    var header = new MediaTypeHeaderValue(acceptHeader);
                    header.CharSet = Encoding.UTF8.WebName;

                    context.Request.Headers.Remove("Accept");
                    context.Request.Headers.Add("Accept", new StringValues(header.ToString()));
                }
            }

            await this._next(context);
        }
    }

    public static class FhirInputMiddlewareExtensions
    {
        public static IApplicationBuilder UseFhirInputMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<FhirInputMiddleware>();
        }
    }
}
