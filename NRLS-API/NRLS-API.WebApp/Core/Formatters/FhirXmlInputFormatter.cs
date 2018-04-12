using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using NRLS_API.Core.Exceptions;
using NRLS_API.Core.Factories;
using System;
using System.Net;
using System.Text;
using System.Xml;
using SystemTask = System.Threading.Tasks;

namespace NRLS_API.WebApp.Core.Formatters
{
    public class FhirXmlInputFormatter : TextInputFormatter
    {
        public FhirXmlInputFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/fhir+xml"));
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/xml+fhir"));
            SupportedEncodings.Clear();
            SupportedEncodings.Add(Encoding.UTF8);
        }

        protected override bool CanReadType(Type type)
        {
            if (typeof(Resource).IsAssignableFrom(type))
            {
                return base.CanReadType(type);
            }
            return false;
        }

        public override SystemTask.Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
        {
            var request = context.HttpContext.Request;

            using (var streamReader = context.ReaderFactory(request.Body, encoding))
            using (XmlTextReader xmlReader = new XmlTextReader(streamReader))
            {
                var type = context.ModelType;

                try
                {
                    //TODO: create a dumb model to allow passthrough to validation
                    var resource = new FhirXmlParser().Parse(xmlReader, type);
                    return InputFormatterResult.SuccessAsync(resource);
                }
                catch (Exception ex)
                {
                    //TODO: Remove Fhir Hack
                    if (ex != null)
                    {
                        return InputFormatterResult.SuccessAsync(OperationOutcomeFactory.CreateInvalidResource("Unknown", ex.Message));
                    }

                    return InputFormatterResult.FailureAsync();
                }
            }
        }

    }
}
