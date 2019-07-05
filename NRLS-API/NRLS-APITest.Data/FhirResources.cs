using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NRLS_APITest.Data
{
    public class FhirResources
    {
        public static List<DocumentReference.ContentComponent> Valid_Content
        {
            get
            {
                return new List<DocumentReference.ContentComponent>()
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
                };
            }
        }

        public static List<DocumentReference.ContentComponent> Invalid_Url
        {
            get
            {
                var content = Valid_Content;
                content.First().Attachment.Url = "http://.";

                return content;
            }
        }

        public static List<DocumentReference.ContentComponent> Missing_Url
        {
            get
            {
                var content = Valid_Content;
                content.First().Attachment.Url = null;

                return content;
            }
        }

        public static List<DocumentReference.ContentComponent> Invalid_ContentType
        {
            get
            {
                var content = Valid_Content;
                content.First().Attachment.ContentType = null;

                return content;
            }
        }

        public static DocumentReference.RelatesToComponent Valid_Single_RelatesTo
        {
            get
            {
                var validRelatesTo = new DocumentReference.RelatesToComponent
                {
                    Code = DocumentRelationshipType.Replaces,
                    Target = new ResourceReference
                    {
                        Identifier = new Identifier("urn:ietf:rfc:4151", "urn:tag:humber.nhs.uk,2004:cdc:600009612669")
                    }
                };

                return validRelatesTo;
            }
        }

        public static DocumentReference.RelatesToComponent Invalid_Single_RelatesTo_BadTarget
        {
            get
            {
                var validRelatesTo = Valid_Single_RelatesTo;
                validRelatesTo.Target.Identifier = null;

                return validRelatesTo;
            }
        }

        public static DocumentReference.RelatesToComponent Invalid_Single_RelatesTo_BadTargetValue
        {
            get
            {
                var validRelatesTo = Valid_Single_RelatesTo;
                validRelatesTo.Target.Identifier.Value = null;

                return validRelatesTo;
            }
        }

        public static DocumentReference.RelatesToComponent Invalid_Single_RelatesTo_BadTargetSystem
        {
            get
            {
                var validRelatesTo = Valid_Single_RelatesTo;
                validRelatesTo.Target.Identifier.System = null;

                return validRelatesTo;
            }
        }

        public static ValueSet ValueSet_NrlsType
        {
            get
            {
                var content = new ValueSet
                {
                    Id = "NRLS-RecordType-1",
                    Text = new Narrative
                    {
                        Status = Narrative.NarrativeStatus.Generated
                    },
                    Url = "https://fhir.nhs.uk/STU3/ValueSet/NRLS-RecordType-1",
                    Version = "1.0.0",
                    Name = "NRLS Record Type",
                    Status = PublicationStatus.Draft,
                    Date = "2018-05-25T00:00:00+00:00",
                    Publisher = "NHS Digital",
                    Contact = new List<ContactDetail>()
                    {
                        new ContactDetail
                        {
                            Name = "Interoperability Team",
                            Telecom = new List<ContactPoint>()
                            {
                                new ContactPoint
                                {
                                    System = ContactPoint.ContactPointSystem.Email,
                                    Value = "interoperabilityteam@nhs.net",
                                    Use = ContactPoint.ContactPointUse.Work
                                }
                            }
                        }
                    },
                    Description = new Markdown {
                        Value = "A code from the SNOMED Clinical Terminology UK coding system to represent the NRLS clinical record type."
                    },
                    Copyright = new Markdown
                    {
                        Value = "This value set includes content from SNOMED CT, which is copyright © 2002 + International Health Terminology Standards Development Organisation(IHTSDO), and distributed by agreement between IHTSDO and HL7.Implementer use of SNOMED CT is not covered by this agreement."
                    },
                    Compose = new ValueSet.ComposeComponent
                    {
                        Include = new List<ValueSet.ConceptSetComponent>()
                        {
                            new ValueSet.ConceptSetComponent
                            {
                                System = "http://snomed.info/sct",
                                Concept = new List<ValueSet.ConceptReferenceComponent>()
                                {
                                    new ValueSet.ConceptReferenceComponent
                                    {
                                        Code = "736253002",
                                        Display = "Mental health crisis plan (record artifact)"
                                    }
                                }
                            }
                        }
                    }
                };

                return content;
            }
        }

        public static StructureDefinition SD_NrlsPointer
        {
            get
            {
                var content = new StructureDefinition
                {
                    Id = "NRLS-DocumentReference-1",
                    Meta = new Meta
                    {
                        LastUpdated = new DateTime(2018, 05, 29, 12, 06, 47, 158, DateTimeKind.Local)
                    },
                    Text = new Narrative
                    {
                        Status = Narrative.NarrativeStatus.Generated
                    },
                    Url = "https://fhir.nhs.uk/STU3/StructureDefinition/NRLS-DocumentReference-1",
                    Version = "1.1.0",
                    Name = "NRLS-DocumentReference-1",
                    Title = "NRLS-DocumentReference-1",
                    Status = PublicationStatus.Draft,
                    Date = "2017-11-20",
                    Publisher = "NHS Digital",
                    Contact = new List<ContactDetail>()
                    {
                        new ContactDetail
                        {
                            Name = "Interoperability Team",
                            Telecom = new List<ContactPoint>()
                            {
                                new ContactPoint
                                {
                                    System = ContactPoint.ContactPointSystem.Email,
                                    Value = "interoperabilityteam@nhs.net",
                                    Use = ContactPoint.ContactPointUse.Work
                                }
                            }
                        }

                    },
                    Description = new Markdown
                    {
                        Value = "NRLS-DocumentReference-1 reduced for testing purposes only!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!"
                    },
                    Copyright = new Markdown
                    {
                        Value = "Copyright © 2017 NHS Digital"
                    },
                    FhirVersion = "3.0.1",
                    Kind = StructureDefinition.StructureDefinitionKind.Resource,
                    Abstract = false,
                    Type = "DocumentReference",
                    BaseDefinition = "http://hl7.org/fhir/StructureDefinition/DocumentReference",
                    Snapshot = new StructureDefinition.SnapshotComponent
                    {
                        Element = new List<ElementDefinition>()
                        {
                            new ElementDefinition
                            {
                                ElementId = "DocumentReference",
                                Path = "DocumentReference",
                                Min = 0,
                                Max = "*",
                                Base = new ElementDefinition.BaseComponent
                                {
                                    Path = "Resource",
                                    Min = 0,
                                    Max = "*"
                                }
                            },
                            new ElementDefinition
                            {
                                ElementId = "DocumentReference.id",
                                Path = "DocumentReference.id",
                                Min = 0,
                                Max = "1",
                                Base = new ElementDefinition.BaseComponent
                                {
                                    Path = "Resource.id",
                                    Min = 0,
                                    Max = "1"
                                },
                                Type = new List<ElementDefinition.TypeRefComponent>()
                                {
                                    new ElementDefinition.TypeRefComponent
                                    {
                                        Code = "id"
                                    }
                                }
                            },
                            new ElementDefinition
                            {
                                ElementId = "DocumentReference.status",
                                Path = "DocumentReference.status",
                                Min = 1,
                                Max = "1",
                                Base = new ElementDefinition.BaseComponent
                                {
                                    Path = "DocumentReference.status",
                                    Min = 1,
                                    Max = "1"
                                },
                                Type = new List<ElementDefinition.TypeRefComponent>()
                                {
                                    new ElementDefinition.TypeRefComponent
                                    {
                                        Code = "code"
                                    }
                                }
                            },
                            new ElementDefinition
                            {
                                ElementId = "DocumentReference.type",
                                Path = "DocumentReference.type",
                                Min = 1,
                                Max = "1",
                                Base = new ElementDefinition.BaseComponent
                                {
                                    Path = "DocumentReference.type",
                                    Min = 1,
                                    Max = "1"
                                },
                                Type = new List<ElementDefinition.TypeRefComponent>()
                                {
                                    new ElementDefinition.TypeRefComponent
                                    {
                                        Code = "CodeableConcept"
                                    }
                                }
                            },
                            new ElementDefinition
                            {
                                ElementId = "DocumentReference.type.coding",
                                Path = "DocumentReference.type.coding",
                                Min = 1,
                                Max = "1",
                                Base = new ElementDefinition.BaseComponent
                                {
                                    Path = "CodeableConcept.coding",
                                    Min = 0,
                                    Max = "*"
                                },
                                Type = new List<ElementDefinition.TypeRefComponent>()
                                {
                                    new ElementDefinition.TypeRefComponent
                                    {
                                        Code = "Coding"
                                    }
                                }
                            },
                            new ElementDefinition
                            {
                                ElementId = "DocumentReference.type.coding.system",
                                Path = "DocumentReference.type.coding.system",
                                Min = 1,
                                Max = "1",
                                Base = new ElementDefinition.BaseComponent
                                {
                                    Path = "Coding.system",
                                    Min = 0,
                                    Max = "1"
                                },
                                Type = new List<ElementDefinition.TypeRefComponent>()
                                {
                                    new ElementDefinition.TypeRefComponent
                                    {
                                        Code = "uri"
                                    }
                                }
                            },
                            new ElementDefinition
                            {
                                ElementId = "DocumentReference.type.coding.code",
                                Path = "DocumentReference.type.coding.code",
                                Min = 1,
                                Max = "1",
                                Base = new ElementDefinition.BaseComponent
                                {
                                    Path = "Coding.code",
                                    Min = 0,
                                    Max = "1"
                                },
                                Type = new List<ElementDefinition.TypeRefComponent>()
                                {
                                    new ElementDefinition.TypeRefComponent
                                    {
                                        Code = "code"
                                    }
                                }
                            },
                            new ElementDefinition
                            {
                                ElementId = "DocumentReference.type.coding.display",
                                Path = "DocumentReference.type.coding.display",
                                Extension = new List<Extension>
                                {
                                    new Extension
                                    {
                                        Url = "http://hl7.org/fhir/StructureDefinition/elementdefinition-translatable",
                                        Value = new FhirBoolean
                                        {
                                            Value = true
                                        }
                                    }
                                },
                                Min = 1,
                                Max = "1",
                                Base = new ElementDefinition.BaseComponent
                                {
                                    Path = "Coding.display",
                                    Min = 0,
                                    Max = "1"
                                },
                                Type = new List<ElementDefinition.TypeRefComponent>()
                                {
                                    new ElementDefinition.TypeRefComponent
                                    {
                                        Code = "string"
                                    }
                                }
                            },
                            new ElementDefinition
                            {
                                ElementId = "DocumentReference.subject",
                                Path = "DocumentReference.subject",
                                Min = 1,
                                Max = "1",
                                Base = new ElementDefinition.BaseComponent
                                {
                                    Path = "DocumentReference.subject",
                                    Min = 0,
                                    Max = "1"
                                },
                                Type = new List<ElementDefinition.TypeRefComponent>()
                                {
                                    new ElementDefinition.TypeRefComponent
                                    {
                                        Code = "Reference",
                                        TargetProfile = "http://hl7.org/fhir/StructureDefinition/Patient"
                                    }
                                }
                            },
                            new ElementDefinition
                            {
                                ElementId = "DocumentReference.subject.reference",
                                Path = "DocumentReference.subject.reference",
                                Min = 1,
                                Max = "1",
                                Base = new ElementDefinition.BaseComponent
                                {
                                    Path = "Reference.reference",
                                    Min = 0,
                                    Max = "1"
                                },
                                Type = new List<ElementDefinition.TypeRefComponent>()
                                {
                                    new ElementDefinition.TypeRefComponent
                                    {
                                        Code = "string"
                                    }
                                }
                            },
                            new ElementDefinition
                            {
                                ElementId = "DocumentReference.created",
                                Path = "DocumentReference.created",
                                Min = 0,
                                Max = "1",
                                Base = new ElementDefinition.BaseComponent
                                {
                                    Path = "DocumentReference.created",
                                    Min = 0,
                                    Max = "1"
                                },
                                Type = new List<ElementDefinition.TypeRefComponent>()
                                {
                                    new ElementDefinition.TypeRefComponent
                                    {
                                        Code = "dateTime"
                                    }
                                }
                            },
                            new ElementDefinition
                            {
                                ElementId = "DocumentReference.indexed",
                                Path = "DocumentReference.indexed",
                                Min = 1,
                                Max = "1",
                                Base = new ElementDefinition.BaseComponent
                                {
                                    Path = "DocumentReference.indexed",
                                    Min = 1,
                                    Max = "1"
                                },
                                Type = new List<ElementDefinition.TypeRefComponent>()
                                {
                                    new ElementDefinition.TypeRefComponent
                                    {
                                        Code = "instant"
                                    }
                                }
                            },
                            new ElementDefinition
                            {
                                ElementId = "DocumentReference.author",
                                Path = "DocumentReference.author",
                                Min = 1,
                                Max = "1",
                                Base = new ElementDefinition.BaseComponent
                                {
                                    Path = "DocumentReference.author",
                                    Min = 0,
                                    Max = "*"
                                },
                                Type = new List<ElementDefinition.TypeRefComponent>()
                                {
                                    new ElementDefinition.TypeRefComponent
                                    {
                                        Code = "Reference",
                                        TargetProfile = "http://hl7.org/fhir/StructureDefinition/Organization"
                                    }
                                }
                            },
                            new ElementDefinition
                            {
                                ElementId = "DocumentReference.author.reference",
                                Path = "DocumentReference.author.reference",
                                Min = 1,
                                Max = "1",
                                Base = new ElementDefinition.BaseComponent
                                {
                                    Path = "Reference.reference",
                                    Min = 0,
                                    Max = "1"
                                },
                                Type = new List<ElementDefinition.TypeRefComponent>()
                                {
                                    new ElementDefinition.TypeRefComponent
                                    {
                                        Code = "string"
                                    }
                                }
                            },
                            new ElementDefinition
                            {
                                ElementId = "DocumentReference.custodian",
                                Path = "DocumentReference.custodian",
                                Min = 1,
                                Max = "1",
                                Base = new ElementDefinition.BaseComponent
                                {
                                    Path = "DocumentReference.custodian",
                                    Min = 0,
                                    Max = "1"
                                },
                                Type = new List<ElementDefinition.TypeRefComponent>()
                                {
                                    new ElementDefinition.TypeRefComponent
                                    {
                                        Code = "Reference",
                                        TargetProfile = "http://hl7.org/fhir/StructureDefinition/Organization"
                                    }
                                }
                            },
                            new ElementDefinition
                            {
                                ElementId = "DocumentReference.custodian.reference",
                                Path = "DocumentReference.custodian.reference",
                                Min = 1,
                                Max = "1",
                                Base = new ElementDefinition.BaseComponent
                                {
                                    Path = "Reference.reference",
                                    Min = 0,
                                    Max = "1"
                                },
                                Type = new List<ElementDefinition.TypeRefComponent>()
                                {
                                    new ElementDefinition.TypeRefComponent
                                    {
                                        Code = "string"
                                    }
                                }
                            },
                            new ElementDefinition
                            {
                                ElementId = "DocumentReference.content",
                                Path = "DocumentReference.content",
                                Min = 1,
                                Max = "*",
                                Base = new ElementDefinition.BaseComponent
                                {
                                    Path = "DocumentReference.content",
                                    Min = 1,
                                    Max = "*"
                                },
                                Type = new List<ElementDefinition.TypeRefComponent>()
                                {
                                    new ElementDefinition.TypeRefComponent
                                    {
                                        Code = "BackboneElement"
                                    }
                                }
                            },
                            new ElementDefinition
                            {
                                ElementId = "DocumentReference.content.attachment",
                                Path = "DocumentReference.content.attachment",
                                Min = 1,
                                Max = "1",
                                Base = new ElementDefinition.BaseComponent
                                {
                                    Path = "DocumentReference.content.attachment",
                                    Min = 1,
                                    Max = "1"
                                },
                                Type = new List<ElementDefinition.TypeRefComponent>()
                                {
                                    new ElementDefinition.TypeRefComponent
                                    {
                                        Code = "Attachment"
                                    }
                                }
                            },
                            new ElementDefinition
                            {
                                ElementId = "DocumentReference.content.attachment.contentType",
                                Path = "DocumentReference.content.attachment.contentType",
                                Min = 1,
                                Max = "1",
                                Base = new ElementDefinition.BaseComponent
                                {
                                    Path = "Attachment.contentType",
                                    Min = 0,
                                    Max = "1"
                                },
                                Type = new List<ElementDefinition.TypeRefComponent>()
                                {
                                    new ElementDefinition.TypeRefComponent
                                    {
                                        Code = "code"
                                    }
                                }
                            },
                            new ElementDefinition
                            {
                                ElementId = "DocumentReference.content.attachment.url",
                                Path = "DocumentReference.content.attachment.url",
                                Min = 1,
                                Max = "1",
                                Base = new ElementDefinition.BaseComponent
                                {
                                    Path = "Attachment.url",
                                    Min = 0,
                                    Max = "1"
                                },
                                Type = new List<ElementDefinition.TypeRefComponent>()
                                {
                                    new ElementDefinition.TypeRefComponent
                                    {
                                        Code = "uri"
                                    }
                                }
                            },
                            new ElementDefinition
                            {
                                ElementId = "DocumentReference.content.attachment.creation",
                                Path = "DocumentReference.content.attachment.creation",
                                Min = 0,
                                Max = "1",
                                Base = new ElementDefinition.BaseComponent
                                {
                                    Path = "Attachment.creation",
                                    Min = 0,
                                    Max = "1"
                                },
                                Type = new List<ElementDefinition.TypeRefComponent>()
                                {
                                    new ElementDefinition.TypeRefComponent
                                    {
                                        Code = "dateTime"
                                    }
                                }
                            }
                        }
                    }

                };

                return content;
            }
        }
    }
}
