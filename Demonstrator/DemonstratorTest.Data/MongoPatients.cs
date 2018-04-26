using Hl7.Fhir.Model;
using System.Collections.Generic;

namespace DemonstratorTest.Data.Helpers
{
    public static class MongoPatients
    {
        public static IList<Patient> Patients
        {
            get
            {
                return new List<Patient>
                {
                    new Patient
                    {
                        Id = "5ab13695957d0ad5d93a1330",
                        Identifier = new List<Identifier> {
                            new Identifier {
                                System = "https://fhir.nhs.uk/Id/nhs-number",
                                Value = "2686033207"
                            }
                        },
                        Active = true,
                        Name = new List<HumanName> {
                            new HumanName
                            {
                                Use = HumanName.NameUse.Official,
                                Family = "Chalmers",
                                Given = new List<string> {
                                    "Peter",
                                    "James"
                                }
                            }
                        },
                        Telecom = new List<ContactPoint> {
                            new ContactPoint {
                                System = ContactPoint.ContactPointSystem.Phone,
                                Value = "01274100000",
                                Use = ContactPoint.ContactPointUse.Home,
                                Rank = 1
                            }
                        },
                        Gender = AdministrativeGender.Male,
                        BirthDate = "1974-12-25",
                        Deceased = new FhirBoolean(false),
                        Address = new List<Address> {
                            new Address {
                                Use = Address.AddressUse.Home,
                                Type = Address.AddressType.Both,
                                Line = new List<string> {
                                    "534 Erewhon Street"
                                },
                                City = "Bradford",
                                State = "West Yorkshire",
                                PostalCode = "AB1 2CD",
                                Period = new Period
                                {
                                    Start = "1974-12-25"
                                }
                            }
                        },
                        ManagingOrganization = new ResourceReference
                        {
                            Reference = "Organization/00003X"
                        }
                    },
                    new Patient
                    {
                        Id = "5ab13c9f957d0ad5d93a1334",
                        Identifier = new List<Identifier> {
                            new Identifier {
                                System = "https://fhir.nhs.uk/Id/nhs-number",
                                Value = "1020103620"
                            }
                        },
                        Active = true,
                        Name = new List<HumanName> {
                            new HumanName
                            {
                                Use = HumanName.NameUse.Official,
                                Family = "Smith",
                                Given = new List<string> {
                                    "David"
                                }
                            }
                        },
                        Telecom = new List<ContactPoint> {
                            new ContactPoint {
                                System = ContactPoint.ContactPointSystem.Phone,
                                Value = "01274200000",
                                Use = ContactPoint.ContactPointUse.Home,
                                Rank = 1
                            }
                        },
                        Gender = AdministrativeGender.Male,
                        BirthDate = "1953-09-14",
                        Deceased = new FhirBoolean(false),
                        Address = new List<Address> {
                            new Address {
                                Use = Address.AddressUse.Home,
                                Type = Address.AddressType.Both,
                                Line = new List<string> {
                                    "57 North Street"
                                },
                                City = "Bradford",
                                State = "West Yorkshire",
                                PostalCode = "AB2 3CD",
                                Period = new Period
                                {
                                    Start = "1993-04-25"
                                }
                            }
                        },
                        ManagingOrganization = new ResourceReference
                        {
                            Reference = "Organization/00003X"
                        }
                    },
                    new Patient
                    {
                        Id = "5ab13d20957d0ad5d93a1336",
                        Identifier = new List<Identifier> {
                            new Identifier {
                                System = "https://fhir.nhs.uk/Id/nhs-number",
                                Value = "3656987882"
                            }
                        },
                        Active = true,
                        Name = new List<HumanName> {
                            new HumanName
                            {
                                Use = HumanName.NameUse.Official,
                                Family = "Jones",
                                Given = new List<string> {
                                    "Claire"
                                }
                            }
                        },
                        Telecom = new List<ContactPoint> {
                            new ContactPoint {
                                System = ContactPoint.ContactPointSystem.Phone,
                                Value = "01274300000",
                                Use = ContactPoint.ContactPointUse.Home,
                                Rank = 1
                            }
                        },
                        Gender = AdministrativeGender.Female,
                        BirthDate = "1982-06-24",
                        Deceased = new FhirBoolean(false),
                        Address = new List<Address> {
                            new Address {
                                Use = Address.AddressUse.Home,
                                Type = Address.AddressType.Both,
                                Line = new List<string> {
                                    "19 River Street"
                                },
                                City = "Bradford",
                                State = "West Yorkshire",
                                PostalCode = "AB3 4CD",
                                Period = new Period
                                {
                                    Start = "1982-06-24"
                                }
                            }
                        },
                        ManagingOrganization = new ResourceReference
                        {
                            Reference = "Organization/00003X"
                        }
                    },
                    new Patient
                    {
                        Id = "5ab13da4957d0ad5d93a1337",
                        Identifier = new List<Identifier> {
                            new Identifier {
                                System = "https://fhir.nhs.uk/Id/nhs-number",
                                Value = "0766140741"
                            }
                        },
                        Active = true,
                        Name = new List<HumanName> {
                            new HumanName
                            {
                                Use = HumanName.NameUse.Official,
                                Family = "Arthur",
                                Given = new List<string> {
                                    "Jeff"
                                }
                            }
                        },
                        Telecom = new List<ContactPoint> {
                            new ContactPoint {
                                System = ContactPoint.ContactPointSystem.Phone,
                                Value = "01154000000",
                                Use = ContactPoint.ContactPointUse.Home,
                                Rank = 1
                            }
                        },
                        Gender = AdministrativeGender.Male,
                        BirthDate = "1979-09-03",
                        Deceased = new FhirBoolean(false),
                        Address = new List<Address> {
                            new Address {
                                Use = Address.AddressUse.Home,
                                Type = Address.AddressType.Both,
                                Line = new List<string> {
                                    "19 Tower Street"
                                },
                                City = "Nottingham",
                                State = "Nottinghamshire",
                                PostalCode = "AB4 5CD",
                                Period = new Period
                                {
                                    Start = "2003-11-14"
                                }
                            }
                        },
                        ManagingOrganization = new ResourceReference
                        {
                            Reference = "Organization/00004X"
                        }
                    },
                    new Patient
                    {
                        Id = "5ab13e1e957d0ad5d93a1338",
                        Identifier = new List<Identifier> {
                            new Identifier {
                                System = "https://fhir.nhs.uk/Id/nhs-number",
                                Value = "5456049767"
                            }
                        },
                        Active = true,
                        Name = new List<HumanName> {
                            new HumanName
                            {
                                Use = HumanName.NameUse.Official,
                                Family = "Deacon",
                                Given = new List<string> {
                                    "Jennifer"
                                }
                            }
                        },
                        Telecom = new List<ContactPoint> {
                            new ContactPoint {
                                System = ContactPoint.ContactPointSystem.Phone,
                                Value = "01155000000",
                                Use = ContactPoint.ContactPointUse.Home,
                                Rank = 1
                            }
                        },
                        Gender = AdministrativeGender.Female,
                        BirthDate = "1987-03-26",
                        Deceased = new FhirBoolean(false),
                        Address = new List<Address> {
                            new Address {
                                Use = Address.AddressUse.Home,
                                Type = Address.AddressType.Both,
                                Line = new List<string> {
                                    "19 Cross Street"
                                },
                                City = "Nottingham",
                                State = "Nottinghamshire",
                                PostalCode = "AB5 6CD",
                                Period = new Period
                                {
                                    Start = "1987-03-26"
                                }
                            }
                        },
                        ManagingOrganization = new ResourceReference
                        {
                            Reference = "Organization/00004X"
                        }
                    }
                };
            }
        }

    }
}
