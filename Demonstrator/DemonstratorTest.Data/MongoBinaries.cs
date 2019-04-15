using Hl7.Fhir.Model;
using System.Collections.Generic;

namespace DemonstratorTest.Data.Helpers
{
    public static class MongoBinaries
    {
        public static IList<Binary> Binaries
        {
            get
            {
                return new List<Binary>
                {
                    new Binary
                    {
                        Id = "TestBinaryPdf1",
                        Content = new byte[10],
                        ContentType = "application/pdf"                        
                    }
                };
            }
        }

    }
}
