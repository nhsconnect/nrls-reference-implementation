using Demonstrator.Core.Exceptions;
using Demonstrator.Core.Factories;
using Demonstrator.Core.Interfaces.Helpers;
using Demonstrator.Core.Interfaces.Services.Fhir;
using Demonstrator.Models.Core.Models;
using Demonstrator.NRLSAdapter.Models;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Serialization;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using SystemTasks = System.Threading.Tasks;

namespace Demonstrator.NRLSAdapter.Helpers
{
    public class FhirConnector : IFhirConnector
    {
        private readonly ILoggingHelper _loggingHelper;

        public FhirConnector(ILoggingHelper loggingHelper)
        {
            _loggingHelper = loggingHelper;
        }

        public async SystemTasks.Task<Tout> RequestOneFhir<Tin, Tout>(Tin request) where Tin : Request where Tout : Resource
        {
            var fhirResponse = await Request(request as CommandRequest);

            return fhirResponse.GetResource<Tout>();
        }

        public async SystemTasks.Task<Tout> RequestOne<Tin, Tout>(Tin request) where Tin : Request where Tout : Response
        {
            var fhirResponse = await Request(request as CommandRequest);

            return fhirResponse as Tout;
        }

        public async SystemTasks.Task<List<Tout>> RequestMany<Tin, Tout>(Tin request) where Tin : Request where Tout : Resource
        {

            var fhirResponse = await Request(request as CommandRequest);

            return fhirResponse.GetResources<Tout>();
        }

        private async SystemTasks.Task<FhirResponse> Request(CommandRequest request)
        {
            var internalTraceId = Guid.NewGuid();
            var fhirResponse = new FhirResponse();

            var handler = GetHandler(request);

            using (var client = new HttpClient(handler))
            {
                var httpRequest = GetMessage(request);

                //LogRequest(httpRequest, request.Resource, internalTraceId);

                using (HttpResponseMessage res = await client.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead))
                using (HttpContent content = res.Content)
                {
                    //res.EnsureSuccessStatusCode(); //will throw a HttpRequestException to catch in future

                    var mediaType = content.Headers.ContentType?.MediaType?.ToLowerInvariant();

                    if (res.Headers?.Location != null)
                    {
                        fhirResponse.ResponseLocation = res.Headers.Location;
                    }

                    //TODO: upgrade to core 2.2 and use IHttpClientFactory 
                    // for delegate handling and retries policy
                    if (!string.IsNullOrEmpty(mediaType) && mediaType.Contains("fhir"))
                    {
                        fhirResponse = await ParseResource(content, request, fhirResponse);
                    }
                    else
                    {
                        fhirResponse = await ParseBinary(content, request, fhirResponse);
                    }

                    //get content from fhirResponse
                    //string responseMessage = null;


                    //LogResponse(res.Headers, (int)res.StatusCode, responseMessage, internalTraceId);

                    if (!res.IsSuccessStatusCode)
                    {
                        var diagnostics = $"{res.StatusCode} encountered for the URL {httpRequest.RequestUri.AbsoluteUri}";

                        var errorResource =
                            (fhirResponse.Resource.ResourceType != ResourceType.OperationOutcome) ?
                                OperationOutcomeFactory.CreateGenericError(diagnostics) :
                                fhirResponse.GetResource<OperationOutcome>();

                        throw new HttpFhirException("Request resulted in an error.", errorResource, res.StatusCode);
                    }
                }
            }


            return await SystemTasks.Task.Run(() => fhirResponse);
        }

        private async SystemTasks.Task<FhirResponse> ParseResource(HttpContent content, CommandRequest request, FhirResponse fhirResponse)
        {
            var data = await content.ReadAsStreamAsync();

            if (data == null)
            {
                throw new HttpRequestException($"Request resulted in nothing for: {request.FullUrl}.");
            }

            using (var reader = new StreamReader(data, Encoding.UTF8))
            {
                try
                {
                    var mediaType = content.Headers.ContentType.MediaType.ToLowerInvariant();

                    var body = reader.ReadToEnd();

                    if (!string.IsNullOrEmpty(body))
                    {
                        if(mediaType.Contains("xml"))
                        {
                            var xmlParser = new FhirXmlParser();
                            fhirResponse.Resource = xmlParser.Parse<Resource>(body);
                        }
                        else
                        {
                            var jsonParser = new FhirJsonParser();
                            fhirResponse.Resource = jsonParser.Parse<Resource>(body);
                        }

                    }
                }
                catch (Exception ex)
                {
                    throw new HttpRequestException(ex.Message, ex.InnerException);
                }
            }

            return fhirResponse;
        }

        private async SystemTasks.Task<FhirResponse> ParseBinary(HttpContent content, CommandRequest request, FhirResponse fhirResponse)
        {
            var binaryResource = new Binary();

            var data = await content.ReadAsByteArrayAsync();

            if (data == null)
            {
                throw new HttpRequestException($"Request resulted in nothing for: {request.FullUrl}.");
            }

            try
            {
                binaryResource.Content = data;
                binaryResource.ContentType = content.Headers.ContentType?.MediaType;

                fhirResponse.Resource = binaryResource;
            }
            catch (Exception ex)
            {
                throw new HttpRequestException(ex.Message, ex.InnerException);
            }

            return fhirResponse;
        }

        private void LogRequest(HttpRequestMessage httpRequest, Resource resource, Guid internalTraceId)
        {
            var pointerJson = resource != null ? new FhirJsonSerializer().SerializeToString(resource) : string.Empty;

            _loggingHelper.LogHttpRequestMessage(httpRequest.Headers, httpRequest.Version, httpRequest.Method, httpRequest.RequestUri, pointerJson, internalTraceId);
        }

        private void LogResponse(HttpResponseHeaders headers, int statusCode, string bodyAsText, Guid internalTraceId)
        {
            _loggingHelper.LogHttpResponseMessage(headers, statusCode, bodyAsText, internalTraceId);
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
