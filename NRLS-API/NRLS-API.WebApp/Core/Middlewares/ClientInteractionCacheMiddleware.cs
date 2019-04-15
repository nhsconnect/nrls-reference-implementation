using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using NRLS_API.Core.Interfaces.Services;
using System;
using System.Threading.Tasks;

namespace NRLS_API.WebApp.Core.Middlewares
{
    public class ClientInteractionCacheMiddleware
    {
        private readonly RequestDelegate _next;
        private ISdsService _sdsService;

        public ClientInteractionCacheMiddleware(RequestDelegate next, ISdsService sdsService)
        {
            _next = next;
            _sdsService = sdsService;
        }

        public async Task Invoke(HttpContext context)
        {
            //Fake SSP Interaction/ASID datastore

            var entries = _sdsService.GetAll();

            entries = null;

            await _next.Invoke(context);
            return;

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
