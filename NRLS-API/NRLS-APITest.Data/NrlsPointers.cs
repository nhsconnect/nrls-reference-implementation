using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NRLS_APITest.Data
{
    public class NrlsPointers
    {
        public static DocumentReference Valid
        {
            get
            {
                return new DocumentReference
                {
                    Id = "5ab13f41957d0ad5d93a1339",
                    Status = DocumentReferenceStatus.Current,
                    Type = new CodeableConcept
                    {
                        Coding = new List<Coding>
                            {
                                new Coding
                                {
                                    System = "http://snomed.info/sct",
                                    Code = "736253002",
                                    Display = "Mental health crisis plan (record artifact)"
                                }
                            }
                    },
                    Subject = new ResourceReference
                    {
                        Reference = "https://demographics.spineservices.nhs.uk/STU3/Patient/2686033207"
                    },
                    Indexed = new DateTime(2005, 12, 24, 9, 43, 41),
                    Author = new List<ResourceReference>
                        {
                            new ResourceReference
                            {
                                Reference = "https://directory.spineservices.nhs.uk/STU3/Organization/1XR"
                            }
                        },
                    Custodian = new ResourceReference
                    {
                        Reference = "https://directory.spineservices.nhs.uk/STU3/Organization/1XR"
                    },
                    Content = new List<DocumentReference.ContentComponent>
                        {
                            new DocumentReference.ContentComponent
                            {
                                Attachment = new Attachment
                                {
                                    ContentType = "application/pdf",
                                    Url = "http://example.org/xds/mhd/Binary/07a6483f-732b-461e-86b6-edb665c45510.pdf",
                                    Title = "Mental health Care Plan Report",
                                    Creation = "2016-03-08T15:26:00+00:00"
                                }
                            }
                        }
                };
            }
        }

        public static DocumentReference Valid_With_MasterId
        {
            get
            {
                var pointer = Valid;
                pointer.MasterIdentifier = new Identifier("testSystem.com", "testValue");
                return pointer;
            }
        }

        public static DocumentReference Valid_With_MasterId_Alt
        {
            get
            {
                var pointer = Valid;
                pointer.MasterIdentifier = new Identifier("testSystem.com", "testValueForMaintTest");
                return pointer;
            }
        }

        public static DocumentReference Invalid_MasterId_Value
        {
            get
            {
                var pointer = Valid;
                pointer.MasterIdentifier = new Identifier("testSystem.com", null);
                return pointer;
            }
        }

        public static DocumentReference Invalid_MasterId_System
        {
            get
            {
                var pointer = Valid;
                pointer.MasterIdentifier = new Identifier(null, "testValue");
                return pointer;
            }
        }

        public static DocumentReference Invalid_Status
        {
            get
            {
                var pointer = Valid;
                pointer.Status = null;
                pointer.Id = "5affc72bcae33eb8690e5881";
                return pointer;
            }
        }

        public static DocumentReference Invalid_Type
        {
            get
            {
                var pointer = Valid;
                pointer.Type.Coding.First().Code = null;

                return pointer;
            }
        }

        public static DocumentReference Missing_Type
        {
            get
            {
                var pointer = Valid;
                pointer.Type = null;

                return pointer;
            }
        }

        public static DocumentReference Invalid_Subject
        {
            get
            {
                var pointer = Valid;
                pointer.Subject.Reference = "https://demographics.spineservices.nhs.uk/STU3/Patient/";

                return pointer;
            }
        }

        public static DocumentReference Missing_Subject
        {
            get
            {
                var pointer = Valid;
                pointer.Subject = null;

                return pointer;
            }
        }

        public static DocumentReference Invalid_Indexed
        {
            get
            {
                var pointer = Valid;
                pointer.Indexed = null;

                return pointer;
            }
        }

        public static DocumentReference Invalid_Author
        {
            get
            {
                var pointer = Valid;
                pointer.Author.First().Reference = "InvalidAuthorhttps://directory.spineservices.nhs.uk/STU3/Organization/";

                return pointer;
            }
        }

        public static DocumentReference Missing_Author
        {
            get
            {
                var pointer = Valid;
                pointer.Author = null;

                return pointer;
            }
        }

        public static DocumentReference TooMany_Author
        {
            get
            {
                var pointer = Valid;
                var authour = pointer.Author.First();

                pointer.Author.Add(authour);

                return pointer;
            }
        }

        public static DocumentReference Invalid_Custodian
        {
            get
            {
                var pointer = Valid;
                pointer.Custodian.Reference = "InvalidCustodianhttps://directory.spineservices.nhs.uk/STU3/Organization/";

                return pointer;
            }
        }

        public static DocumentReference Invalid_Custodian_Alt
        {
            get
            {
                var pointer = Valid;
                pointer.Custodian.Reference = "https://directory.spineservices.nhs.uk/STU3/Organization/error";

                return pointer;
            }
        }

        public static DocumentReference Missing_Custodian
        {
            get
            {
                var pointer = Valid;
                pointer.Custodian = null;

                return pointer;
            }
        }

        public static DocumentReference Missing_Content
        {
            get
            {
                var pointer = Valid;
                pointer.Content = null;

                return pointer;
            }
        }
     

    }
}
