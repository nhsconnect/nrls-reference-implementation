using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace DemonstratorTest.Data.FhirModels
{
    public static class DocumentReferences
    {
        public static DocumentReference ValidFull_DocumentReference
        {
            get
            {
                var model = new DocumentReference
                {
                    Content = new List<DocumentReference.ContentComponent>
                    {
                        new DocumentReference.ContentComponent
                        {
                            Attachment = new Attachment
                            {
                                ContentType = "application/pdf",
                                Creation = "2019-01-01T10:13:53.000+00:00",
                                Title = "My Attachement",
                                Url = "baseUrl/link-to-attachment"
                            }
                        }
                    },
                    Created = null,
                    Custodian = new ResourceReference
                    {
                        Reference = "ods-system/id"
                    },
                    Id = "abc",
                    Status = DocumentReferenceStatus.Current,
                    Identifier = new List<Identifier>
                    {
                        new Identifier
                        {
                            System = "MasterSystem",
                            Value = "MasterId"
                        }
                    },
                    Subject = new ResourceReference
                    {
                        Reference = "pds-system/id"
                    },
                    Type = new CodeableConcept
                    {
                        Coding = new List<Coding>
                        {
                            new Coding
                            {
                                System = "TypeSystem",
                                Code = "TypeCode",
                                Display = "TypeDisplay"
                            }
                        }
                    },
                    Author = new List<ResourceReference>
                    {
                        new ResourceReference
                        {
                            Reference = "ods-system/id"
                        }
                    },
                    Indexed = new DateTimeOffset(new DateTime(2019, 01, 01, 12, 33, 14), new TimeSpan(0, 0, 0))
                };

                return model;
            }
        }

        public static DocumentReference InvalidFull_DocumentReference_NullReferenceList
        {
            get
            {
                var model = new DocumentReference
                {
                    Content = new List<DocumentReference.ContentComponent>
                    {
                        new DocumentReference.ContentComponent
                        {
                            Attachment = new Attachment
                            {
                                ContentType = "application/pdf",
                                Creation = "2019-01-01T10:13:53.000+00:00",
                                Title = "My Attachement",
                                Url = "link-to-attachment"
                            }
                        }
                    },
                    Created = null,
                    Custodian = new ResourceReference
                    {
                        Reference = "ods-system/id"
                    },
                    Id = "abc",
                    Status = DocumentReferenceStatus.Current,
                    Identifier = new List<Identifier>
                    {
                        new Identifier
                        {
                            System = "MasterSystem",
                            Value = "MasterId"
                        }
                    },
                    Subject = new ResourceReference
                    {
                        Reference = "pds-system/id"
                    },
                    Type = new CodeableConcept
                    {
                        Coding = new List<Coding>
                        {
                            new Coding
                            {
                                System = "TypeSystem",
                                Code = "TypeCode",
                                Display = "TypeDisplay"
                            }
                        }
                    },
                    Author = null,
                    Indexed = new DateTimeOffset(new DateTime(2019, 01, 01, 12, 33, 14), new TimeSpan(0, 0, 0))
                };

                return model;
            }
        }

        public static DocumentReference InvalidFull_DocumentReference_NullReference
        {
            get
            {
                var model = new DocumentReference
                {
                    Content = new List<DocumentReference.ContentComponent>
                    {
                        new DocumentReference.ContentComponent
                        {
                            Attachment = new Attachment
                            {
                                ContentType = "application/pdf",
                                Creation = "2019-01-01T10:13:53.000+00:00",
                                Title = "My Attachement",
                                Url = "link-to-attachment"
                            }
                        }
                    },
                    Created = null,
                    Custodian = null,
                    Id = "abc",
                    Status = DocumentReferenceStatus.Current,
                    Identifier = new List<Identifier>
                    {
                        new Identifier
                        {
                            System = "MasterSystem",
                            Value = "MasterId"
                        }
                    },
                    Subject = new ResourceReference
                    {
                        Reference = "pds-system/id"
                    },
                    Type = new CodeableConcept
                    {
                        Coding = new List<Coding>
                        {
                            new Coding
                            {
                                System = "TypeSystem",
                                Code = "TypeCode",
                                Display = "TypeDisplay"
                            }
                        }
                    },
                    Author = null,
                    Indexed = new DateTimeOffset(new DateTime(2019, 01, 01, 12, 33, 14), new TimeSpan(0, 0, 0))
                };

                return model;
            }
        }

        public static DocumentReference InvalidFull_DocumentReference_NullContent
        {
            get
            {
                var model = new DocumentReference
                {
                    Content = null,
                    Created = null,
                    Custodian = new ResourceReference
                    {
                        Reference = "ods-system/id"
                    },
                    Id = "abc",
                    Status = DocumentReferenceStatus.Current,
                    Identifier = new List<Identifier>
                    {
                        new Identifier
                        {
                            System = "MasterSystem",
                            Value = "MasterId"
                        }
                    },
                    Subject = new ResourceReference
                    {
                        Reference = "pds-system/id"
                    },
                    Type = new CodeableConcept
                    {
                        Coding = new List<Coding>
                        {
                            new Coding
                            {
                                System = "TypeSystem",
                                Code = "TypeCode",
                                Display = "TypeDisplay"
                            }
                        }
                    },
                    Author = null,
                    Indexed = new DateTimeOffset(new DateTime(2019, 01, 01, 12, 33, 14), new TimeSpan(0, 0, 0))
                };

                return model;
            }
        }

        public static DocumentReference InvalidFull_DocumentReference_NullCodeableConcept
        {
            get
            {
                var model = new DocumentReference
                {
                    Content = new List<DocumentReference.ContentComponent>
                    {
                        new DocumentReference.ContentComponent
                        {
                            Attachment = new Attachment
                            {
                                ContentType = "application/pdf",
                                Creation = "2019-01-01T10:13:53.000+00:00",
                                Title = "My Attachement",
                                Url = "link-to-attachment"
                            }
                        }
                    },
                    Created = null,
                    Custodian = new ResourceReference
                    {
                        Reference = "ods-system/id"
                    },
                    Id = "abc",
                    Status = DocumentReferenceStatus.Current,
                    Identifier = new List<Identifier>
                    {
                        new Identifier
                        {
                            System = "MasterSystem",
                            Value = "MasterId"
                        }
                    },
                    Subject = new ResourceReference
                    {
                        Reference = "pds-system/id"
                    },
                    Type = null,
                    Author = null,
                    Indexed = new DateTimeOffset(new DateTime(2019, 01, 01, 12, 33, 14), new TimeSpan(0, 0, 0))
                };

                return model;
            }
        }
    }
}
