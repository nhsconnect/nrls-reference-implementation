using Demonstrator.Core.Interfaces.Helpers;
using Demonstrator.Core.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Demonstrator.Core.Helpers
{
    public class LoggingHelper : ILoggingHelper
    {
        private readonly ILogger _logger;

        public LoggingHelper(ILoggerFactory logger)
        {
            _logger = logger.CreateLogger("Demonstrator.Core.Helpers.LoggingHelper");
        }

        public void LogHttpRequestMessage(HttpRequestHeaders headers, Version version, HttpMethod method, Uri requestUri, string bodyAsText, Guid internalTraceId)
        {
            var requestLog = GenerateRequestLog(internalTraceId, FormatEnumerableHeaders(headers), version.ToString(), method.ToString(), requestUri.Scheme, new HostString(requestUri.Host, requestUri.Port), string.Empty, requestUri.AbsolutePath, new QueryString(requestUri.Query), bodyAsText);

            _logger.LogInformation(LoggingEvents.HttpRequestOut, requestLog);
            //TODO: persist log to storage
        }

        public void LogHttpResponseMessage(HttpResponseHeaders headers, int statusCode, string bodyAsText, Guid internalTraceId)
        {
            var requestLog = GenerateResponseLog(internalTraceId, FormatEnumerableHeaders(headers), statusCode, bodyAsText);

            _logger.LogInformation(LoggingEvents.HttpResponseIn, requestLog);
            //TODO: persist log to storage
        }

        public async void LogHttpRequest(HttpRequest request, Guid internalTraceId)
        {
            var requestLog = await FormatRequest(request, internalTraceId);

            _logger.LogInformation(LoggingEvents.HttpRequestIn, requestLog);
            //TODO: persist log to storage
        }

        public async void LogHttpResponse(HttpResponse response, Guid internalTraceId)
        {
            var responseLog = await FormatResponse(response, internalTraceId);

            _logger.LogInformation(LoggingEvents.HttpResponseOut, responseLog);
            //TODO: persist log to storage
        }

        private string GenerateRequestLog(Guid internalTraceId, string headers, string protocol, string method, string scheme, HostString host, string pathBase, string path, QueryString queryString, string body)
        {
            return $"Request started {internalTraceId} {DateTime.UtcNow.ToString("dddd, dd MMMM yyyy HH:mm:sszzz")} {headers} {protocol} {method} {scheme}://{host}{pathBase}{path} {queryString} {body}";
        }

        private string GenerateResponseLog(Guid internalTraceId, string headers, int statusCode, string bodyAsText)
        {
            return $"Response started {internalTraceId} {DateTime.UtcNow.ToString("dddd, dd MMMM yyyy HH:mm:sszzz")} {headers} {statusCode}: {bodyAsText}";
        }

        private async Task<string> FormatRequest(HttpRequest request, Guid internalTraceId)
        {
            request.EnableRewind();

            var buffer = new byte[Convert.ToInt32(request.ContentLength)];

            await request.Body.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);

            var bodyAsText = Encoding.UTF8.GetString(buffer);

            request.Body.Position = 0;

            var headers = FormatHeaders(request.Headers);

            return GenerateRequestLog(internalTraceId, headers, request.Protocol, request.Method, request.Scheme, request.Host, request.PathBase, request.Path, request.QueryString, bodyAsText);
        }

        private async Task<string> FormatResponse(HttpResponse response, Guid internalTraceId)
        {
            response.Body.Seek(0, SeekOrigin.Begin);

            var bodyAsText = await new StreamReader(response.Body).ReadToEndAsync();
            var headers = FormatHeaders(response.Headers);

            response.Body.Seek(0, SeekOrigin.Begin);

            return GenerateResponseLog(internalTraceId, headers, response.StatusCode, bodyAsText);
        }

        private string FormatHeaders(IHeaderDictionary headersIn)
        {
            var headers = headersIn.AsEnumerable().Select(h => KeyValuePair.Create(h.Key, !StringValues.IsNullOrEmpty(h.Value) ? h.Value.ToList() : Enumerable.Empty<string>()));

            return FormatEnumerableHeaders(headers);
        }

        private string FormatEnumerableHeaders(IEnumerable<KeyValuePair<string, IEnumerable<string>>> headersIn)
        {
            var headers = headersIn.Select(h => KeyValuePair.Create(h.Key, string.Join(" ", (h.Value.ToList()))));

            var formatted = "Headers ";

            if (headers != null && headers.Count() > 0)
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
}
