using Hl7.Fhir.Rest;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using NRLS_API.Core.Exceptions;
using NRLS_API.Core.Factories;
using NRLS_API.Models.Core;
using NRLS_API.Models.Extensions;
using System.IO;
using System.Linq;
using System.Net;
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
