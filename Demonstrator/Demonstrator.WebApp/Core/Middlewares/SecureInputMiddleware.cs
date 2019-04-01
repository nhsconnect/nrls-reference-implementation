using Demonstrator.Core.Exceptions;
using Demonstrator.Core.Factories;
using Demonstrator.Core.Interfaces.Helpers;
using Demonstrator.Models.Core.Enums;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Demonstrator.WebApp.Core.Middlewares
{
    public class SecureInputMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IJwtHelper _jwtHelper;

        public SecureInputMiddleware(RequestDelegate next, IJwtHelper jwtHelper)
        {
            _next = next;
            _jwtHelper = jwtHelper;
        }

        public async Task Invoke(HttpContext context)
        {

            //Order of validation is Important
            var request = context.Request;
            var headers = request.Headers;
            var method = request.Method;
            var scope = method == HttpMethods.Get ? JwtScopes.Read : JwtScopes.Write;


            var authorization = GetHeaderValue(headers, HeaderNames.Authorization);
            var jwtResponse = _jwtHelper.IsValidUser(authorization);
            if (!jwtResponse.Success)
            {
                SetJwtError(HeaderNames.Authorization, jwtResponse.Message);
            }

            //TODO: check user is auth & auth

            //We've Passed! Continue to App...
            await _next(context);

        }

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

        private void SetJwtError(string header, string diagnostics)
        {
            throw new HttpFhirException("Invalid/Missing Header", OperationOutcomeFactory.CreateInvalidJwtHeader(header, diagnostics), HttpStatusCode.BadRequest);
        }
    }

    public static class SecureInputMiddlewareExtension
    {
        public static IApplicationBuilder UseSecureInputMiddleware(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<SecureInputMiddleware>();
        }
    }
}
