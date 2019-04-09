using NRLS_API.Core.Interfaces.Services;
using NRLS_API.Models.Core;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace NRLS_API.Services
{
    //rudementry proxy
    public class SspProxyService : ISspProxyService
    {
        public async Task<CommandResponse> ForwardRequest(CommandRequest request)
        {
            //var internalTraceId = Guid.NewGuid();

            var handler = GetHandler(request);

            var response = new CommandResponse();

            using (var client = new HttpClient(handler))
            {
                var httpRequest = GetMessage(request);
                //string responseMessage = null;

                //LogRequest(httpRequest, request.Resource, internalTraceId);

                using (HttpResponseMessage res = await client.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead))
                using (HttpContent content = res.Content)
                {
                    try
                    {
                        //TODO: upgrade to core 2.2 and use IHttpClientFactory for delegate handling
                        var data = content.ReadAsByteArrayAsync().Result;

                        //responseMessage = Encoding.UTF8.GetString(data, 0, data.Length);

                        if (res.Headers.TransferEncodingChunked == true &&
                            res.Headers.TransferEncoding.Count == 1)
                        {
                            res.Headers.TransferEncoding.Clear();
                        }

                        foreach (var resHeader in res.Headers)
                        {
                            response.Headers.Add(resHeader.Key, resHeader.Value);
                        }


                        var contentLength = content.Headers.ContentLength;

                        foreach (var resHeader in content.Headers)
                        {
                            response.Headers.Add(resHeader.Key, resHeader.Value);
                        }

                        response.Content = data;
                        response.StatusCode = (int)res.StatusCode;

                    }
                    catch(Exception e)
                    {
                        throw new HttpRequestException($"Parsing response from {request.ForwardUrl} failed.");
                    }
                    finally
                    {
                        //LogResponse(res.Headers, (int)res.StatusCode, responseMessage, internalTraceId);
                    }

                }
            }


            return await Task.Run(() => response);
        }

        private HttpClientHandler GetHandler(CommandRequest request)
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

        private HttpRequestMessage GetMessage(CommandRequest request)
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
