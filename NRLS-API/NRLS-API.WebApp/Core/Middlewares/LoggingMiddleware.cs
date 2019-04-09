using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using NRLS_API.Core.Resources;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;


namespace NRLS_API.WebApp.Core.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public LoggingMiddleware(RequestDelegate next, ILoggerFactory logger)
        {
            _next = next;
            _logger = logger.CreateLogger("Startup.LoggingMiddleware");
        }

        public async Task Invoke(HttpContext context)
        {
            var internalTrace = Guid.NewGuid();
            var request = await FormatRequest(context.Request, internalTrace);

            _logger.LogInformation(LoggingEvents.HttpRequestIn, request);
            //TODO: persist log to storage


            var originalBodyStream = context.Response.Body;

            using (var responseBody = new MemoryStream())
            {
                context.Response.Body = responseBody;

                await _next(context);

                context.Response.Body.Position = 0;
                await responseBody.CopyToAsync(originalBodyStream);

                var response = await FormatResponse(context.Response, internalTrace);

                _logger.LogInformation(LoggingEvents.HttpResponseOut, response);
                //TODO: persist log to storage
            }

            context.Response.Body = originalBodyStream;

        }

        private async Task<string> FormatRequest(HttpRequest request, Guid internalTraceId)
        {
            //var body = request.Body;

            request.EnableRewind();

            var buffer = new byte[Convert.ToInt32(request.ContentLength)];

            await request.Body.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);

            var bodyAsText = Encoding.UTF8.GetString(buffer);

            //request.Body = body;

            request.Body.Position = 0;

            var headers = FormatHeaders(request.Headers);

            return $"Request started {internalTraceId} {DateTime.UtcNow.ToString("dddd, dd MMMM yyyy HH:mm:sszzz")} {headers} {request.Protocol} {request.Method} {request.Scheme}://{request.Host}{request.PathBase}{request.Path} {request.QueryString} {bodyAsText}";
        }

        private async Task<string> FormatResponse(HttpResponse response, Guid internalTraceId)
        {

            response.Body.Seek(0, SeekOrigin.Begin);

            var text = await new StreamReader(response.Body).ReadToEndAsync();
            var headers = FormatHeaders(response.Headers);

            response.Body.Seek(0, SeekOrigin.Begin);

            return $"Response started {internalTraceId} {DateTime.UtcNow.ToString("dddd, dd MMMM yyyy HH:mm:sszzz")} {headers} {response.StatusCode}: {text}";
        }

        private string FormatHeaders(IHeaderDictionary headers)
        {
            var formatted = "Headers ";

            if (headers != null && headers.Count > 0)
            {
                foreach (var header in headers)
                {
                    formatted = $"{formatted}'{header.Key}:{header.Value}',";
                }
            }
            else
            {
                formatted = $"{formatted}none";
            }

            return formatted = $"{formatted};";
        }
    }

    public static class LoggingMiddlewareExtension
    {
        public static IApplicationBuilder UseLoggingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LoggingMiddleware>();
        }
    }
}
