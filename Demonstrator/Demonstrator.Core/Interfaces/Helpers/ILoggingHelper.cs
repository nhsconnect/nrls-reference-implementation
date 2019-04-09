using Microsoft.AspNetCore.Http;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Demonstrator.Core.Interfaces.Helpers
{
    public interface ILoggingHelper
    {
        void LogHttpRequest(HttpRequest request, Guid internalTraceId);

        void LogHttpResponse(HttpResponse response, Guid internalTraceId);

        void LogHttpRequestMessage(HttpRequestHeaders headers, Version version, HttpMethod method, Uri requestUri, string bodyAsText, Guid internalTraceId);

        void LogHttpResponseMessage(HttpResponseHeaders headers, int statusCode, string bodyAsText, Guid internalTraceId);
    }
}
