using Demonstrator.Core.Exceptions;
using Demonstrator.Core.Factories;
using Demonstrator.Core.Interfaces.Helpers;
using Demonstrator.Core.Interfaces.Services.Fhir;
using Demonstrator.Models.Core.Models;
using Demonstrator.NRLSAdapter.Models;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using SystemTasks = System.Threading.Tasks;

namespace Demonstrator.NRLSAdapter.Helpers
{
    public class FhirConnector : IFhirConnector
    {
        private readonly ILoggingHelper _loggingHelper;
        private readonly IHttpRequestHelper _httpRequestHelper;

        public FhirConnector(ILoggingHelper loggingHelper, IHttpRequestHelper httpRequestHelper)
        {
            _loggingHelper = loggingHelper;
            _httpRequestHelper = httpRequestHelper;
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

            var handler = _httpRequestHelper.GetClientHandler(request);

            using (var client = new HttpClient(handler))
            {
                var httpRequest = _httpRequestHelper.GetRequestMessage(request);

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

        
    }
}
