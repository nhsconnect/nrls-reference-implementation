using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using NRLS_API.Core.Exceptions;
using NRLS_API.Core.Factories;
using NRLS_API.Core.Helpers;
using NRLS_API.Core.Resources;
using NRLS_API.Models.Core;
using System;
using System.Net;
using SystemTasks = System.Threading.Tasks;

namespace NRLS_API.WebApp.Core.Middlewares
{
    public class SspAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly SpineSetting _spineSettings;
        private readonly NrlsApiSetting _nrlsApiSettings;
        private IMemoryCache _cache;

        public SspAuthorizationMiddleware(RequestDelegate next, IOptions<SpineSetting> spineSettings, IOptions<NrlsApiSetting> nrlsApiSettings, IMemoryCache memoryCache)
        {
            _next = next;
            _spineSettings = spineSettings.Value;
            _nrlsApiSettings = nrlsApiSettings.Value;
            _cache = memoryCache;
        }

        public async SystemTasks.Task Invoke(HttpContext context)
        {
            //Order of validation is Important
            var request = context.Request;
            var headers = request.Headers;
            var method = request.Method;


            //Accept is optional but must be valid if supplied
            var accept = GetHeaderValue(headers, HttpRequestHeader.Accept.ToString());
            if (accept != null && !ValidAccept(accept))
            {
                SetError(HttpRequestHeader.Accept.ToString());
            }

            var authorization = GetHeaderValue(headers, HttpRequestHeader.Authorization.ToString());
            if (authorization == null || !ValidJwt(method, authorization))
            {
                SetError(HttpRequestHeader.Authorization.ToString());
            }

            var fromASID = GetHeaderValue(headers, FhirConstants.HeaderFromAsid);
            if (fromASID == null || GetFromAsidMap(fromASID) == null)
            {
                SetError(FhirConstants.HeaderFromAsid);
            }

            var toASID = GetHeaderValue(headers, FhirConstants.HeaderToAsid);
            if (toASID == null || toASID != _spineSettings.Asid)
            {
                SetError(FhirConstants.HeaderToAsid);
            }

            var sspInteractionID = GetHeaderValue(headers, FhirConstants.HeaderSspInterationId);
            if (sspInteractionID == null || !ValidInteraction(method, sspInteractionID, fromASID))
            {
                SetError(FhirConstants.HeaderSspInterationId);
            }

            //var sspTraceId = GetHeaderValue(headers, "Ssp-TraceID");
            //if (sspTraceId == null)
            //{
            //    SetError("Ssp-TraceID");
            //}

            //var sspVersion = GetHeaderValue(headers, "Ssp-Version");
            //if (sspVersion == null)
            //{
            //    SetError("Ssp-Version");
            //}

            //We've Passed! Continue to App...
            await _next.Invoke(context);
            return;

        }

        private bool ValidJwt(string method, string jwt)
        {
            var scope = method == HttpMethods.Get ? JwtScopes.Read : JwtScopes.Write;

            return JwtHelper.IsValid(jwt, scope);
        }

        private bool ValidAccept(string accept)
        {
            foreach(var type in _nrlsApiSettings.SupportedContentTypes)
            {
                if (accept.Contains(type))
                {
                    return true;
                }
            }

            return false;
        }

        private bool ValidInteraction(string method, string interactionId, string fromASID)
        {

            var clientAsid = GetFromAsidMap(fromASID);

            if (method.Equals(HttpMethods.Get) && clientAsid.Interactions.Contains(interactionId) && (FhirConstants.ReadInteractionId.Equals(interactionId) || FhirConstants.SearchInteractionId.Equals(interactionId)))
            {
                return true;
            }

            if (method.Equals(HttpMethods.Post) && clientAsid.Interactions.Contains(interactionId) && FhirConstants.CreateInteractionId.Equals(interactionId))
            {
                return true;
            }

            if (method.Equals(HttpMethods.Put) && clientAsid.Interactions.Contains(interactionId) && FhirConstants.UpdateInteractionId.Equals(interactionId))
            {
                return true;
            }

            if (method.Equals(HttpMethods.Delete) && clientAsid.Interactions.Contains(interactionId) && FhirConstants.DeleteInteractionId.Equals(interactionId))
            {
                return true;
            }

            return false;

        }

        private string GetHeaderValue(IHeaderDictionary headers, string header)
        {
            string headerValue = null;

            if(headers.ContainsKey(header))
            {
                var check = headers[header];

                if (!string.IsNullOrWhiteSpace(check))
                {
                    headerValue = check;
                }
            }

            return headerValue;
        }

        private ClientAsid GetFromAsidMap(string fromASID)
        {
            ClientAsidMap clientAsidMap;

            if (!_cache.TryGetValue<ClientAsidMap>(ClientAsidMap.Key, out clientAsidMap))
            {
                return null;
            }

            if (clientAsidMap.ClientAsids == null || !clientAsidMap.ClientAsids.ContainsKey(fromASID))
            {
                return null;
            }

            return clientAsidMap.ClientAsids[fromASID];
        }

        private void SetError(string header)
        {
            throw new HttpFhirException("Invalid/Missing Header", OperationOutcomeFactory.CreateInvalidHeader(header), HttpStatusCode.BadRequest);
        }

    }

    public static class SspAuthorizationMiddlewareExtension
    {
        public static IApplicationBuilder UseSspAuthorizationMiddleware(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<SspAuthorizationMiddleware>();
        }
    }
}
