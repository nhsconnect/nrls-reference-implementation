using Demonstrator.Core.Exceptions;
using Demonstrator.Core.Factories;
using Hl7.Fhir.Rest;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Demonstrator.Models.Core.Models;
using Demonstrator.Models.Extensions;
using System.Linq;
using System.Net;
using SystemTasks = System.Threading.Tasks;
using Demonstrator.Core.Resources;

namespace Demonstrator.WebApp.Core.Middlewares
{
    public class FhirInputMiddleware
    {
        private readonly RequestDelegate _next;
        private ApiSetting _apiSettings;

        public FhirInputMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async SystemTasks.Task Invoke(HttpContext context, IOptionsSnapshot<ApiSetting> apiSettings)
        {
            CheckRequestRequirements(context);

            _apiSettings = apiSettings.Value;

            var parameters = context.Request.QueryString.Value.GetParameters();

            bool hasFormatParam = parameters?.FirstOrDefault(x => x.Item1 == "_format") != null;
            string formatParam = parameters?.GetParameter("_format");

            bool hasAcceptHeader = context.Request.Headers.ContainsKey(HeaderNames.Accept);
            string acceptHeader = hasAcceptHeader ? context.Request.Headers[HeaderNames.Accept] : StringValues.Empty;


            var validFormatParam = !hasFormatParam || ValidContentType(formatParam);
            var validAcceptHeader = !hasAcceptHeader || ValidContentType(acceptHeader);

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

                    context.Request.Headers.Remove(HeaderNames.Accept);
                    context.Request.Headers.Add(HeaderNames.Accept, new StringValues(header.ToString()));
                }
            }

            //TODO: add default switch on controller
            context.Request.Headers.Remove(FhirConstants.HeaderXFhirDefault);
            context.Request.Headers.Add(FhirConstants.HeaderXFhirDefault, ContentType.XML_CONTENT_HEADER);

            await _next(context);
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

        private bool ValidContentType(string contentType)
        {
            return !string.IsNullOrWhiteSpace(contentType) && _apiSettings.SupportedContentTypes.Contains(contentType);
        }
    }

    public static class FhirInputMiddlewareExtensions
    {
        public static IApplicationBuilder UseFhirInputMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<FhirInputMiddleware>();
        }
    }
}
