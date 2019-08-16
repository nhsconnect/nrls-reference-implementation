using Hl7.Fhir.Model;
using NRLS_API.Services;
using NRLS_APITest.Comparer;
using System;
using System.Collections.Generic;
using Xunit;

namespace NRLS_APITest.Services
{
    public class NrlsConformanceTests
    {
        [Fact]
        public void NrlsConformance_Valid()
        {
            var expected = new CapabilityStatement
            {
                Url = "https://fhir.nhs.uk/STU3/CapabilityStatement/NRL-CapabilityStatement-1",
                Version = ModelInfo.Version,
                Name = "NRL-CapabilityStatement-1",
                Status = PublicationStatus.Draft,
                Date = "2017-10-11T10:20:38+00:00",
                Publisher = "NHS Digital",
                Contact = new List<ContactDetail>
                {
                    new ContactDetail
                    {
                        Name = "Interoperability Team",
                        Telecom = new List<ContactPoint>
                        {
                            new ContactPoint
                            {
                                System = ContactPoint.ContactPointSystem.Email,
                                Use = ContactPoint.ContactPointUse.Work,
                                Value = "interoperabilityteam@nhs.net"
                            }
                        }
                    }
                },
                Description = new Markdown("This profile defines the expected capabilities of the NRL STU3 FHIR server when conforming to the NRL API IG. The Capability Statement resource includes a complete list of actual profiles, RESTful operations, and search parameters that are expected to be supported by NRL STU3 FHIR Server."),
                Copyright = new Markdown("Copyright © 2017 NHS Digital"),
                Kind = CapabilityStatement.CapabilityStatementKind.Requirements,
                FhirVersion = "3.0.1",
                AcceptUnknown = CapabilityStatement.UnknownContentCode.No,
                Format = new List<string>
                {
                    "application/fhir+xml",
                    "application/fhir+json"
                },
                Rest = new List<CapabilityStatement.RestComponent>
                {
                    new CapabilityStatement.RestComponent
                    {
                        Mode = CapabilityStatement.RestfulCapabilityMode.Server,
                        Security = new CapabilityStatement.SecurityComponent
                        {
                            Cors = true
                        },
                        Resource = new List<CapabilityStatement.ResourceComponent>
                        {
                            new CapabilityStatement.ResourceComponent
                            {
                                Extension = new List<Extension>
                                {
                                    new Extension
                                    {
                                        Url = "http://hl7.org/fhir/StructureDefinition/conformance-search-parameter-combination",
                                        Extension = new List<Extension>
                                        {
                                            new Extension
                                            {
                                                Url = "required",
                                                Value = new FhirString("_id")
                                            }
                                        }
                                    },
                                    new Extension
                                    {
                                        Url = "http://hl7.org/fhir/StructureDefinition/conformance-search-parameter-combination",
                                        Extension = new List<Extension>
                                        {
                                            new Extension
                                            {
                                                Url = "required",
                                                Value = new FhirString("subject")
                                            },
                                            new Extension
                                            {
                                                Url = "optional",
                                                Value = new FhirString("custodian")
                                            },
                                            new Extension
                                            {
                                                Url = "optional",
                                                Value = new FhirString("type")
                                            }
                                        }
                                    },
                                    new Extension
                                    {
                                        Url = "http://hl7.org/fhir/StructureDefinition/conformance-search-parameter-combination",
                                        Extension = new List<Extension>
                                        {
                                            new Extension
                                            {
                                                Url = "required",
                                                Value = new FhirString("subject")
                                            },
                                            new Extension
                                            {
                                                Url = "required",
                                                Value = new FhirString("_summary")
                                            }
                                        }
                                    },
                                   new Extension
                                    {
                                        Url = "http://hl7.org/fhir/StructureDefinition/conformance-search-parameter-combination",
                                        Extension = new List<Extension>
                                        {
                                            new Extension
                                            {
                                                Url = "required",
                                                Value = new FhirString("subject")
                                            },
                                            new Extension
                                            {
                                                Url = "required",
                                                Value = new FhirString("identifier")
                                            }
                                        },
                                        FhirComments = new List<string>
                                        {
                                            "This combination can only be used for Supersede, Patch, and Delete interactions."
                                        }
                                    }
                                },
                                Type = ResourceType.DocumentReference,
                                Profile = new ResourceReference
                                {
                                    Reference = "https://fhir.hl7.org.uk/StructureDefinition/NRL-DocumentReference-1"
                                },
                                Interaction = new List<CapabilityStatement.ResourceInteractionComponent>
                                {
                                    new CapabilityStatement.ResourceInteractionComponent
                                    {
                                        Code = CapabilityStatement.TypeRestfulInteraction.SearchType,
                                        Documentation = "Allows discovery of existing NRL document reference resources using different search criteria"
                                    },
                                    new CapabilityStatement.ResourceInteractionComponent
                                    {
                                        Code = CapabilityStatement.TypeRestfulInteraction.Read,
                                        Documentation = "Allows retrieval of specific NRL document references by id"
                                    },
                                    new CapabilityStatement.ResourceInteractionComponent
                                    {
                                        Code = CapabilityStatement.TypeRestfulInteraction.Patch,
                                        Documentation = "Allows update of specific NRL document references by id"
                                    },
                                    new CapabilityStatement.ResourceInteractionComponent
                                    {
                                        Code = CapabilityStatement.TypeRestfulInteraction.Delete,
                                        Documentation = "Allows deletion of specific NRL document references by id"
                                    },
                                    new CapabilityStatement.ResourceInteractionComponent
                                    {
                                        Code = CapabilityStatement.TypeRestfulInteraction.Create,
                                        Documentation = "Allows creation of NRL document references"
                                    }
                                },
                                SearchParam = new List<CapabilityStatement.SearchParamComponent>
                                {
                                    new CapabilityStatement.SearchParamComponent
                                    {
                                        Extension = new List<Extension>
                                        {
                                            new Extension
                                            {
                                                Url = "https://fhir.nhs.uk/STU3/StructureDefinition/Extension-CapabilityStatementExpectation-1",
                                                Value = new FhirString("SHOULD")
                                            }
                                        },
                                        Name = "subject",
                                        Definition = "http://hl7.org/fhir/SearchParameter/DocumentReference.subject",
                                        Type = SearchParamType.Reference,
                                        Documentation = "The Patient the DocumentReference relates to. This is MUST when searching by patient."
                                    },
                                    new CapabilityStatement.SearchParamComponent
                                    {
                                        Extension = new List<Extension>
                                        {
                                            new Extension
                                            {
                                                Url = "https://fhir.nhs.uk/STU3/StructureDefinition/Extension-CapabilityStatementExpectation-1",
                                                Value = new FhirString("MAY")
                                            }
                                        },
                                        Name = "custodian",
                                        Definition = "http://hl7.org/fhir/SearchParameter/DocumentReference.custodian",
                                        Type = SearchParamType.Reference,
                                        Documentation = "The owner of the DocumentReference"
                                    },
                                    new CapabilityStatement.SearchParamComponent
                                    {
                                        Extension = new List<Extension>
                                        {
                                            new Extension
                                            {
                                                Url = "https://fhir.nhs.uk/STU3/StructureDefinition/Extension-CapabilityStatementExpectation-1",
                                                Value = new FhirString("SHOULD")
                                            }
                                        },
                                        Name = "_id",
                                        Definition = "http://hl7.org/fhir/search",
                                        Type = SearchParamType.String,
                                        Documentation = "Logical id of the DocumentReference. This is MUST when searching by _id"
                                    },
                                    new CapabilityStatement.SearchParamComponent
                                    {
                                        Extension = new List<Extension>
                                        {
                                            new Extension
                                            {
                                                Url = "https://fhir.nhs.uk/STU3/StructureDefinition/Extension-CapabilityStatementExpectation-1",
                                                Value = new FhirString("MAY")
                                            }
                                        },
                                        Name = "type",
                                        Definition = "http://hl7.org/fhir/search",
                                        Type = SearchParamType.Token,
                                        Documentation = "Clinical type the DocumentReference refers too"
                                    },
                                    new CapabilityStatement.SearchParamComponent
                                    {
                                        Extension = new List<Extension>
                                        {
                                            new Extension
                                            {
                                                Url = "https://fhir.nhs.uk/STU3/StructureDefinition/Extension-CapabilityStatementExpectation-1",
                                                Value = new FhirString("MAY")
                                            }
                                        },
                                        Name = "_summary",
                                        Definition = "http://hl7.org/fhir/search",
                                        Type = SearchParamType.String,
                                        Documentation = "Type of summary to return"
                                    },
                                    new CapabilityStatement.SearchParamComponent
                                    {
                                        Extension = new List<Extension>
                                        {
                                            new Extension
                                            {
                                                Url = "https://fhir.nhs.uk/STU3/StructureDefinition/Extension-CapabilityStatementExpectation-1",
                                                Value = new FhirString("MAY")
                                            }
                                        },
                                        Name = "identifier",
                                        Definition = "http://hl7.org/fhir/search",
                                        Type = SearchParamType.Token,
                                        Documentation = "The MasterIdentifer associated to the DocumentReference. Used in Supersede, Patch, and Delete interactions."
                                    },
                                    new CapabilityStatement.SearchParamComponent
                                    {
                                        Extension = new List<Extension>
                                        {
                                            new Extension
                                            {
                                                Url = "https://fhir.nhs.uk/STU3/StructureDefinition/Extension-CapabilityStatementExpectation-1",
                                                Value = new FhirString("MAY")
                                            }
                                        },
                                        Name = "_format",
                                        Definition = "http://hl7.org/fhir/search",
                                        Type = SearchParamType.String,
                                        Documentation = "Content Type of returned result"
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var actual = new NrlsConformance().GetConformance();

            Assert.Equal(expected, actual, Comparers.ModelComparer<CapabilityStatement>());
        }
    }
}
