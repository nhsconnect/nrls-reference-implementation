using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
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

            var routeData = context.GetRouteData();

            if (context.Response.StatusCode == 404 && routeData == null)
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
