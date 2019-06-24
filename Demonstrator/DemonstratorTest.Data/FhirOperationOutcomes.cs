using Hl7.Fhir.Model;
using System.Collections.Generic;

namespace DemonstratorTest.Data
{
    public static class FhirOperationOutcomes
    {
        public static OperationOutcome NotFound
        {
            get
            {
                return new OperationOutcome()
                {
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
