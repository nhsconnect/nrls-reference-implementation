using Demonstrator.Models.ViewModels.Fhir;
using System.Collections.Generic;

namespace DemonstratorTest.Data.Helpers
{
    public static class MongoOrganizationViewModels
    {
        public static OrganizationViewModel OrganizationViewModel_00003X
        {
            get
            {
                return new OrganizationViewModel
                {
                    ResourceType = null,
                    Id = "5ab13b6e957d0ad5d93a1332",
                    OrgCode = "00003X",
                    Meta = null,
                    Identifier = new List<IdentifierViewModel> {
                        new IdentifierViewModel {
                            System = "https://fhir.nhs.uk/Id/ods-organization-code",
                            Value = "00003X"
                        }
                    },
                    Name = "Frizinghall Medical Centre",
                    Telecom = new List<ContactPointViewModel> {
                        new ContactPointViewModel {
                            System = "Phone",
                            Value = "+44 1274 000 003",
                            Use = null
                        }
                    },
                    Address = new List<AddressViewModel> {
                        new AddressViewModel {
                            Use = null,
                            Type = null,
                            Line = new List<string> {
                                "3003 Frizinghall Street"
                            },
                            City = "Bradford",
                            District = null,
                            PostalCode = "BD99 03X",
                            Period = null
                        }
                    }
                };
            }
        }

    }
}
