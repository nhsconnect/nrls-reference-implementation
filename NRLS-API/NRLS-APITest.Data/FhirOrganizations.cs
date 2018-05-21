using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRLS_APITest.Data
{
    public class FhirOrganizations
    {
        public static Organization Valid_Organization
        {
            get
            {
                return new Organization
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
                };
            }
        }
    }
}
