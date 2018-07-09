using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System;
using System.Net;
using System.Text;
using SystemTask = System.Threading.Tasks;

namespace NRLS_API.WebApp.Core.Formatters
{
    public class FhirJsonOutputFormatter : TextOutputFormatter
    {
        public FhirJsonOutputFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/fhir+json"));
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/json+fhir")); // DSTU2 support as per NRLS spec
            SupportedEncodings.Clear();
            SupportedEncodings.Add(Encoding.UTF8);
        }

        protected override bool CanWriteType(Type type)
        {
            if (typeof(Resource).IsAssignableFrom(type))
            {
                return base.CanWriteType(type);
            }
            return false;
        }

        public override SystemTask.Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            var response = context.HttpContext.Response;

            var buffer = new StringBuilder();

            if(context.ObjectType == typeof(OperationOutcome) || typeof(Resource).IsAssignableFrom(context.ObjectType))
            {
                var resource = new FhirJsonSerializer().SerializeToString(context.Object as Resource);

                buffer.Append(resource);
            }

            response.Headers.Remove(HttpResponseHeader.ContentType.ToString());
            response.Headers.Add(HttpResponseHeader.ContentType.ToString(), $"{ContentType.XML_CONTENT_HEADER}; {Encoding.UTF8.WebName}");

            return response.WriteAsync(buffer.ToString(), Encoding.UTF8);
        }

    }
}
