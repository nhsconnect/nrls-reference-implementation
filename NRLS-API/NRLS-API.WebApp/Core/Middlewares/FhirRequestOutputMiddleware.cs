﻿using Hl7.Fhir.Rest;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using NRLS_API.Core.Exceptions;
using NRLS_API.Core.Factories;
using NRLS_API.Models.Core;
using NRLS_API.Models.Extensions;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using SystemTasks = System.Threading.Tasks;


namespace NRLS_API.WebApp.Core.Middlewares
{
    public class FhirRequestOutputMiddleware
    {
        private readonly RequestDelegate _next;
        private ApiSetting _nrlsApiSettings;

        public FhirRequestOutputMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async SystemTasks.Task Invoke(HttpContext context, IOptionsSnapshot<ApiSetting> nrlsApiSettings)
        {
            CheckRequestRequirements(context);

            _nrlsApiSettings = nrlsApiSettings.Get("NrlsApiSetting");

            var formatKey = "_format";
            var acceptKey = HeaderNames.Accept;

            var parameters = context.Request.QueryString.Value.GetParameters();

            bool hasFormatParam = parameters?.FirstOrDefault(x => x.Item1 == formatKey) != null;
            string formatParam = parameters?.GetParameter(formatKey);

            string acceptHeader = null;
            bool hasAcceptHeader = context.Request.Headers.ContainsKey(acceptKey);
            if (hasAcceptHeader)
            {
                acceptHeader = context.Request.Headers[acceptKey];
            }

            var validFormatParam = !hasFormatParam || (!string.IsNullOrWhiteSpace(formatParam) && _nrlsApiSettings.SupportedContentTypes.Contains(formatParam));
            var validAcceptHeader = !hasAcceptHeader || (!string.IsNullOrWhiteSpace(acceptHeader) && ValidAccept(acceptHeader));

            if (!validFormatParam && (hasFormatParam || !validAcceptHeader))
            {
                throw new HttpFhirException("Unsupported Media Type", OperationOutcomeFactory.CreateInvalidMediaType(), HttpStatusCode.UnsupportedMediaType);
            }

            if (validFormatParam)
            {
                var accepted = ContentType.GetResourceFormatFromFormatParam(formatParam);
                if (accepted != ResourceFormat.Unknown)
                {
                    var newAcceptHeader = ContentType.XML_CONTENT_HEADER;

                    if (accepted == ResourceFormat.Json)
                    {
                        newAcceptHeader = ContentType.JSON_CONTENT_HEADER;
                    }

                    var header = new MediaTypeHeaderValue(newAcceptHeader);

                    context.Request.Headers.Remove(acceptKey);
                    context.Request.Headers.Add(acceptKey, new StringValues(header.ToString()));
                }
            }

            await this._next(context);
        }

        private void CheckRequestRequirements(HttpContext context)
        {
            var contentLength = context?.Request?.ContentLength;
            var type = context?.Request?.Method;
            if (new string[] { HttpMethods.Post, HttpMethods.Put }.Contains(type) && (!contentLength.HasValue || contentLength.Value == 0))
            {
                throw new HttpFhirException("Invalid Request", OperationOutcomeFactory.CreateInvalidRequest(), HttpStatusCode.BadRequest);
            }
        }

        private bool ValidAccept(string accept)
        {
            foreach (var type in _nrlsApiSettings.SupportedContentTypes)
            {
                if (accept.Contains(type))
                {
                    return true;
                }
            }

            return false;
        }
    }

    public static class FhirRequestOutputMiddlewareExtensions
    {
        public static IApplicationBuilder UseFhirRequestOutputMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<FhirRequestOutputMiddleware>();
        }
    }
}
