using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Demonstrator.WebApp.Core.Middlewares
{
    public class ProviderBinaryOutputMiddleware
    {
        private readonly RequestDelegate _next;

        public ProviderBinaryOutputMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Response.OnStarting(state => {

                var httpContext = (HttpContext)state;

                var filename = Guid.NewGuid().ToString("N");

                try
                {
                    filename = httpContext.Request.Path.Value.Split("/").ToList().Last();
                }
                finally
                {
                    filename = $"ficticious-medical-document-{filename}.pdf";
                }

                httpContext.Response.Headers.Add("x-filename", filename);
                httpContext.Response.Headers.Add("Access-Control-Expose-Headers", "x-filename");
                //httpContext.Response.Headers.Add(HeaderNames.ContentDisposition, $"inline; filename='{filename}'");
                return Task.FromResult(0);
            }, context);

            await _next.Invoke(context);
            return;

        }
    }

    public static class ProviderBinaryOutputMiddlewareExtension
    {
        public static IApplicationBuilder UseProviderBinaryOutputMiddleware(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<ProviderBinaryOutputMiddleware>();
        }
    }
}
