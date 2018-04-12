using Hl7.Fhir.Model;
using NRLS_API.Core.Resources;
using System;
using System.Collections.Generic;

namespace NRLS_API.Core.Factories
{
    public static class OperationOutcomeFactory
    {
        public static OperationOutcome Create(OperationOutcome.IssueSeverity issueSeverity, OperationOutcome.IssueType issueType, string diagnostics, CodeableConcept details)
        {
            var outcome = Base();

            outcome.Issue = new List<OperationOutcome.IssueComponent>
            {
                new OperationOutcome.IssueComponent
                {
                    Severity = issueSeverity,
                    Code = issueType,
                    Diagnostics = diagnostics,
                    Details = details
                }
            };

            return outcome;
        }

        public static OperationOutcome CreateError(string diagnostics, CodeableConcept details)
        {
            return Create(OperationOutcome.IssueSeverity.Error, OperationOutcome.IssueType.Invalid, diagnostics, details);
        }

        public static OperationOutcome CreateInternalError(string diagnostics)
        {

            var details = CreateDetails("INTERNAL_SERVER_ERROR", "Internal Server Error");

            return Create(OperationOutcome.IssueSeverity.Fatal, OperationOutcome.IssueType.Exception, diagnostics, details);
        }

        public static OperationOutcome CreateInvalidResourceType(string resourceType)
        {
            var details = CreateDetails("INVALID_RESOURCE", "ResourceType is invalid");

            return CreateError($"The requested resource {resourceType} is invalid.", details);
        }

        public static OperationOutcome CreateInvalidResource(string property, string diagnostics = null)
        {
            var display = "Invalid validation of resource";

            if (!string.IsNullOrEmpty(property) && string.IsNullOrEmpty(diagnostics))
            {
                diagnostics = $"Resource is invalid : {property}";
            }

            if (!string.IsNullOrEmpty(diagnostics))
            {
                display = $"Resource is invalid : {property}";
            }

            var details = CreateDetails("INVALID_RESOURCE", display);

            return CreateError(diagnostics, details);
        }

        public static OperationOutcome CreateInvalidParameter(string display, string diagnostics = null)
        {
            var details = CreateDetails("INVALID_PARAMETER", display);

            return CreateError(diagnostics, details);
        }

        public static OperationOutcome CreateInvalidMediaType(string diagnostics = null)
        {
            if (string.IsNullOrEmpty(diagnostics))
            {
                diagnostics = "Unsupported Media Type";
            }

            var details = CreateDetails("UNSUPPORTED_MEDIA_TYPE", "Unsupported Media Type", FhirConstants.SystemOpOutcome1);

            return CreateError(diagnostics, details);
        }

        public static OperationOutcome CreateInvalidNhsNumberRes(string value)
        {

            return CreateInvalidNhsNumber($"The NHS number does not conform to the NHS Number format: {value}");
        }

        public static OperationOutcome CreateInvalidNhsNumber(string diagnostics = null)
        {

            if (string.IsNullOrEmpty(diagnostics))
            {
                diagnostics = "The NHS number is missing from the search request patient URL";
            }

            var details = CreateDetails("INVALID_NHS_NUMBER", "Invalid NHS number");

            return CreateError(diagnostics, details);
        }

        public static OperationOutcome CreateInvalidHeader(string header)
        {

            var details = CreateDetails("MISSING_OR_INVALID_HEADER", "There is a required header missing or invalid");

            return CreateError($"{header} HTTP Header is missing or invalid.", details);
        }

        public static OperationOutcome CreateNotFound(string id)
        {

            var details = CreateDetails("NO_RECORD_FOUND", "No record found");

            return CreateError($"No record found for supplied DocumentReference identifier – {id}.", details);
        }

        public static OperationOutcome CreateOk()
        {
            return Base();
        }

        public static OperationOutcome CreateSuccess()
        {
            var details = CreateDetails(null, null, null, Guid.NewGuid().ToString());

            return Create(OperationOutcome.IssueSeverity.Information, OperationOutcome.IssueType.Informational, $"Successfully created resource DocumentReference", details);
        }

        public static OperationOutcome CreateDelete(string url)
        {
            return Create(OperationOutcome.IssueSeverity.Information, OperationOutcome.IssueType.Informational, $"Successfully removed resource DocumentReference: {url}", null);
        }

        public static CodeableConcept CreateDetails(string code, string display, string system = null, string text = null)
        {

            if(string.IsNullOrEmpty(system))
            {
                system = FhirConstants.SystemOpOutcome;
            }

            var details = new CodeableConcept
            {
                Text = text
            };

            if(!string.IsNullOrEmpty(code))
            {
                details.Coding = new List<Coding>
                {
                    new Coding
                    {
                        System = system,
                        Code = code,
                        Display = display
                    }
                };
            }

            return details;
        }

        private static OperationOutcome Base()
        {
            var outcome = new OperationOutcome
            {
                Meta = new Meta
                {
                    Profile = new List<string>{
                        FhirConstants.SDSpineOpOutcome
                    }
                }
            };

            return outcome;
        }
    }
}
