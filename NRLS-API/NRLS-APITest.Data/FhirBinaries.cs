using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using System;
using System.Text;

namespace NRLS_APITest.Data
{
    public static class FhirBinaries
    {
        public static string AsJsonString(this Resource resource)
        {
            return new FhirJsonSerializer().SerializeToString(resource);
        }

        public static Binary Html
        {
            get
            {
                return new Binary
                {
                    Content = Encoding.UTF8.GetBytes("<p>Hello</p>"),
                    ContentType = "text/html"
                };
            }
        }
    }
}
