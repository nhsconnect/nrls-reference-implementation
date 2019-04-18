using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using NRLS_API.Core.Interfaces.Services;
using NRLS_API.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using SystemTasks = System.Threading.Tasks;


namespace NRLS_API.WebApp.Core.Middlewares
{
    public class SspProxyRequestMiddleware
    {
        private readonly RequestDelegate _next;
        private ApiSetting _sspApiSettings;

        public SspProxyRequestMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async SystemTasks.Task Invoke(HttpContext context, IOptionsSnapshot<ApiSetting> apiSettings, ISspProxyService _sspProxyService)
        {
            _sspApiSettings = apiSettings.Get("SspApiSetting");

            var request = new CommandRequest();

            var forwardingUrl = context.Request.Path.Value.Replace($"{_sspApiSettings.BasePath}/", "");

            request.Headers = context.Request.Headers.Select(h => KeyValuePair.Create(h.Key, string.Join("; ", (h.Value.ToList()))));
            request.Method = HttpMethod.Get; //could be more types in future, so fetch from context
            request.ForwardUrl = new Uri(WebUtility.UrlDecode(forwardingUrl));
            request.Forwarded = new Forwarded
            {
                By = context.Connection.LocalIpAddress.ToString(),
                For = context.Connection.RemoteIpAddress.ToString(),
                Host = request.ForwardUrl.Host,
                Protocol = _sspApiSettings.Secure ? "https" : "http"
            };

            var result = await _sspProxyService.ForwardRequest(request);

            context.Response.StatusCode = result.StatusCode;

            foreach (var resHeader in result.Headers)
            {
                context.Response.Headers.Add(resHeader.Key, new StringValues(resHeader.Value.ToArray()));
            }

            await context.Response.Body.WriteAsync(result.Content, 0, result.Content.Length);

        }

    }

    public static class SspProxyRequestMiddlewareExtensions
    {
        public static IApplicationBuilder UseSspProxyRequestMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SspProxyRequestMiddleware>();
        }
    }
}
