using Hl7.Fhir.Model;
using System.Collections.Generic;

namespace NRLS_APITest.Data
{
    public class OperationOutcomes
    {
        public static OperationOutcome Ok
        {
            get
            {
                return new OperationOutcome()
                {
                    Meta = new Meta
                    {
                        Profile = new List<string> {
                            "https://fhir.nhs.uk/STU3/StructureDefinition/Spine-OperationOutcome-1-0"
                        }
                    }
                };
            }
        }

        public static OperationOutcome Error
        {
            get
            {
                return new OperationOutcome()
                {
                    //Meta = new Meta
                    //{
                    //    Profile = new List<string> {
                    //        "https://fhir.nhs.uk/STU3/StructureDefinition/Spine-OperationOutcome-1-0"
                    //    }
                    //},
                    Issue = new List<OperationOutcome.IssueComponent>
                    {
                        new OperationOutcome.IssueComponent
                        {
                            Code = OperationOutcome.IssueType.Invalid,
                            Severity = OperationOutcome.IssueSeverity.Error,
                            Diagnostics = "Error"
                        }
                    }
                };
            }
        }

        public static OperationOutcome Deleted
        {
            get
            {
                return new OperationOutcome()
                {
                    //Meta = new Meta
                    //{
                    //    Profile = new List<string> {
                    //        "https://fhir.nhs.uk/STU3/StructureDefinition/Spine-OperationOutcome-1-0"
                    //    }
                    //},
                    Issue = new List<OperationOutcome.IssueComponent>
                    {
                        new OperationOutcome.IssueComponent
                        {
                            Code = OperationOutcome.IssueType.Informational,
                            Severity = OperationOutcome.IssueSeverity.Information,
                            Diagnostics = "Successfully removed resource DocumentReference: https://testdomain/testurl",
                            Details = new CodeableConcept
                            {
                                Coding = new List<Coding>
                                {
                                    new Coding
                                    {
                                        System = "https://fhir.nhs.uk/STU3/ValueSet/spine-response-code-2-0",
                                        Code = "RESOURCE_DELETED", 
                                        Display = "Resource removed"
                                    }
                                },
                                Text = "91370360-d667-4bc8-bebe-f223560ff90e"
                            }
                        }
                    }
                };
            }
        }

        public static OperationOutcome NotFound
        {
            get
            {
                return new OperationOutcome()
                {
                    //Meta = new Meta
                    //{
                    //    Profile = new List<string> {
                    //        "https://fhir.nhs.uk/STU3/StructureDefinition/Spine-OperationOutcome-1-0"
                    //    }
                    //},
                    Issue = new List<OperationOutcome.IssueComponent>
                    {
                        new OperationOutcome.IssueComponent
                        {
                            Code = OperationOutcome.IssueType.NotFound,
                            Severity = OperationOutcome.IssueSeverity.Error,
                            Diagnostics = "No record found for supplied DocumentReference identifier – 5b5c5bec7f1c649fdea426a1.",
                            Details = new CodeableConcept
                            {
                                Coding = new List<Coding>
                                {
                                    new Coding
                                    {
                                        System = "https://fhir.nhs.uk/STU3/ValueSet/spine-response-code-2-0",
                                        Code = "NO_RECORD_FOUND",
                                        Display = "No record found"
                                    }
                                }
                            }
                        }
                    }
                };
            }
        }
    }
}
