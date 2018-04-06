using Demonstrator.Core.Exceptions;
using Demonstrator.NRLSAdapter.Models;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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

        public async SystemTasks.Task<List<T>> RequestMany<T>(CommandRequest request) where T : Resource
        {

            var fhirResponse = await Request(request);

            return fhirResponse.GetResources<T>();
        }

        private async SystemTasks.Task<FhirResponse> Request(CommandRequest request)
        {
            var fhirResponse = new FhirResponse();

            using (HttpClient client = new HttpClient())
            {
                //need to tidy this up, move out of this file and expand the request to include SSP stuff
                //also include SSL
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
                        { HttpRequestHeader.Authorization.ToString(), "authtoken" }
                    }
                };

                //Add additional Spine Headers
                foreach(var header in request.Headers)
                {
                    httpRequest.Headers.Add(header.Key, header.Value);
                }

                if (request.Content != null)
                {
                    httpRequest.Content = request.Content;
                }

                using (HttpResponseMessage res = await client.SendAsync(httpRequest))
                using (HttpContent content = res.Content)
                {
                    //res.EnsureSuccessStatusCode(); will throw a HttpRequestException to catch in future

                    var data = content.ReadAsStreamAsync().Result;

                    if (request.Method != HttpMethod.Delete && data == null)
                    {
                        throw new HttpRequestException($"Request resulted in nothing for: {request.FullUrl}.");
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
    }
}
