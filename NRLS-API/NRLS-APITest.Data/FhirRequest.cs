using Hl7.Fhir.Model;
using NRLS_API.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NRLS_APITest.Data
{
    public class FhirRequests
    {
        public static FhirRequest Valid_Create
        {
            get
            {
                return new FhirRequest
                {
                    RequestingAsid = "000",
                    Resource = NrlsPointers.Valid,
                    ResourceType = ResourceType.DocumentReference
                };
            }
        }

        public static FhirRequest Valid_Create_MasterId
        {
            get
            {
                return new FhirRequest
                {
                    RequestingAsid = "000",
                    Resource = NrlsPointers.Valid_With_MasterId_Alt,
                    ResourceType = ResourceType.DocumentReference
                };
            }
        }

        

        public static FhirRequest Valid_Create_Alt
        {
            get
            {
                return new FhirRequest
                {
                    RequestingAsid = "001",
                    Resource = NrlsPointers.Valid,
                    ResourceType = ResourceType.DocumentReference
                };
            }
        }

        public static FhirRequest Invalid_Create
        {
            get
            {
                return new FhirRequest
                {
                    RequestingAsid = "000",
                    Resource = NrlsPointers.Invalid_Status,
                    ResourceType = ResourceType.DocumentReference
                };
            }
        }

        public static FhirRequest Invalid_Create_NoDocument
        {
            get
            {
                return new FhirRequest
                {
                    RequestingAsid = "000",
                    Resource = null,
                    ResourceType = ResourceType.DocumentReference
                };
            }
        }

        public static FhirRequest Invalid_Custodian
        {
            get
            {
                return new FhirRequest
                {
                    RequestingAsid = "002",
                    Resource = NrlsPointers.Invalid_Custodian_Alt,
                    ResourceType = ResourceType.DocumentReference
                };
            }
        }

        public static FhirRequest Valid_Read
        {
            get
            {
                return new FhirRequest
                {
                    Id = "testid",
                    RequestingAsid = "000",
                    RequestUrl = new Uri("https://testdomain/testurl"),
                    Resource = NrlsPointers.Valid,
                    ResourceType = ResourceType.DocumentReference,
                    QueryParameters = new List<Tuple<string, string>>
                    {
                        new Tuple<string, string>("_id", "5b5c5bec7f1c649fdea426a1")
                    },
                    AllowedParameters = new string[] { "_id" }
                };
            }
        }

        public static FhirRequest Valid_Delete_Path_Id
        {
            get
            {
                return new FhirRequest
                {
                    RequestingAsid = "003",
                    RequestUrl = new Uri("https://testdomain/testurl"),
                    ResourceType = ResourceType.DocumentReference,
                    Id = "5b5c5bec7f1c649fdea426a1",
                    AuditId = "91370360-d667-4bc8-bebe-f223560ff90e"
                };
            }
        }

        public static FhirRequest Valid_Delete_Query_Id
        {
            get
            {
                return new FhirRequest
                {
                    RequestingAsid = "003",
                    RequestUrl = new Uri("https://testdomain/testurl"),
                    ResourceType = ResourceType.DocumentReference,
                    QueryParameters = new List<Tuple<string, string>>
                    {
                        new Tuple<string, string>("_id", "5b5c5bec7f1c649fdea426a1")
                    },
                    AuditId = "91370360-d667-4bc8-bebe-f223560ff90e"
                };
            }
        }

        public static FhirRequest Valid_Delete_Alt
        {
            get
            {
                return new FhirRequest
                {
                    RequestingAsid = "001",
                    RequestUrl = new Uri("https://testdomain/testurl"),
                    ResourceType = ResourceType.DocumentReference,
                    QueryParameters = new List<Tuple<string, string>>
                    {
                        new Tuple<string, string>("_id", "5b5c5bec7f1c649fdea426a1")
                    }
                };
            }
        }

        public static FhirRequest Invalid_Delete
        {
            get
            {
                return new FhirRequest
                {
                    RequestingAsid = "000",
                    RequestUrl = new Uri("https://testdomain/testurl"),
                    ResourceType = ResourceType.DocumentReference
                };
            }
        }

        public static FhirRequest Invalid_Delete_Path_Id_and_Query
        {
            get
            {
                return new FhirRequest
                {
                    RequestingAsid = "003",
                    RequestUrl = new Uri("https://testdomain/testurl"),
                    ResourceType = ResourceType.DocumentReference,
                    Id = "5b5c5bec7f1c649fdea426a1",
                    AuditId = "91370360-d667-4bc8-bebe-f223560ff90e",
                    QueryParameters = new List<Tuple<string, string>>
                    {
                        new Tuple<string, string>("identifier", "testsystem|testvalue"),
                    },
                    AllowedParameters = new string[] { "identifier" }
                };
            }
        }

        public static FhirRequest Invalid_Delete_NotFound
        {
            get
            {
                return new FhirRequest
                {
                    RequestingAsid = "000",
                    RequestUrl = new Uri("https://testdomain/testurl"),
                    ResourceType = ResourceType.DocumentReference,
                    Id = "5b5c5bec7f1c649fdea426a1"
                };
            }
        }

        public static FhirRequest Valid_ConditionalDelete
        {
            get
            {
                return new FhirRequest
                {
                    RequestingAsid = "003",
                    RequestUrl = new Uri("https://testdomain/testurl"),
                    ProfileUri = "https://fhir.nhs.uk/STU3/StructureDefinition/NRLS-DocumentReference-1",
                    ResourceType = ResourceType.DocumentReference,
                    QueryParameters = new List<Tuple<string, string>>
                    {
                        new Tuple<string, string>("identifier", "testsystem|testvalue"),
                        new Tuple<string, string>("subject", "https://demographics.spineservices.nhs.uk/STU3/Patient/2686033207")
                    },
                    AllowedParameters = new string[] { "custodian", "subject", "identifier" }
                };
            }
        }

        public static FhirRequest Invalid_ConditionalDelete_NoSearchValues
        {
            get
            {
                return new FhirRequest
                {
                    RequestingAsid = "000",
                    RequestUrl = new Uri("https://testdomain/testurl"),
                    ProfileUri = "https://fhir.nhs.uk/STU3/StructureDefinition/NRLS-DocumentReference-1",
                    ResourceType = ResourceType.DocumentReference,
                    QueryParameters = new List<Tuple<string, string>>
                    {
                        new Tuple<string, string>("identifier", ""),
                        new Tuple<string, string>("subject", "")
                    },
                    AllowedParameters = new string[] { "custodian", "subject", "identifier" }
                };
            }
        }

        public static FhirRequest Invalid_ConditionalDelete_NoSubject
        {
            get
            {
                return new FhirRequest
                {
                    RequestingAsid = "000",
                    RequestUrl = new Uri("https://testdomain/testurl"),
                    ProfileUri = "https://fhir.nhs.uk/STU3/StructureDefinition/NRLS-DocumentReference-1",
                    ResourceType = ResourceType.DocumentReference,
                    QueryParameters = new List<Tuple<string, string>>
                    {
                        new Tuple<string, string>("identifier", "testsystem|testvalue")
                    }
                };
            }
        }

        public static FhirRequest Invalid_ConditionalDelete_IncompleteIdentifier
        {
            get
            {
                return new FhirRequest
                {
                    RequestingAsid = "000",
                    RequestUrl = new Uri("https://testdomain/testurl"),
                    ProfileUri = "https://fhir.nhs.uk/STU3/StructureDefinition/NRLS-DocumentReference-1",
                    ResourceType = ResourceType.DocumentReference,
                    QueryParameters = new List<Tuple<string, string>>
                    {
                        new Tuple<string, string>("identifier", "|testvalue"),
                        new Tuple<string, string>("subject", "https://demographics.spineservices.nhs.uk/STU3/Patient/2686033207")
                    }
                };
            }
        }

        public static FhirRequest Invalid_ConditionalDelete
        {
            get
            {
                return new FhirRequest
                {
                    RequestingAsid = "001",
                    RequestUrl = new Uri("https://testdomain/testurl"),
                    ProfileUri = "https://fhir.nhs.uk/STU3/StructureDefinition/NRLS-DocumentReference-1",
                    ResourceType = ResourceType.DocumentReference,
                    QueryParameters = new List<Tuple<string, string>>
                    {
                        new Tuple<string, string>("identifier", "testsystem|testvalue"),
                        new Tuple<string, string>("subject", "https://demographics.spineservices.nhs.uk/STU3/Patient/2686033207")
                    },
                    AllowedParameters = new string[] { "custodian", "subject", "identifier" }
                };
            }
        }

        public static FhirRequest Valid_Search
        {
            get
            {
                return new FhirRequest
                {
                    Id = "testid",
                    RequestingAsid = "000",
                    RequestUrl = new Uri("https://testdomain/testurl"),
                    Resource = NrlsPointers.Valid,
                    ResourceType = ResourceType.DocumentReference,
                    ProfileUri = "https://fhir.nhs.uk/STU3/StructureDefinition/NRLS-DocumentReference-1",
                    QueryParameters = new List<Tuple<string, string>>
                    {
                        new Tuple<string, string>("subject", "https://demographics.spineservices.nhs.uk/STU3/Patient/1234"),
                        new Tuple<string, string>("custodian", "https://directory.spineservices.nhs.uk/STU3/Organization/TestOrgCode")
                    },
                    AllowedParameters = new string[] { "custodian", "subject" }
                };
            }
        }

        public static FhirRequest Valid_Search_Summary
        {
            get
            {
                return new FhirRequest
                {
                    Id = "testid",
                    RequestingAsid = "000",
                    RequestUrl = new Uri("https://testdomain/testurl"),
                    Resource = NrlsPointers.Valid,
                    ResourceType = ResourceType.DocumentReference,
                    ProfileUri = "https://fhir.nhs.uk/STU3/StructureDefinition/NRLS-DocumentReference-1",
                    QueryParameters = new List<Tuple<string, string>>
                    {
                        new Tuple<string, string>("subject", "https://demographics.spineservices.nhs.uk/STU3/Patient/1234"),
                        new Tuple<string, string>("_summary", "count")
                    },
                    AllowedParameters = new string[] { "_summary", "subject" }
                };
            }
        }

        public static FhirRequest Valid_Search_No_Params
        {
            get
            {
                return new FhirRequest
                {
                    Id = "testid",
                    RequestingAsid = "000",
                    RequestUrl = new Uri("https://testdomain/testurl"),
                    Resource = NrlsPointers.Valid,
                    ResourceType = ResourceType.DocumentReference,
                    AllowedParameters = new string[] { "custodian", "Patient" }
                };
            }
        }

        public static FhirRequest Invalid_Search_Invalid_Params
        {
            get
            {
                return new FhirRequest
                {
                    Id = "testid",
                    RequestingAsid = "000",
                    RequestUrl = new Uri("https://testdomain/testurl"),
                    Resource = NrlsPointers.Valid,
                    ResourceType = ResourceType.DocumentReference,
                    ProfileUri = "https://fhir.nhs.uk/STU3/StructureDefinition/NRLS-DocumentReference-1",
                    QueryParameters = new List<Tuple<string, string>>
                    {
                        new Tuple<string, string>("practitioner", "https://demographics.spineservices.nhs.uk/STU3/Patient/1234"),
                    },
                    AllowedParameters = new string[] { "custodian", "Patient" }
                };
            }
        }

        public static FhirRequest Invalid_Search
        {
            get
            {
                return new FhirRequest
                {
                    Id = "testid",
                    RequestingAsid = "000",
                    RequestUrl = new Uri("https://testdomain/testurl"),
                    Resource = NrlsPointers.Valid,
                    ResourceType = ResourceType.DocumentReference,
                    QueryParameters = new List<Tuple<string, string>>
                    {
                        new Tuple<string, string>("custodian", "https://directory.spineservices.nhs.uk/STU3/Organization/odscode")
                    }
                };
            }
        }

        public static FhirRequest Invalid_Read_TooManyParams
        {
            get
            {
                return new FhirRequest
                {
                    Id = "testid",
                    RequestingAsid = "000",
                    RequestUrl = new Uri("https://testdomain/testurl"),
                    Resource = NrlsPointers.Valid,
                    ResourceType = ResourceType.DocumentReference,
                    QueryParameters = new List<Tuple<string, string>>
                    {
                        new Tuple<string, string>("_id", "testId"),
                        new Tuple<string, string>("subject", "https://demographics.spineservices.nhs.uk/STU3/Patient/1234")
                    }
                };
            }
        }

        public static FhirRequest Invalid_Read_EmptyId
        {
            get
            {
                return new FhirRequest
                {
                    Id = "testid",
                    RequestingAsid = "000",
                    RequestUrl = new Uri("https://testdomain/testurl"),
                    Resource = NrlsPointers.Valid,
                    ResourceType = ResourceType.DocumentReference,
                    QueryParameters = new List<Tuple<string, string>>
                    {
                        new Tuple<string, string>("_id", ""),
                        new Tuple<string, string>("subject", "https://demographics.spineservices.nhs.uk/STU3/Patient/1234")
                    }
                };
            }
        }

        public static FhirRequest Invalid_Search_IncorrectSummary
        {
            get
            {
                return new FhirRequest
                {
                    Id = "testid",
                    RequestingAsid = "000",
                    RequestUrl = new Uri("https://testdomain/testurl"),
                    Resource = NrlsPointers.Valid,
                    ResourceType = ResourceType.DocumentReference,
                    QueryParameters = new List<Tuple<string, string>>
                    {
                        new Tuple<string, string>("_summary", "notcount"),
                        new Tuple<string, string>("patient", "https://demographics.spineservices.nhs.uk/STU3/Patient/1234")
                    }
                };
            }
        }

        public static FhirRequest Valid_Update
        {
            get
            {
                return new FhirRequest
                {
                    RequestingAsid = "000",
                    Resource = NrlsPointers.Valid_AltCustodian_With_MasterId_and_RelatesTo,
                    ResourceType = ResourceType.DocumentReference
                };
            }
        }

        public static FhirRequest Valid_Update_Alt
        {
            get
            {
                return new FhirRequest
                {
                    RequestingAsid = "001",
                    Resource = NrlsPointers.Valid_AltCustodian_With_MasterId_and_RelatesTo,
                    ResourceType = ResourceType.DocumentReference
                };
            }
        }

        public static FhirRequest Valid_Update_Alt2
        {
            get
            {
                return new FhirRequest
                {
                    RequestingAsid = "002",
                    Resource = NrlsPointers.Valid_AltCust2_With_MasterId_and_RelatesTo,
                    ResourceType = ResourceType.DocumentReference
                };
            }
        }

        public static FhirRequest Valid_Update_ByReference
        {
            get
            {
                return new FhirRequest
                {
                    RequestingAsid = "002",
                    Resource = NrlsPointers.Valid_AltCustodian_With_RelatesToReference,
                    ResourceType = ResourceType.DocumentReference
                };
            }
        }

        public static FhirRequest Invalid_Update_Bad_Status
        {
            get
            {
                return new FhirRequest
                {
                    RequestingAsid = "003",
                    Resource = NrlsPointers.Valid_AltCustodian_With_MasterId_and_RelatesTo,
                    ResourceType = ResourceType.DocumentReference
                };
            }
        }

        public static FhirRequest Invalid_Update_PatientMismatch
        {
            get
            {
                return new FhirRequest
                {
                    RequestingAsid = "patientmismatch",
                    Resource = NrlsPointers.Valid_With_AltPatient,
                    ResourceType = ResourceType.DocumentReference
                };
            }
        }

        public static FhirRequest Invalid_Update_RelatesToNoRelatedIdentifier
        {
            get
            {
                return new FhirRequest
                {
                    RequestingAsid = "RelatesToNoRelatedIdentifier",
                    Resource = NrlsPointers.Valid_AltCustodian_With_RelatesToReferenceAndIdentifier,
                    ResourceType = ResourceType.DocumentReference
                };
            }
        }

        public static FhirRequest Invalid_Update_RelatesToInvalidRelatedIdentifier
        {
            get
            {
                return new FhirRequest
                {
                    RequestingAsid = "RelatesToInvalidRelatedIdentifier",
                    Resource = NrlsPointers.Valid_AltCustodian_With_RelatesToReferenceAndIdentifier,
                    ResourceType = ResourceType.DocumentReference
                };
            }
        }

        public static FhirRequest Invalid_Update_Bad_RelatesTo
        {
            get
            {
                return new FhirRequest
                {
                    RequestingAsid = "000",
                    Resource = NrlsPointers.Valid_AltCust_With_MasterId_and_Bad_RelatesTo,
                    ResourceType = ResourceType.DocumentReference
                };
            }
        }
    }
}
