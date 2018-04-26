using Demonstrator.Models.ViewModels.Fhir;
using Demonstrator.Models.ViewModels.Nrls;
using System;
using System.Collections.Generic;

namespace DemonstratorTest.Data.Helpers
{
    public static class MongoPointerViewModels
    {
        public static List<PointerViewModel> PointerViewModels_3656987882
        {
            get
            {
                return new List<PointerViewModel>
                {
                    new PointerViewModel
                    {
                        ResourceType = null,
                        Id = "5aba0f464f02ced4c7eb16c4",
                        Status = "Current",
                        Identifier = new List<IdentifierViewModel>(),
                        Type = new CodeableConceptViewModel
                        {
                            Coding = new List<CodingViewModel>
                            {
                                new CodingViewModel
                                {
                                    System = "http://snomed.info/sct",
                                    Code = "718347002",
                                    Display = "Mental health care plan"
                                }
                            }
                        },
                        Subject = new ReferenceViewModel
                        {
                            Id = "3656987882",
                            Reference = "https://demographics.spineservices.nhs.uk/STU3/Patient/3656987882"
                        },
                        Indexed = new DateTime(2011, 06, 21, 11, 45, 16, DateTimeKind.Local),
                        Author = new ReferenceViewModel
                        {
                            Id = "00003X",
                            Reference = "https://directory.spineservices.nhs.uk/STU3/Organization/00003X"
                        },
                        Custodian = new ReferenceViewModel
                        {
                            Id = "00003X",
                            Reference = "https://directory.spineservices.nhs.uk/STU3/Organization/00003X"
                        },
                        Content = new List<ContentViewModel>
                        {
                            new ContentViewModel
                            {
                                Attachment = new AttachmentViewModel
                                {
                                    ContentType = "application/pdf",
                                    Url = "http://example.org/xds/mhd/Binary/07a6483f-732b-461e-86b6-edb665c45510.pdf",
                                    Title = "Mental health Care Plan Report",
                                    Creation = new DateTime(2011, 06, 21, 11, 45, 16, DateTimeKind.Utc)
                                }
                            }
                        }
                    }
                };
                
            }
        }

    }
}
