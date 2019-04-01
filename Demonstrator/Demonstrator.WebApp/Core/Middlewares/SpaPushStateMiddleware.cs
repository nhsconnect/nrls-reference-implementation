using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Demonstrator.WebApp.Core.Middlewares
{
    public class SpaPushStateMiddleware
    {
        private readonly RequestDelegate _next;

        public SpaPushStateMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            await _next(context);

            if (context.Response.StatusCode == 404)
            {
                context.Request.Path = "/index.html";
                await _next(context);
            }

        }
    }

    public static class SpaPushStateMiddlewareExtension
    {
        public static IApplicationBuilder UseSpaPushStateMiddleware(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<SpaPushStateMiddleware>();
        }
    }
}
