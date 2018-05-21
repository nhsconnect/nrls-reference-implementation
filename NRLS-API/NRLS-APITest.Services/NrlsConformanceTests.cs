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
                Url = "https://fhir.nhs.uk/STU3/CapabilityStatement/NRLS-CapabilityStatement-1",
                Version = ModelInfo.Version,
                Name = "NRLS-CapabilityStatement-1",
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
                Description = new Markdown("This profile defines the expected capabilities of the NRLS STU3 FHIR server when conforming to the NRLS API IG. The Capability Statement resource includes a complete list of actual profiles, RESTful operations, and search parameters that are expected to be supported by NRLS STU3 FHIR Server."),
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
                                                Value = new FhirString("patient")
                                            },
                                            new Extension
                                            {
                                                Url = "optional",
                                                Value = new FhirString("custodian")
                                            }
                                        }
                                    }
                                },
                                Type = ResourceType.DocumentReference,
                                Profile = new ResourceReference
                                {
                                    Reference = "https://fhir.hl7.org.uk/StructureDefinition/NRLS-DocumentReference-1"
                                },
                                Interaction = new List<CapabilityStatement.ResourceInteractionComponent>
                                {
                                    new CapabilityStatement.ResourceInteractionComponent
                                    {
                                        Code = CapabilityStatement.TypeRestfulInteraction.SearchType,
                                        Documentation = "Allows discovery of existing NRLS document reference resources using different search criteria"
                                    },
                                    new CapabilityStatement.ResourceInteractionComponent
                                    {
                                        Code = CapabilityStatement.TypeRestfulInteraction.Read,
                                        Documentation = "Allows retrieval of specific NRLS document references by id"
                                    },
                                    //new CapabilityStatement.ResourceInteractionComponent
                                    //{
                                    //    Code = CapabilityStatement.TypeRestfulInteraction.Update,
                                    //    Documentation = "Allows update of specific NRLS document references by id"
                                    //},
                                    new CapabilityStatement.ResourceInteractionComponent
                                    {
                                        Code = CapabilityStatement.TypeRestfulInteraction.Delete,
                                        Documentation = "Allows deletion of specific NRLS document references by id"
                                    },
                                    new CapabilityStatement.ResourceInteractionComponent
                                    {
                                        Code = CapabilityStatement.TypeRestfulInteraction.Create,
                                        Documentation = "Allows creation of NRLS document references"
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
                                                Value = new FhirString("MUST")
                                            }
                                        },
                                        Name = "patient",
                                        Definition = "http://hl7.org/fhir/SearchParameter/DocumentReference.subject",
                                        Type = SearchParamType.Reference
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
                                        Name = "custodian",
                                        Definition = "http://hl7.org/fhir/SearchParameter/DocumentReference.custodian",
                                        Type = SearchParamType.Reference
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
                                        Name = "_count",
                                        Definition = "http://hl7.org/fhir/search",
                                        Type = SearchParamType.Number,
                                        Documentation = "Number of records to return"
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
