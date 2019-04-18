using NRLS_API.Core.Interfaces.Helpers;
using NRLS_API.Models.Core;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace NRLS_API.Core.Helpers
{
    public class HttpRequestHelper : IHttpRequestHelper
    {

        public HttpClientHandler GetClientHandler(CommandRequest request)
        {
            var handler = new HttpClientHandler();

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

        public HttpRequestMessage GetRequestMessage(CommandRequest request)
        {
            var httpRequest = new HttpRequestMessage()
            {
                RequestUri = new Uri(request.ForwardUrl.AbsoluteUri),
                Method = request.Method,
                Headers =
                    {
                        //{ "Host", request.Forwarded.Host },
                        { "Forwarded", $"by={request.Forwarded.By};for={request.Forwarded.For};host={request.Forwarded.Host};proto={request.Forwarded.Protocol}" }
                        //{ "X-Rate-Limit", "20" },
                        //{ "X-Rate-Limit-Remaining", "5" },
                        //{ "X-Rate-Limit-Reset", "2000" }
                    }
            };

            //Add additional Spine Headers
            foreach (var header in request.Headers)
            {
                httpRequest.Headers.Add(header.Key, header.Value);
            }

            return httpRequest;
        }

        private bool ValidateServer(HttpRequestMessage sender, X509Certificate2 cert, X509Chain chain, SslPolicyErrors error, string providerThumbprint)
        {
            //Update to ensure we grab certs in a cross platform way
            using (var store = new X509Store(StoreName.My, StoreLocation.CurrentUser))
            {

                store.Open(OpenFlags.ReadOnly);

                //Check that we know the cert and that it is the correct thumbnail
                return store.Certificates.Contains(cert) && !string.IsNullOrEmpty(providerThumbprint) && cert.GetCertHashString() == providerThumbprint && error == SslPolicyErrors.None;

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
