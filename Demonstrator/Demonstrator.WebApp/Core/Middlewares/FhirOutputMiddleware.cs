using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System;
using System.Threading.Tasks;

namespace Demonstrator.WebApp.Core.Middlewares
{
    public class FhirOutputMiddleware
    {
        private readonly RequestDelegate _next;

        public FhirOutputMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Response.OnStarting(state =>
            {

                var httpContext = (HttpContext)state;

                httpContext.Response.Headers.Remove(HeaderNames.CacheControl);
                httpContext.Response.Headers.Add(HeaderNames.CacheControl, "no-store");


                return Task.FromResult(0);

            }, context);

            await _next(context);
        }
    }

    public static class FhirOutputMiddlewareExtension
    {
        public static IApplicationBuilder UseFhirOutputMiddleware(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<FhirOutputMiddleware>();
        }
    }
}
