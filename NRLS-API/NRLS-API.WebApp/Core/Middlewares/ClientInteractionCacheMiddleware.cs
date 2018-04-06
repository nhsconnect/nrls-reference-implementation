using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NRLS_API.Models.Core;
using System;
using System.IO;
using System.Threading.Tasks;

namespace NRLS_API.WebApp.Core.Middlewares
{
    public class ClientInteractionCacheMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly SpineSetting _spineSettings;
        private IMemoryCache _cache;

        public ClientInteractionCacheMiddleware(RequestDelegate next, IOptions<SpineSetting> spineSettings, IMemoryCache memoryCache)
        {
            _next = next;
            _spineSettings = spineSettings.Value;
            _cache = memoryCache;
        }

        public async Task Invoke(HttpContext context)
        {
            //Fake SSP Interaction/ASID datastore

            ClientAsidMap clientAsidMap;
            if (!_cache.TryGetValue<ClientAsidMap>("ClientAsidMap", out clientAsidMap))
            {
                var appPath = AppContext.BaseDirectory;
                var pathEnd = appPath.LastIndexOf("bin");
                var isBinPath = !(pathEnd < 0);
                var basePath = isBinPath ? appPath.Substring(0, pathEnd) : appPath;
                var pathAffix = isBinPath ? @"..\" : "";

                using (StreamReader interactionFile = File.OpenText(Path.Combine(basePath, _spineSettings.ClientAsidMapFile)))
                {
                    var serializer = new JsonSerializer();
                    clientAsidMap = (ClientAsidMap)serializer.Deserialize(interactionFile, typeof(ClientAsidMap));

                    // Save data in cache.
                    _cache.Set("ClientAsidMap", clientAsidMap);
                }
            }


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
