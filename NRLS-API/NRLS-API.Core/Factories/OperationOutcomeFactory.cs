using Hl7.Fhir.Model;
using NRLS_API.Core.Resources;
using System.Collections.Generic;

namespace NRLS_API.Core.Factories
{
    public static class OperationOutcomeFactory
    {
        public static OperationOutcome Create(OperationOutcome.IssueSeverity issueSeverity, OperationOutcome.IssueType issueType, string diagnostics, CodeableConcept details)
        {
            var outcome = new OperationOutcome
            {
                Meta = new Meta
                {
                    Profile = new List<string>{
                        FhirConstants.SDSpineOpOutcome
                    }
                },
                Issue = new List<OperationOutcome.IssueComponent>
                {
                    new OperationOutcome.IssueComponent
                    {
                        Severity = issueSeverity,
                        Code = issueType,
                        Diagnostics = diagnostics,
                        Details = details
                    }
                }
            };

            return outcome;
        }

        public static OperationOutcome CreateInternalError(string diagnostics)
        {

            var details = new CodeableConcept
            {
                Coding = new List<Coding>
                {
                    new Coding
                    {
                        System = FhirConstants.SystemOpOutcome,
                        Code = "INTERNAL_SERVER_ERROR",
                        Display = "Internal Server Error"
                    }
                }
            };

            return Create(OperationOutcome.IssueSeverity.Fatal, OperationOutcome.IssueType.Exception, diagnostics, details);
        }

        public static OperationOutcome CreateInvalidResource(string resourceType)
        {

            var details = new CodeableConcept
            {
                Coding = new List<Coding>
                {
                    new Coding
                    {
                        System = FhirConstants.SystemOpOutcome,
                        Code = "INVALID_RESOURCE",
                        Display = "Invalid validation of resource"
                    }
                }
            };

            return Create(OperationOutcome.IssueSeverity.Error, OperationOutcome.IssueType.Invalid, $"The requested resource {resourceType} is invalid.", details);
        }

        public static OperationOutcome CreateInvalidHeader(string resourceType, string header)
        {

            var details = new CodeableConcept
            {
                Coding = new List<Coding>
                {
                    new Coding
                    {
                        System = FhirConstants.SystemOpOutcome,
                        Code = "MISSING_OR_INVALID_HEADER",
                        Display = "There is a required header missing or invalid"
                    }
                }
            };

            return Create(OperationOutcome.IssueSeverity.Error, OperationOutcome.IssueType.Invalid, $"{header} HTTP Header is missing or invalid.", details);
        }
    }
}
