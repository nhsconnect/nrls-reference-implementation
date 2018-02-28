using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using SystemTasks = System.Threading.Tasks;

namespace Demonstrator.NRLSAdapter.Helpers
{
    public class FhirConnector
    {
        public async SystemTasks.Task<T> RequestOne<T>(string requestUrl) where T : Resource
        {
            var fhirResponse = await Request(requestUrl);

            return fhirResponse.GetResource<T>();
        }

        public async SystemTasks.Task<List<T>> RequestMany<T>(string requestUrl) where T : Resource
        {
            var fhirResponse = await Request(requestUrl);

            return fhirResponse.GetResources<T>();
        }

        private async SystemTasks.Task<FhirResponse> Request(string requestUrl)
        {
            var fhirResponse = new FhirResponse();

            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage res = await client.GetAsync(requestUrl))
            using (HttpContent content = res.Content)
            {
                //res.EnsureSuccessStatusCode(); will throw a HttpRequestException to catch in future

                var data = content.ReadAsStreamAsync().Result;

                if (data == null)
                {
                    throw new HttpRequestException($"Request resulted in nothing for: {requestUrl}.");
                }

                using (var reader = new StreamReader(data, Encoding.UTF8))
                {
                    var body = reader.ReadToEnd();
                    var jsonParser = new FhirJsonParser();
                    fhirResponse.Resource = jsonParser.Parse<Resource>(body);

                    if (!res.StatusCode.Equals(HttpStatusCode.OK))
                    {
                        throw new HttpRequestException(new FhirJsonSerializer().SerializeToString(fhirResponse.GetResource<OperationOutcome>()));
                    }


                }
            }

            return await SystemTasks.Task.Run(() => fhirResponse);
        }
    }
}
