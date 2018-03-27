using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System;
using System.Text;
using SystemTask = System.Threading.Tasks;

namespace NRLS_API.WebApp.Core.Formatters
{
    public class FhirXmlOutputFormatter : TextOutputFormatter
    {
        public FhirXmlOutputFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/fhir+xml"));
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/xml+fhir")); // DSTU2 support as per NRLS spec
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
                var resource = new FhirXmlSerializer().SerializeToString(context.Object as Resource);

                buffer.Append(resource);
            }

            return response.WriteAsync(buffer.ToString(), Encoding.UTF8);
        }

    }
}
