using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;

namespace Demonstrator.NRLSAdapter.Helpers.Models
{
    public static class NrlsPointer
    {
        public static DocumentReference Generate(string profile, string orgCode, string nhsNumber, string recordUrl, string recordContentType, string typeCode, string typeDisplay)
        {
            var pointer = new DocumentReference
            {
                Meta = new Meta
                {
                    Profile = new List<string>
                    {
                        profile
                    }
                },
                Status = DocumentReferenceStatus.Current,
                Author = new List<ResourceReference>()
                    {
                        new ResourceReference
                        {
                            Reference = $"{FhirConstants.SystemODS}{orgCode}"
                        }
                    },
                Custodian = new ResourceReference
                {
                    Reference = $"{FhirConstants.SystemODS}{orgCode}"
                },
                Subject = new ResourceReference
                {
                    Reference = $"{FhirConstants.SystemPDS}{nhsNumber}"
                },
                Indexed = DateTime.UtcNow,
                Type = new CodeableConcept() {
                    Coding = new List<Coding>()
                    {
                        new Coding(FhirConstants.CodingSystemPointerType, typeCode, typeDisplay)
                    }
                },
                Content = new List<DocumentReference.ContentComponent>()
                    {
                        new DocumentReference.ContentComponent
                        {
                            Attachment = new Attachment
                            {
                                ContentType = recordContentType,
                                Url = recordUrl,
                                Creation = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz")
                            }
                        }
                    }

            };

            return pointer;
        }
    }
}
