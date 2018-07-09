using Hl7.Fhir.Rest;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using NRLS_API.Core.Exceptions;
using NRLS_API.Core.Factories;
using NRLS_API.Models.Core;
using NRLS_API.Models.Extensions;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using SystemTasks = System.Threading.Tasks;


namespace NRLS_API.WebApp.Core.Middlewares
{
    public class FhirInputMiddleware
    {
        private readonly RequestDelegate _next;
        private ApiSetting _nrlsApiSettings;

        public FhirInputMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async SystemTasks.Task Invoke(HttpContext context, IOptionsSnapshot<ApiSetting> nrlsApiSettings)
        {
            _nrlsApiSettings = nrlsApiSettings.Get("NrlsApiSetting");

            var formatKey = "_format";
            var acceptKey = HttpRequestHeader.Accept.ToString();

            var parameters = context.Request.QueryString.Value.GetParameters();

            bool hasFormatParam = parameters?.FirstOrDefault(x => x.Item1 == "_format") != null;
            string formatParam = parameters != null ? parameters.GetParameter(formatKey) : null;

            string acceptHeader = null;
            bool hasAcceptHeader = context.Request.Headers.ContainsKey(acceptKey);
            if (hasAcceptHeader)
            {
                acceptHeader = context.Request.Headers[acceptKey];
            }

            var validFormatParam = !hasFormatParam || (!string.IsNullOrWhiteSpace(formatParam) && _nrlsApiSettings.SupportedContentTypes.Contains(formatParam));
            var validAcceptHeader = !hasAcceptHeader || (!string.IsNullOrWhiteSpace(acceptHeader) && ValidAccept(acceptHeader));


            if(!validFormatParam && !validAcceptHeader)
            {
                throw new HttpFhirException("Unsupported Media Type", OperationOutcomeFactory.CreateInvalidMediaType(), HttpStatusCode.UnsupportedMediaType);
            }

            if (validFormatParam)
            {
                var accepted = ContentType.GetResourceFormatFromFormatParam(formatParam);
                if (accepted != ResourceFormat.Unknown)
                {
                    context.Request.Headers.Remove(acceptKey);

                    var newAcceptHeader = ContentType.XML_CONTENT_HEADER;

                    if (accepted == ResourceFormat.Json)
                    {
                        newAcceptHeader = ContentType.JSON_CONTENT_HEADER;
                    }

                    var header = new MediaTypeHeaderValue(newAcceptHeader);
                    header.CharSet = Encoding.UTF8.WebName;

                    context.Request.Headers.Remove(acceptKey);
                    context.Request.Headers.Add(acceptKey, new StringValues(header.ToString()));
                }
            }

            await this._next(context);
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

    public static class FhirInputMiddlewareExtensions
    {
        public static IApplicationBuilder UseFhirInputMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<FhirInputMiddleware>();
        }
    }
}
