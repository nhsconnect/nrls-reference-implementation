using Demonstrator.Core.Interfaces.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Demonstrator.WebApp.Core.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILoggingHelper _loggingHelper;

        public LoggingMiddleware(RequestDelegate next, ILoggingHelper loggingHelper)
        {
            _next = next;
            _loggingHelper = loggingHelper;
        }

        public async Task Invoke(HttpContext context)
        {
            var internalTrace = Guid.NewGuid();
            _loggingHelper.LogHttpRequest(context.Request, internalTrace);

            var originalBodyStream = context.Response.Body;

            using (var responseBody = new MemoryStream())
            {
                context.Response.Body = responseBody;

                await _next(context);

                context.Response.Body.Position = 0;
                await responseBody.CopyToAsync(originalBodyStream);

                _loggingHelper.LogHttpResponse(context.Response, internalTrace);

            }

            context.Response.Body = originalBodyStream;

        }
    }

    public static class LoggingMiddlewareExtension
    {
        public static IApplicationBuilder UseLoggingMiddleware(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<LoggingMiddleware>();
        }
    }
}
