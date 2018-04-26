using Hl7.Fhir.Model;
using System.Collections.Generic;

namespace DemonstratorTest.Data.Helpers
{
    public static class MongoPractitioner
    {
        public static IList<Practitioner> Practitioners
        {
            get
            {
                return new List<Practitioner>
                {
                    new Practitioner
                    {
                        Id = "5ab13695957d0ad5d93a1330",
                        Active = true,
                        Name = new List<HumanName> {
                            new HumanName
                            {
                                Use = HumanName.NameUse.Official,
                                Family = "Peters",
                                Given = new List<string> {
                                    "James"
                                }
                            }
                        },
                        Telecom = new List<ContactPoint> {
                            new ContactPoint {
                                System = ContactPoint.ContactPointSystem.Phone,
                                Value = "01274100555",
                                Use = ContactPoint.ContactPointUse.Work,
                                Rank = 1
                            }
                        },
                        Gender = AdministrativeGender.Male,
                        BirthDate = "1974-12-25",
                        Address = new List<Address> {
                            new Address {
                                Use = Address.AddressUse.Work,
                                Type = Address.AddressType.Both,
                                Line = new List<string> {
                                    "534 Erewhon Street"
                                },
                                City = "Bradford",
                                State = "West Yorkshire",
                                PostalCode = "AB1 2CD"
                            }
                        }
                    }
                };
            }
        }

    }
}
