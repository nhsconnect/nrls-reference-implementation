using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using NRLS_API.Core.Exceptions;
using NRLS_API.Core.Factories;
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

            var accept = GetHeaderValue(context, "Accept");
            if (accept == null || !ValidAccept(accept))
            {
                SetError("Accept");
            }

            var authorization = GetHeaderValue(context, "Authorization");
            if (authorization == null) //check jwt
            {
                SetError("Authorization");
            }

            var fromASID = GetHeaderValue(context, "fromASID");
            if (fromASID == null || GetFromAsidMap(fromASID) == null)
            {
                SetError("fromASID");
            }

            var toASID = GetHeaderValue(context, "toASID");
            if (toASID == null || toASID != _spineSettings.Asid)
            {
                SetError("toASID");
            }

            var sspInteractionID = GetHeaderValue(context, "Ssp-InteractionID");
            if (sspInteractionID == null || !ValidInteraction(context, sspInteractionID, fromASID))
            {
                SetError("Ssp-InteractionID");
            }

            //var sspTraceId = GetHeaderValue(context, "Ssp-TraceID");
            //if (sspTraceId == null)
            //{
            //    SetError("Ssp-TraceID");
            //}

            //var sspVersion = GetHeaderValue(context, "Ssp-Version");
            //if (sspVersion == null)
            //{
            //    SetError("Ssp-Version");
            //}

            //We've Passed! Continue to App...
            await _next.Invoke(context);
            return;

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

        private bool ValidInteraction(HttpContext context, string interactionId, string fromASID)
        {

            var clientAsid = GetFromAsidMap(fromASID);

            if (context.Request.Method.Equals(HttpMethods.Get) && clientAsid.Interactions.Contains(interactionId) && (FhirConstants.ReadInteractionId.Equals(interactionId) || FhirConstants.SearchInteractionId.Equals(interactionId)))
            {
                return true;
            }

            if (context.Request.Method.Equals(HttpMethods.Post) && clientAsid.Interactions.Contains(interactionId) && FhirConstants.CreateInteractionId.Equals(interactionId))
            {
                return true;
            }

            if (context.Request.Method.Equals(HttpMethods.Put) && clientAsid.Interactions.Contains(interactionId) && FhirConstants.UpdateInteractionId.Equals(interactionId))
            {
                return true;
            }

            if (context.Request.Method.Equals(HttpMethods.Delete) && clientAsid.Interactions.Contains(interactionId) && FhirConstants.DeleteInteractionId.Equals(interactionId))
            {
                return true;
            }

            return false;

        }

        private string GetHeaderValue(HttpContext context, string header)
        {
            string headerValue = null;

            if(context.Request.Headers.ContainsKey(header))
            {
                var check = context.Request.Headers[header];

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

            if (!_cache.TryGetValue<ClientAsidMap>("ClientAsidMap", out clientAsidMap))
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
            throw new HttpFhirException("Invalid/Missing Header", OperationOutcomeFactory.CreateInvalidHeader(ResourceType.DocumentReference.ToString(), header), HttpStatusCode.BadRequest);
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
