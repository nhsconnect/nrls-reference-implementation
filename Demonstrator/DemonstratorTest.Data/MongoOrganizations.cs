using Hl7.Fhir.Model;
using System.Collections.Generic;

namespace DemonstratorTest.Data.Helpers
{
    public static class MongoOrganizations
    {
        public static IList<Organization> Organizations
        {
            get
            {
                return new List<Organization>
                {
                    new Organization
                    {
                        Id = "5ab13695957d0ad5d93a1330",
                        Identifier = new List<Identifier> {
                            new Identifier {
                                System = "https://fhir.nhs.uk/Id/ods-organization-code",
                                Value = "1XR"
                            }
                        },
                        Name = "Bradford North NHS Foundation Trust",
                        Telecom = new List<ContactPoint> {
                            new ContactPoint {
                                System = ContactPoint.ContactPointSystem.Phone,
                                Value = "+44 1274 000 001"
                            }
                        },
                        Address = new List<Address> {
                            new Address{
                                Line = new List<string> {
                                    "3001 Bradford Street"
                                },
                                City = "Bradford",
                                State = "West Yorkshire",
                                PostalCode = "BD99 1XR",
                                Country = "United Kingdom"
                            }
                        }
                    },
                    new Organization
                    {
                        Id = "5ab13ab4957d0ad5d93a1331",
                        Identifier = new List<Identifier> {
                            new Identifier {
                                System = "https://fhir.nhs.uk/Id/ods-organization-code",
                                Value = "2XR"
                            }
                        },
                        Name = "Nottingham West NHS Foundation Trust",
                        Telecom = new List<ContactPoint> {
                            new ContactPoint {
                                System = ContactPoint.ContactPointSystem.Phone,
                                Value = "+44 115 000 0002"
                            }
                        },
                        Address = new List<Address> {
                            new Address{
                                Line = new List<string> {
                                    "3002 Nottingham Street"
                                },
                                City = "Nottingham",
                                State = "Nottinghamshire",
                                PostalCode = "NG99 2XR",
                                Country = "United Kingdom"
                            }
                        }
                    },
                    new Organization
                    {
                        Id = "5ab13b6e957d0ad5d93a1332",
                        Identifier = new List<Identifier> {
                            new Identifier {
                                System = "https://fhir.nhs.uk/Id/ods-organization-code",
                                Value = "00003X"
                            }
                        },
                        Name = "Frizinghall Medical Centre",
                        Telecom = new List<ContactPoint> {
                            new ContactPoint {
                                System = ContactPoint.ContactPointSystem.Phone,
                                Value = "+44 1274 000 003"
                            }
                        },
                        Address = new List<Address> {
                            new Address{
                                Line = new List<string> {
                                    "3003 Frizinghall Street"
                                },
                                City = "Bradford",
                                State = "West Yorkshire",
                                PostalCode = "BD99 03X",
                                Country = "United Kingdom"
                            }
                        }
                    },
                    new Organization
                    {
                        Id = "5ab13c13957d0ad5d93a1333",
                        Identifier = new List<Identifier> {
                            new Identifier {
                                System = "https://fhir.nhs.uk/Id/ods-organization-code",
                                Value = "00004X"
                            }
                        },
                        Name = "Sandiacre Medical Centre",
                        Telecom = new List<ContactPoint> {
                            new ContactPoint {
                                System = ContactPoint.ContactPointSystem.Phone,
                                Value = "+44 115 000 0004"
                            }
                        },
                        Address = new List<Address> {
                            new Address{
                                Line = new List<string> {
                                    "3003 Sandiacre Street"
                                },
                                City = "Nottingham",
                                State = "Nottinghamshire",
                                PostalCode = "NG99 04X",
                                Country = "United Kingdom"
                            }
                        }
                    },
                    new Organization
                    {
                        Id = "5ac5467ea35c66326e022230",
                        Identifier = new List<Identifier> {
                            new Identifier {
                                System = "https://fhir.nhs.uk/Id/ods-organization-code",
                                Value = "MHT01"
                            }
                        },
                        Name = "Northcliffe Mental Health Trust",
                        Telecom = new List<ContactPoint> {
                            new ContactPoint {
                                System = ContactPoint.ContactPointSystem.Phone,
                                Value = "+44 1274 000 0005"
                            }
                        },
                        Address = new List<Address> {
                            new Address{
                                Line = new List<string> {
                                    "123 Green View"
                                },
                                City = "Bradford",
                                State = "West Yorkshire",
                                PostalCode = "BD17 1HT",
                                Country = "United Kingdom"
                            }
                        }
                    },
                    new Organization
                    {
                        Id = "5ad5ffac0ed89913c3d8c81c",
                        Identifier = new List<Identifier> {
                            new Identifier {
                                System = "https://fhir.nhs.uk/Id/ods-organization-code",
                                Value = "AMS01"
                            }
                        },
                        Name = "Otley Ambulance Service",
                        Telecom = new List<ContactPoint> {
                            new ContactPoint {
                                System = ContactPoint.ContactPointSystem.Phone,
                                Value = "+44 1274 000 0006"
                            }
                        },
                        Address = new List<Address> {
                            new Address{
                                Line = new List<string> {
                                    "456 Hill Lane"
                                },
                                City = "Leeds",
                                State = "West Yorkshire",
                                PostalCode = "LS25 2MS",
                                Country = "United Kingdom"
                            }
                        }
                    }
                };
            }
        }

    }
}
