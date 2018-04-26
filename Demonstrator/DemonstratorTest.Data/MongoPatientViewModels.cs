using Demonstrator.Models.ViewModels.Fhir;
using System;
using System.Collections.Generic;

namespace DemonstratorTest.Data.Helpers
{
    public static class MongoPatientViewModels
    {
        public static PatientViewModel PatientViewModel_3656987882
        {
            get
            {
                return new PatientViewModel
                {
                    ResourceType = null,
                    Id = "5ab13d20957d0ad5d93a1336",
                    Identifier = new List<IdentifierViewModel> {
                        new IdentifierViewModel {
                            System = "https://fhir.nhs.uk/Id/nhs-number",
                            Value = "3656987882"
                        }
                    },
                    Active = true,
                    Name = new List<NameViewModel> {
                        new NameViewModel
                        {
                            Use = "Official",
                            Family = "Jones",
                            Given = new List<string> {
                                "Claire"
                            },
                            GivenString = "Claire",
                            Period = null
                        }
                    },
                    CurrentName = new NameViewModel
                    {
                        Use = "Official",
                        Family = "Jones",
                        Given = new List<string> {
                            "Claire"
                        },
                        GivenString = "Claire",
                        Period = null
                    },
                    Telecom = new ContactPointViewModel {
                        System = "Phone",
                        Value = "01274300000",
                        Use = "Home"
                    },
                    Gender = "Female",
                    BirthDate = new DateTime(1982, 6, 24, 0, 0, 0, DateTimeKind.Unspecified),
                    DeceasedBoolean = null,
                    Address = new List<AddressViewModel> {
                        new AddressViewModel {
                            Use = "Home",
                            Type = "Both",
                            Line = new List<string> {
                                "19 River Street"
                            },
                            City = "Bradford",
                            District = null,
                            PostalCode = "AB3 4CD",
                            Period = new PeriodViewModel
                            {
                                Start = new DateTime(1982, 6, 24, 0, 0, 0, DateTimeKind.Unspecified),
                                End = null,
                                IsActive = true
                            }
                        }
                    },
                    CurrentAddress = new AddressViewModel
                    {
                        Use = "Home",
                        Type = "Both",
                        Line = new List<string> {
                                "19 River Street"
                            },
                        City = "Bradford",
                        District = null,
                        PostalCode = "AB3 4CD",
                        Period = new PeriodViewModel
                        {
                            Start = new DateTime(1982, 6, 24, 0, 0, 0, DateTimeKind.Unspecified),
                            End = null,
                            IsActive = true
                        }
                    },
                    ManagingOrganization = new ReferenceViewModel
                    {
                        Id = "00003X",
                        Reference = "Organization/00003X"
                    },
                    GpPractice = null,
                    NhsNumber = null
                };
            }
        }

    }
}
