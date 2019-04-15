using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using NRLS_API.Core.Exceptions;
using NRLS_API.Core.Factories;
using NRLS_API.Core.Interfaces.Services;
using NRLS_API.Core.Resources;
using NRLS_API.Models.Core;
using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace NRLS_API.WebApp.Core.Middlewares
{
    public class ClientCertificateCheckMiddleware
    {
        private readonly RequestDelegate _next;
        private ISdsService _sdsService;
        private ApiSetting _nrlsApiSetting;

        public ClientCertificateCheckMiddleware( RequestDelegate next, ISdsService sdsService)
        {
            _next = next;
            _sdsService = sdsService;
        }

        public async Task Invoke(HttpContext context, IOptionsSnapshot<ApiSetting> apiSettings)
        {
            _nrlsApiSetting = apiSettings.Get("NrlsApiSetting");

            //Fake SSP Interaction/ASID datastore

            if (_nrlsApiSetting.Secure && context.Request.IsHttps)
            {
                //var clientAsidMap = _cache.Get<ClientAsidMap>(ClientAsidMap.Key);
                var clientCertificate = context.Connection.ClientCertificate;

                using (var store = new X509Store(StoreName.My, StoreLocation.CurrentUser))
                {
                    //Get ASID
                    var fromAsid = GetHeaderValue(context.Request.Headers, FhirConstants.HeaderFromAsid);
                    if (string.IsNullOrEmpty(fromAsid) || clientCertificate == null)
                    {
                        SetError();
                    }

                    //Check Certificate
                    store.Open(OpenFlags.ReadOnly);

                    var clientCertificates = store.Certificates.Find(X509FindType.FindByThumbprint, clientCertificate.Thumbprint, false);
                    if (clientCertificates.Count < 1)
                    {
                        SetError();
                    }

                    //Check client ASID Thumbprint against Supplied Certificate Thumbprint
                    var client = _sdsService.GetFor(fromAsid);

                    if (client == null || (client.Thumbprint.ToLowerInvariant() != clientCertificate.Thumbprint.ToLowerInvariant()))
                    {
                        SetError();
                    }
                    

                }
            }
            

            await _next.Invoke(context);
            return;

        }

        private void SetError()
        {
            throw new HttpFhirException("Invalid Client Request", OperationOutcomeFactory.CreateAccessDenied(), HttpStatusCode.Unauthorized);
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
    }

    public static class ClientCertificateCheckMiddlewareExtension
    {
        public static IApplicationBuilder UseClientCertificateCheckMiddleware(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<ClientCertificateCheckMiddleware>();
        }
    }
}
