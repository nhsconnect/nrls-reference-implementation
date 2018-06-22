using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using NRLS_API.Core.Exceptions;
using NRLS_API.Core.Factories;
using NRLS_API.Core.Helpers;
using NRLS_API.Core.Interfaces.Services;
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
        private readonly INrlsValidation _nrlsValidation;

        public SspAuthorizationMiddleware(RequestDelegate next, IOptions<SpineSetting> spineSettings, IOptions<NrlsApiSetting> nrlsApiSettings, IMemoryCache memoryCache, INrlsValidation nrlsValidation)
        {
            _next = next;
            _spineSettings = spineSettings.Value;
            _nrlsApiSettings = nrlsApiSettings.Value;
            _cache = memoryCache;
            _nrlsValidation = nrlsValidation;
        }

        public async SystemTasks.Task Invoke(HttpContext context)
        {
            //Order of validation is Important
            var request = context.Request;
            var headers = request.Headers;
            var method = request.Method;


            //Accept is optional but must be valid if supplied
            //Check is delegated to FhirInputMiddleware


            var authorization = GetHeaderValue(headers, HttpRequestHeader.Authorization.ToString());
            var scope = method == HttpMethods.Get ? JwtScopes.Read : JwtScopes.Write;
            if (authorization == null || !_nrlsValidation.ValidJwt(scope, authorization))
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

            //We've Passed! Continue to App...
            await _next.Invoke(context);
            return;

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
