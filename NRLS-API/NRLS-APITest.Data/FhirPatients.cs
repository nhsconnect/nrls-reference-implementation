using Hl7.Fhir.Model;
using System.Collections.Generic;

namespace NRLS_APITest.Data
{
    public class FhirPatients
    {
        public static Patient Valid_Patient
        {
            get
            {
                return new Patient
                {
                    Id = "5d5aa12021b74204d2e5a708",
                    Identifier = new List<Identifier> {
                            new Identifier {
                                System = "https://fhir.nhs.uk/Id/nhs-number",
                                Value = "1445545101"
                            }
                        }
                };
            }
        }
    }
}
