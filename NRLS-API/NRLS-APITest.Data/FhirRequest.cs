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
                        new Tuple<string, string>("_id", "testId")
                    }
                };
            }
        }

        public static FhirRequest Valid_Delete
        {
            get
            {
                return new FhirRequest
                {
                    RequestingAsid = "000",
                    RequestUrl = new Uri("https://testdomain/testurl"),
                    ResourceType = ResourceType.DocumentReference,
                    QueryParameters = new List<Tuple<string, string>>
                    {
                        new Tuple<string, string>("_id", "testId")
                    },
                    Id = "5b5c5bec7f1c649fdea426a1",
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
                        new Tuple<string, string>("_id", "testId")
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

        public static FhirRequest Valid_ConditionalDelete
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
                        new Tuple<string, string>("custodian", "https://directory.spineservices.nhs.uk/STU3/Organization/odscode")
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
                        new Tuple<string, string>("patient", "https://demographics.spineservices.nhs.uk/STU3/Patient/1234")
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
                        new Tuple<string, string>("patient", "https://demographics.spineservices.nhs.uk/STU3/Patient/1234")
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
    }
}
