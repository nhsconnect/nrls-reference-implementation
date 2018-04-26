using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;

namespace DemonstratorTest.Data.Helpers
{
    public static class MongoDocumentReferences
    {
        public static IList<DocumentReference> DocumentReferences
        {
            get
            {
                var documentReferences =  new List<DocumentReference>();

                documentReferences.AddRange(DocumentReferences_2686033207);
                documentReferences.AddRange(DocumentReferences_3656987882);

                return documentReferences;
            }
        }

        public static IList<DocumentReference> DocumentReferences_2686033207
        {
            get
            {
                return new List<DocumentReference>
                {
                    new DocumentReference
                    {
                        Id = "5ab13f41957d0ad5d93a1339",
                        Status = DocumentReferenceStatus.Current,
                        Type = new CodeableConcept
                        {
                            Coding = new List<Coding>
                            {
                                new Coding
                                {
                                    System = "http://snomed.info/sct",
                                    Code = "718347000",
                                    Display = "Mental health care plan"
                                }
                            }
                        },
                        Subject = new ResourceReference
                        {
                            Reference = "https://demographics.spineservices.nhs.uk/STU3/Patient/2686033207"
                        },
                        Indexed = new DateTime(2005, 12, 24, 9, 43, 41),
                        Author = new List<ResourceReference>
                        {
                            new ResourceReference
                            {
                                Reference = "https://directory.spineservices.nhs.uk/STU3/Organization/1XR"
                            }
                        },
                        Custodian = new ResourceReference
                        {
                            Reference = "https://directory.spineservices.nhs.uk/STU3/Organization/1XR"
                        },
                        Content = new List<DocumentReference.ContentComponent>
                        {
                            new DocumentReference.ContentComponent
                            {
                                Attachment = new Attachment
                                {
                                    ContentType = "application/pdf",
                                    Url = "http://example.org/xds/mhd/Binary/07a6483f-732b-461e-86b6-edb665c45510.pdf",
                                    Title = "Mental health Care Plan Report",
                                    Creation = "2016-03-08T15:26:00+00:00"
                                }
                            }
                        }
                    },
                    new DocumentReference
                    {
                        Id = "5ab13f4a957d0ad5d93a133a",
                        Status = DocumentReferenceStatus.Current,
                        Type = new CodeableConcept
                        {
                            Coding = new List<Coding>
                            {
                                new Coding
                                {
                                    System = "http://snomed.info/sct",
                                    Code = "718347002",
                                    Display = "Cancer care plan"
                                }
                            }
                        },
                        Subject = new ResourceReference
                        {
                            Reference = "https://demographics.spineservices.nhs.uk/STU3/Patient/2686033207"
                        },
                        Indexed = new DateTime(2011, 07, 19, 11, 27, 16),
                        Author = new List<ResourceReference>
                        {
                            new ResourceReference
                            {
                                Reference = "https://directory.spineservices.nhs.uk/STU3/Organization/1XR"
                            }
                        },
                        Custodian = new ResourceReference
                        {
                            Reference = "https://directory.spineservices.nhs.uk/STU3/Organization/1XR"
                        },
                        Content = new List<DocumentReference.ContentComponent>
                        {
                            new DocumentReference.ContentComponent
                            {
                                Attachment = new Attachment
                                {
                                    ContentType = "application/pdf",
                                    Url = "http://example.org/xds/mhd/Binary/07a6483f-732b-461e-86b6-edb665c45510.pdf",
                                    Title = "Cancer Care Plan Report",
                                    Creation = "2011-07-19T11:27:16+00:00"
                                }
                            }
                        }
                    }
                };
            }
        }

        public static IList<DocumentReference> DocumentReferences_3656987882
        {
            get
            {
                return new List<DocumentReference>
                {
                    new DocumentReference
                    {
                        Id = "5aba0f464f02ced4c7eb16c4",
                        Status = DocumentReferenceStatus.Current,
                        Type = new CodeableConcept
                        {
                            Coding = new List<Coding>
                            {
                                new Coding
                                {
                                    System = "http://snomed.info/sct",
                                    Code = "718347002",
                                    Display = "Mental health care plan"
                                }
                            }
                        },
                        Subject = new ResourceReference
                        {
                            Reference = "https://demographics.spineservices.nhs.uk/STU3/Patient/3656987882"
                        },
                        Indexed = new DateTime(2011, 06, 21, 11, 45, 16),
                        Author = new List<ResourceReference>
                        {
                            new ResourceReference
                            {
                                Reference = "https://directory.spineservices.nhs.uk/STU3/Organization/00003X"
                            }
                        },
                        Custodian = new ResourceReference
                        {
                            Reference = "https://directory.spineservices.nhs.uk/STU3/Organization/00003X"
                        },
                        Content = new List<DocumentReference.ContentComponent>
                        {
                            new DocumentReference.ContentComponent
                            {
                                Attachment = new Attachment
                                {
                                    ContentType = "application/pdf",
                                    Url = "http://example.org/xds/mhd/Binary/07a6483f-732b-461e-86b6-edb665c45510.pdf",
                                    Title = "Mental health Care Plan Report",
                                    Creation = "2011-06-21T11:45:16+00:00"
                                }
                            }
                        }
                    }
                };
            }
        }

    }
}
