using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using NRLS_API.Core.Enums;
using NRLS_API.Core.Exceptions;
using NRLS_API.Core.Factories;
using NRLS_API.Core.Interfaces.Services;
using NRLS_API.Core.Resources;
using System;
using System.Linq;
using System.Net;
using SystemTasks = System.Threading.Tasks;


namespace NRLS_API.WebApp.Core.Middlewares
{
    public class SspProxyGateMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ISdsService _sdsService;
        private readonly INrlsValidation _nrlsValidation;

        public SspProxyGateMiddleware(RequestDelegate next, ISdsService sdsService, INrlsValidation nrlsValidation)
        {
            _next = next;
            _sdsService = sdsService;
            _nrlsValidation = nrlsValidation;
        }

        public async SystemTasks.Task Invoke(HttpContext context)
        {

            //Basic headers check (asid, auth, etc)

            var headers = context.Request.Headers;
            var method = context.Request.Method;

            // -> JWT
            var authorization = GetHeaderValue(headers, HeaderNames.Authorization);

            var jwtResponse = _nrlsValidation.ValidJwt(new Tuple<JwtScopes, string>(JwtScopes.Read, "*"), authorization);
            if (string.IsNullOrEmpty(authorization) || !jwtResponse.Success)
            {
                SetJwtError(HeaderNames.Authorization, jwtResponse.Message);
            }

            // -> fromASID
            var fromASID = GetHeaderValue(headers, FhirConstants.HeaderSspFromAsid);
            var consumerCache = _sdsService.GetFor(fromASID);

            if (consumerCache == null)
            {
                SetError(FhirConstants.HeaderSspFromAsid, "The Ssp-From ASID header value is not known.");
            }

            // -> toASID
            var toASID = GetHeaderValue(headers, FhirConstants.HeaderSspToAsid);
            var providerCache = _sdsService.GetFor(toASID);

            if (providerCache == null)
            {
                SetError(FhirConstants.HeaderSspToAsid, "The Ssp-To ASID header value is not known.");
            }

            // -> traceID
            var traceId = GetHeaderValue(headers, FhirConstants.HeaderSspTradeId);
            if (string.IsNullOrEmpty(traceId))
            {
                SetError(FhirConstants.HeaderSspTradeId, null);
            }

            // -> interactionID
            var interactionId = GetHeaderValue(headers, FhirConstants.HeaderSspInterationId);
            if (string.IsNullOrEmpty(interactionId) || !consumerCache.Interactions.Contains(interactionId))
            {
                SetError(FhirConstants.HeaderSspInterationId, null);
            }

            //Provider FQDN check
            var providerOdsCache = _sdsService.GetFor(providerCache.OdsCode, interactionId);
            var forwardingUrl = WebUtility.UrlDecode(context.Request.Path.Value.Replace("/nrls-ri/SSP/", ""));
            var validFqdn = false;

            if (providerOdsCache == null)
            {
                SetError(FhirConstants.HeaderSspToAsid, "The Ssp-To ASID header value is not associated with the requested interaction.");
            }

            foreach (var endpoint in providerCache.EndPoints)
            {
                if(forwardingUrl.StartsWith(endpoint.AbsoluteUri))
                {
                    validFqdn = true;
                    break;
                }
            }

            if (!validFqdn)
            {
                SetError(FhirConstants.HeaderSspToAsid, $"The FQDN in the request ({forwardingUrl}) does not match an FQDN registered for the provided {FhirConstants.HeaderSspToAsid} header.");
            }

            //TODO: ssl check

            await _next(context);

        }

        //TODO: move to generic helper
        private string GetHeaderValue(IHeaderDictionary headers, string header)
        {
            string headerValue = null;

            if (headers.ContainsKey(header))
            {
                var check = headers[header];

                if (!string.IsNullOrWhiteSpace(check))
                {
                    headerValue = check;
                }
            }

            return headerValue;
        }

        //TODO: move to generic helper
        private void SetError(string header, string diagnostics)
        {
            throw new HttpFhirException("Invalid/Missing Header", OperationOutcomeFactory.CreateInvalidHeader(header, diagnostics), HttpStatusCode.BadRequest);
        }

        //TODO: move to generic helper
        private void SetJwtError(string header, string diagnostics)
        {
            throw new HttpFhirException("Invalid/Missing Header", OperationOutcomeFactory.CreateInvalidJwtHeader(header, diagnostics), HttpStatusCode.BadRequest);
        }

    }

    public static class SspProxyGateMiddlewareExtensions
    {
        public static IApplicationBuilder UseSspProxyGateMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SspProxyGateMiddleware>();
        }
    }
}
