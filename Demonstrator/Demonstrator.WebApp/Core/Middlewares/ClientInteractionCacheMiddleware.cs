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
        private readonly IClientAsidHelper _clientAsidHelper;

        public ClientInteractionCacheMiddleware(RequestDelegate next, IClientAsidHelper clientAsidHelper)
        {
            _next = next;
            _clientAsidHelper = clientAsidHelper;
        }

        public async Task Invoke(HttpContext context)
        {
            _clientAsidHelper.Load();

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
