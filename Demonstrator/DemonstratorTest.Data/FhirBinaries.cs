using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using System.Text;

namespace DemonstratorTest.Data
{
    public static class FhirBinaries
    {
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

        public static Bundle AsBundle
        {
            get
            {
                return new Bundle
                {
                    Type = Bundle.BundleType.Searchset,
                    Entry =
                    {
                        new Bundle.EntryComponent
                        {
                            Resource = new Binary
                            {
                                Content = Encoding.UTF8.GetBytes("<p>Hello</p>"),
                                ContentType = "text/html"
                            }
                        },
                        new Bundle.EntryComponent
                        {
                            Resource = new Binary
                            {
                                Content = Encoding.UTF8.GetBytes("pdf.file"),
                                ContentType = "application/pdf"
                            }
                        }
                    }
                };
            }
        }
    }
}
