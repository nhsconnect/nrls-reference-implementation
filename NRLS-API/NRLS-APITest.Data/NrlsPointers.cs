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

        public static DocumentReference Valid_With_Alt_Custodian
        {
            get
            {
                var pointer = Valid;
                pointer.Custodian.Reference = "https://directory.spineservices.nhs.uk/STU3/Organization/RV99";

                return pointer;
            }
        }

        public static DocumentReference Valid_With_Alt2_Custodian
        {
            get
            {
                var pointer = Valid;
                pointer.Custodian.Reference = "https://directory.spineservices.nhs.uk/STU3/Organization/RV88";

                return pointer;
            }
        }

        public static DocumentReference Valid_AltCustodian_With_MasterId_and_RelatesTo
        {
            get
            {
                var pointer = Valid;
                pointer.Custodian.Reference = "https://directory.spineservices.nhs.uk/STU3/Organization/RV99";
                pointer.MasterIdentifier = new Identifier("urn:ietf:rfc:4151", "urn:tag:humber.nhs.uk,2004:cdc:600009612675​");
                pointer.RelatesTo = new List<DocumentReference.RelatesToComponent>
                {
                    new DocumentReference.RelatesToComponent
                    {
                        Code = DocumentRelationshipType.Replaces,
                        Target = new ResourceReference
                        {
                            Identifier = new Identifier("urn:ietf:rfc:4151", "urn:tag:humber.nhs.uk,2004:cdc:600009612669​")
                        }
                    }
                };
                return pointer;
            }
        }

        public static DocumentReference Valid_AltCust_With_MasterId_and_RelatesTo_BadStatus
        {
            get
            {
                var pointer = Valid_AltCustodian_With_MasterId_and_RelatesTo;
                pointer.Status = DocumentReferenceStatus.EnteredInError;

                return pointer;
            }
        }

        public static DocumentReference Valid_AltCust2_With_MasterId_and_RelatesTo
        {
            get
            {
                var pointer = Valid_AltCustodian_With_MasterId_and_RelatesTo;
                pointer.Custodian.Reference = "https://directory.spineservices.nhs.uk/STU3/Organization/RV88";

                return pointer;
            }
        }

        public static DocumentReference Valid_AltCust_With_MasterId_and_Bad_RelatesTo
        {
            get
            {
                var pointer = Valid_AltCustodian_With_MasterId_and_RelatesTo;
                pointer.RelatesTo = new List<DocumentReference.RelatesToComponent>
                {
                    new DocumentReference.RelatesToComponent
                    {
                        Target = new ResourceReference()
                    }
                };

                return pointer;
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


        public static DocumentReference Invalid_Subject
        {
            get
            {
                var pointer = Valid;
                pointer.Subject.Reference = "https://demographics.spineservices.nhs.uk/STU3/Patient/";

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

        public static DocumentReference Missing_RelatesTo_System
        {
            get
            {
                var pointer = Valid;
                pointer.RelatesTo = new List<DocumentReference.RelatesToComponent>
                {
                    new DocumentReference.RelatesToComponent
                    {
                        Code = DocumentRelationshipType.Replaces,
                        Target = new ResourceReference
                        {
                            Identifier = new Identifier(null, "value")
                        }
                    }
                };

                return pointer;
            }
        }

        public static DocumentReference Missing_RelatesTo_Value
        {
            get
            {
                var pointer = Valid;
                pointer.RelatesTo = new List<DocumentReference.RelatesToComponent>
                {
                    new DocumentReference.RelatesToComponent
                    {
                        Code = DocumentRelationshipType.Replaces,
                        Target = new ResourceReference
                        {
                            Identifier = new Identifier("system", null)
                        }
                    }
                };

                return pointer;
            }
        }
    }
}
