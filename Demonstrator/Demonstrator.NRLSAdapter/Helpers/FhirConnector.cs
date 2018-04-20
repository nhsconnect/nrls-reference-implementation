using Demonstrator.NRLSAdapter.Models;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using SystemTasks = System.Threading.Tasks;

namespace Demonstrator.NRLSAdapter.Helpers
{
    public class FhirConnector
    {
        public async SystemTasks.Task<T> RequestOne<T>(CommandRequest request) where T : Resource
        {
            var fhirResponse = await Request(request);

            return fhirResponse.GetResource<T>();
        }

        public async SystemTasks.Task<FhirResponse> RequestOne(CommandRequest request)
        {
            var fhirResponse = await Request(request);

            return fhirResponse;
        }

        public async SystemTasks.Task<List<T>> RequestMany<T>(CommandRequest request) where T : Resource
        {

            var fhirResponse = await Request(request);

            return fhirResponse.GetResources<T>();
        }

        private async SystemTasks.Task<FhirResponse> Request(CommandRequest request)
        {
            var fhirResponse = new FhirResponse();

            var handler = GetHandler(request);

            using (var client = new HttpClient(handler))
            {
                var httpRequest = GetMessage(request);

                using (HttpResponseMessage res = await client.SendAsync(httpRequest))
                using (HttpContent content = res.Content)
                {
                    //res.EnsureSuccessStatusCode(); //will throw a HttpRequestException to catch in future

                    var data = content.ReadAsStreamAsync().Result;

                    if (data == null)
                    {
                        throw new HttpRequestException($"Request resulted in nothing for: {request.FullUrl}.");
                    }

                    if(res.Headers?.Location != null)
                    {
                        fhirResponse.ResponseLocation = res.Headers.Location;
                    }

                    using (var reader = new StreamReader(data, Encoding.UTF8))
                    {
                        try
                        {
                            var body = reader.ReadToEnd();

                            if(!string.IsNullOrEmpty(body))
                            {
                                var jsonParser = new FhirJsonParser();
                                fhirResponse.Resource = jsonParser.Parse<Resource>(body);
                            }

                            if (!res.IsSuccessStatusCode)
                            {
                                throw new HttpRequestException(new FhirJsonSerializer().SerializeToString(fhirResponse.GetResource<OperationOutcome>()));
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new HttpRequestException(ex.Message, ex.InnerException);
                        }
                    }
                }
            }


            return await SystemTasks.Task.Run(() => fhirResponse);
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
                RequestUri = new Uri(request.FullUrl.AbsoluteUri),
                Method = request.Method,
                Headers =
                    {
                        { HttpRequestHeader.Accept.ToString(), $"{ContentType.JSON_CONTENT_HEADER}; {Encoding.UTF8.WebName}"},
                        { HttpRequestHeader.AcceptEncoding.ToString(), "gzip, deflate" },
                        { HttpRequestHeader.AcceptLanguage.ToString(), "en-GB,en" },
                        { HttpRequestHeader.CacheControl.ToString(), "no-cache" },
                        { HttpRequestHeader.Connection.ToString(), "Keep-Alive" },
                        { HttpRequestHeader.Host.ToString(), $"{request.FullUrl.Host}{(":" + request.FullUrl.Port ?? "")}" },
                    }
            };

            //Add additional Spine Headers
            foreach (var header in request.Headers)
            {
                httpRequest.Headers.Add(header.Key, header.Value);
            }

            if (request.Content != null)
            {
                httpRequest.Content = request.Content;
            }

            return httpRequest;
        }

        private bool ValidateServer(HttpRequestMessage sender, X509Certificate2 cert, X509Chain chain, SslPolicyErrors error, string spineThumbprint)
        {
            //Update to ensure we grab certs in a cross platform way
            using (var store = new X509Store(StoreName.TrustedPeople, StoreLocation.LocalMachine))
            {

                store.Open(OpenFlags.ReadOnly);

                //Check that we know the cert and that it is the correct thumbnail
                return store.Certificates.Contains(cert) && !string.IsNullOrEmpty(spineThumbprint) && cert.GetCertHashString() == spineThumbprint && error == SslPolicyErrors.None;

            }
        }

        private X509Certificate2 ClientCertificate(string thumbprint)
        {
            //Update to ensure we grab certs in a cross platform way
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
