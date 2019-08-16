using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using NRLS_API.Core.Enums;
using NRLS_API.Core.Exceptions;
using NRLS_API.Core.Factories;
using NRLS_API.Core.Interfaces.Services;
using NRLS_API.Core.Resources;
using NRLS_API.Models.Core;
using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using SystemTasks = System.Threading.Tasks;

namespace NRLS_API.WebApp.Core.Middlewares
{
    public class SpineAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly SpineSetting _spineSettings;
        private ApiSetting _nrlsApiSettings;
        private ISdsService _sdsService;
        private readonly INrlsValidation _nrlsValidation;

        public SpineAuthorizationMiddleware(RequestDelegate next, IOptions<SpineSetting> spineSettings, ISdsService sdsService, INrlsValidation nrlsValidation)
        {
            _next = next;
            _spineSettings = spineSettings.Value;
            _sdsService = sdsService;
            _nrlsValidation = nrlsValidation;
        }

        public async SystemTasks.Task Invoke(HttpContext context, IOptionsSnapshot<ApiSetting> nrlsApiSettings)
        {
            _nrlsApiSettings = nrlsApiSettings.Get("NrlsApiSetting");


            //Order of validation is Important
            var request = context.Request;
            var headers = request.Headers;
            var method = request.Method;


            //Accept is optional but must be valid if supplied
            //Check is delegated to FhirInputMiddleware


            var authorization = GetHeaderValue(headers, HeaderNames.Authorization);
            var scope = method == HttpMethods.Get ? JwtScopes.Read : JwtScopes.Write;
            var jwtResponse = _nrlsValidation.ValidJwt(new Tuple<JwtScopes, string>(scope, "DocumentReference"), authorization);
            if (string.IsNullOrEmpty(authorization) || !jwtResponse.Success)
            {
                SetJwtError(HeaderNames.Authorization, jwtResponse.Message);
            }

            var fromASID = GetHeaderValue(headers, FhirConstants.HeaderFromAsid);
            var clientCache = _sdsService.GetFor(fromASID);

            if (clientCache == null)
            {
                SetError(FhirConstants.HeaderFromAsid, null);
            }

            var toASID = GetHeaderValue(headers, FhirConstants.HeaderToAsid);
            if (string.IsNullOrEmpty(toASID) || toASID != _spineSettings.Asid)
            {
                SetError(FhirConstants.HeaderToAsid, null);
            }

            var interactionId = GetInteractionId(method, request.Path.Value);
            if(string.IsNullOrEmpty(interactionId) || !clientCache.Interactions.Contains(interactionId))
            {
                throw new HttpFhirException("Client interaction request invalid", OperationOutcomeFactory.CreateAccessDenied(), HttpStatusCode.Forbidden);
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

        private string GetInteractionId(string method, string url)
        {
            if(method == HttpMethods.Get)
            {
                if(new Regex(@"\/DocumentReference\/([A-Fa-f0-9]{1,1024}$)").IsMatch(url))
                {
                    return FhirConstants.ReadInteractionId;
                }
                else if(url.EndsWith("/DocumentReference"))
                {
                    return FhirConstants.SearchInteractionId;
                }
            }

            if(method == HttpMethods.Post)
            {
                return FhirConstants.CreateInteractionId;
            }

            if (method == HttpMethods.Patch)
            {
                return FhirConstants.UpdateInteractionId;
            }

            if (method == HttpMethods.Delete)
            {
                return FhirConstants.DeleteInteractionId;
            }

            return null;
        }

        private void SetError(string header, string diagnostics)
        {
            throw new HttpFhirException("Invalid/Missing Header", OperationOutcomeFactory.CreateInvalidHeader(header, diagnostics), HttpStatusCode.BadRequest);
        }

        private void SetJwtError(string header, string diagnostics)
        {
            throw new HttpFhirException("Invalid/Missing Header", OperationOutcomeFactory.CreateInvalidJwtHeader(header, diagnostics), HttpStatusCode.BadRequest);
        }

    }

    public static class SpineAuthorizationMiddlewareExtension
    {
        public static IApplicationBuilder UseSpineAuthorizationMiddleware(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<SpineAuthorizationMiddleware>();
        }
    }
}
