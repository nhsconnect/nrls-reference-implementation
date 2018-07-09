using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NRLS_API.Core.Exceptions;
using NRLS_API.Core.Factories;
using NRLS_API.Core.Helpers;
using NRLS_API.Core.Resources;
using NRLS_API.Models.Core;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace NRLS_API.WebApp.Core.Middlewares
{
    public class ClientCertificateCheckMiddleware
    {
        private readonly RequestDelegate _next;
        private IMemoryCache _cache;
        private ApiSetting _nrlsApiSetting;

        public ClientCertificateCheckMiddleware( RequestDelegate next, IMemoryCache memoryCache)
        {
            _next = next;
            _cache = memoryCache;
        }

        public async Task Invoke(HttpContext context, IOptionsSnapshot<ApiSetting> apiSettings)
        {
            _nrlsApiSetting = apiSettings.Get("NrlsApiSetting");

            //Fake SSP Interaction/ASID datastore

            if (_nrlsApiSetting.Secure)
            {
                var clientAsidMap = _cache.Get<ClientAsidMap>(ClientAsidMap.Key);
                var clientCertificate = context.Connection.ClientCertificate;

                using (var store = new X509Store(StoreName.TrustedPeople, StoreLocation.CurrentUser))
                {
                    //Get ASID
                    var fromAsid = GetHeaderValue(context.Request.Headers, FhirConstants.HeaderFromAsid);
                    if (string.IsNullOrEmpty(fromAsid))
                    {
                        SetError();
                    }

                    //Check Certificate
                    store.Open(OpenFlags.ReadOnly);

                    if (!store.Certificates.Contains(clientCertificate))
                    {
                        SetError();
                    }

                    //Check client ASID Thumbprint against Supplied Certificate Thumbprint
                    var clientmap = _cache.Get<ClientAsidMap>(ClientAsidMap.Key);

                    if (clientmap != null)
                    {
                        var client = clientmap.ClientAsids.FirstOrDefault(x => x.Key == fromAsid);

                        if (client.Value != null && client.Value.Thumbprint != clientCertificate.Thumbprint)
                        {
                            SetError();
                        }
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
