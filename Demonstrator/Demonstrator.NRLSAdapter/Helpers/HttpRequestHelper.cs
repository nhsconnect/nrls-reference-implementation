using Demonstrator.Core.Interfaces.Helpers;
using Demonstrator.Models.Core.Models;
using Demonstrator.NRLSAdapter.Models;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Serialization;
using Microsoft.Net.Http.Headers;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Demonstrator.Core.Helpers
{
    public class HttpRequestHelper : IHttpRequestHelper
    {

        public HttpClientHandler GetClientHandler<T>(T coreRequest) where T : Request
        {
            var handler = new HttpClientHandler();

            var request = coreRequest as CommandRequest;

            if (request.UseSecure)
            {
                handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                handler.SslProtocols = SslProtocols.Tls12;
                handler.ClientCertificates.Add(ClientCertificate(request.ClientThumbprint));
                handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, errors) => ValidateServer(sender, cert, chain, errors, request.ServerThumbprint);
                handler.CheckCertificateRevocationList = false; //TODO: turn this on
            }

            return handler;
        }

        public HttpRequestMessage GetRequestMessage<T>(T coreRequest) where T : Request
        {
            var request = coreRequest as CommandRequest;

            var httpRequest = new HttpRequestMessage()
            {
                RequestUri = new Uri(request.FullUrl.AbsoluteUri),
                Method = request.Method,
                Headers =
                    {
                        //{ HeaderNames.Accept, $"{ContentType.JSON_CONTENT_HEADER}"},
                        { HeaderNames.AcceptEncoding, "gzip, deflate" },
                        { HeaderNames.AcceptLanguage, "en-GB,en" },
                        { HeaderNames.CacheControl, "no-cache" },
                        { HeaderNames.Connection, "Keep-Alive" },
                        { HeaderNames.Host, $"{request.FullUrl.Host}{(":" + request.FullUrl.Port ?? "")}" },
                    }
            };

            //Add additional Spine Headers
            foreach (var header in request.Headers)
            {
                httpRequest.Headers.Add(header.Key, header.Value);
            }

            if (request.Resource != null)
            {
                //Always default to JSON
                var pointerJson = new FhirJsonSerializer().SerializeToString(request.Resource);
                var content = new StringContent(pointerJson, Encoding.UTF8, $"{ContentType.JSON_CONTENT_HEADER }; charset={Encoding.UTF8.WebName}");

                httpRequest.Content = content;
            }

            return httpRequest;
        }

        private bool ValidateServer(HttpRequestMessage sender, X509Certificate2 cert, X509Chain chain, SslPolicyErrors error, string spineThumbprint)
        {
            //Update to ensure we grab certs in a cross platform way
            using (var store = new X509Store(StoreName.My, StoreLocation.CurrentUser))
            {

                store.Open(OpenFlags.ReadOnly);

                //Check that we know the cert and that it is the correct thumbnail
                return store.Certificates.Contains(cert) && !string.IsNullOrEmpty(spineThumbprint) && cert.GetCertHashString() == spineThumbprint && error == SslPolicyErrors.None;

            }
        }

        private X509Certificate2 ClientCertificate(string thumbprint)
        {
            //Update to ensure we grab certs in a cross platform way
            //TODO: change to do check by fqdn and check no chain errors
            using (var store = new X509Store(StoreName.My, StoreLocation.CurrentUser))
            {
                store.Open(OpenFlags.ReadOnly);

                var clientCertificates = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);
                if (clientCertificates.Count < 1)
                {
                    throw new FileNotFoundException($"Certificate {thumbprint} not found.");
                }

                return clientCertificates[0];

            }

        }
    }
}

