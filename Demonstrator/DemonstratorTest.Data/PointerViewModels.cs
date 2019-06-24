using Demonstrator.Models.ViewModels.Fhir;
using Demonstrator.Models.ViewModels.Nrls;
using System;
using System.Collections.Generic;

namespace DemonstratorTest.Data
{
    public class PointerViewModels
    {
        public static PointerViewModel ValidFor0000000000
        {
            get
            {
                return new PointerViewModel
                {
                    Id = "5ab13f4a957d0ad5d93a133a",
                    Status = "Current",
                    Type = new CodeableConceptViewModel
                    {
                        Coding = new List<CodingViewModel>
                            {
                                new CodingViewModel
                                {
                                    System = "http://snomed.info/sct",
                                    Code = "718347002",
                                    Display = "Cancer care plan"
                                }
                            }
                    },
                    Subject = new ReferenceViewModel
                    {
                        Reference = "https://demographics.spineservices.nhs.uk/STU3/Patient/2686033207"
                    },
                    Indexed = new DateTime(2011, 07, 19, 11, 27, 16),
                    Author = new ReferenceViewModel
                    {
                            Reference = "https://directory.spineservices.nhs.uk/STU3/Organization/1XR"                         
                    },
                    Custodian = new ReferenceViewModel
                    {
                        Reference = "https://directory.spineservices.nhs.uk/STU3/Organization/1XR"
                    },
                    Content = new List<ContentViewModel>
                        {
                            new ContentViewModel
                            {
                                Attachment = new AttachmentViewModel
                                {
                                    ContentType = "application/pdf",
                                    Url = "http://example.org/xds/mhd/Binary/07a6483f-732b-461e-86b6-edb665c45510.pdf",
                                    Title = "Cancer Care Plan Report",
                                    Creation = new DateTimeOffset(new DateTime(2011, 07, 19, 11, 27, 16))
                                }
                            }
                        }
                };
            }
        }
    }
}
