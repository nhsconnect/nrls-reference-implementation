using Demonstrator.Core.Interfaces.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Demonstrator.WebApp.Core.Middlewares
{
    public class ClientInteractionCacheMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ISdsService _sdsService;

        public ClientInteractionCacheMiddleware(RequestDelegate next, ISdsService sdsService)
        {
            _next = next;
            _sdsService = sdsService;
        }

        public async Task Invoke(HttpContext context)
        {
            var entries = _sdsService.GetAllFromSource();

            entries = null;

            await _next(context);
        }
    }

    public static class ClientInteractionCacheMiddlewareExtension
    {
        public static IApplicationBuilder UseClientInteractionCacheMiddleware(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<ClientInteractionCacheMiddleware>();
        }
    }
}
