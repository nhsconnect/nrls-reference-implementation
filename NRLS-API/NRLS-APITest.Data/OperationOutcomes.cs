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
                    Meta = new Meta
                    {
                        Profile = new List<string> {
                            "https://fhir.nhs.uk/STU3/StructureDefinition/Spine-OperationOutcome-1-0"
                        }
                    },
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
    }
}
