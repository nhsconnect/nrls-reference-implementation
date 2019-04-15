using Demonstrator.Core.Resources;
using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;

namespace Demonstrator.Core.Factories
{
    public static class OperationOutcomeFactory
    {
        public static OperationOutcome Create(OperationOutcome.IssueSeverity issueSeverity, OperationOutcome.IssueType issueType, string diagnostics, CodeableConcept details, bool spineError)
        {
            var outcome = Base(spineError);

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

        public static OperationOutcome CreateError(string diagnostics, CodeableConcept details, OperationOutcome.IssueType? issueType = null, bool spineError = false)
        {
            var issueTypeVal = issueType.HasValue ? issueType.Value : OperationOutcome.IssueType.Invalid;
            return Create(OperationOutcome.IssueSeverity.Error, issueTypeVal, diagnostics, details, spineError);
        }

        public static OperationOutcome CreateInternalError(string diagnostics)
        {

            var details = CreateDetails("INTERNAL_SERVER_ERROR", "Unexpected internal server error");

            return Create(OperationOutcome.IssueSeverity.Error, OperationOutcome.IssueType.Invalid, diagnostics, details, false);
        }

        public static OperationOutcome CreateAccessDenied()
        {
            var details = CreateDetails("ACCESS_DENIED", "ResourceType is invalid");

            return CreateError($"Invalid Client Connection.", details, null);
        }

        public static OperationOutcome CreateInvalidResourceType(string resourceType)
        {
            var details = CreateDetails("INVALID_RESOURCE", "ResourceType is invalid");

            return CreateError($"The requested resource {resourceType} is invalid.", details, null);
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

            return CreateError(diagnostics, details, null);
        }

        public static OperationOutcome CreateInvalidParameter(string display, string diagnostics = null)
        {
            var details = CreateDetails("INVALID_PARAMETER", display);

            return CreateError(diagnostics, details, null);
        }

        public static OperationOutcome CreateInvalidMediaType(string diagnostics = null)
        {
            if (string.IsNullOrEmpty(diagnostics))
            {
                diagnostics = "Unsupported Media Type";
            }

            var details = CreateDetails("UNSUPPORTED_MEDIA_TYPE", "Unsupported Media Type", true);

            return CreateError(diagnostics, details, null, true);
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

            return CreateError(diagnostics, details, null);
        }

        public static OperationOutcome CreateInvalidHeader(string header, string diagnostics = null)
        {

            var details = CreateDetails("MISSING_OR_INVALID_HEADER", "There is a required header missing or invalid");

            if (string.IsNullOrEmpty(diagnostics))
            {
                diagnostics = $"{header} HTTP Header is missing";
            }

            return CreateError(diagnostics, details, null);

        }

        public static OperationOutcome CreateInvalidJwtHeader(string header, string diagnostics = null)
        {

            var details = CreateDetails("MISSING_OR_INVALID_HEADER", "There is a required header missing or invalid");

            if (string.IsNullOrEmpty(diagnostics))
            {
                diagnostics = $"{header} HTTP Header is missing";
            }

            return CreateError(diagnostics, details, OperationOutcome.IssueType.Structure);

        }

        public static OperationOutcome CreateGenericError(string additionalDiagnostics = null)
        {

            var details = CreateDetails("BAD_REQUEST", "Bad request");
            var diagnostics = $"The request has failed.";

            if (!string.IsNullOrEmpty(additionalDiagnostics))
            {
                diagnostics = $"{diagnostics} Reason: {additionalDiagnostics}";
            }

            return CreateError(diagnostics, details);

        }

        public static OperationOutcome CreateNotFound(string id)
        {

            var details = CreateDetails("NO_RECORD_FOUND", "No Record Found");

            return CreateError($"No record found for supplied DocumentReference identifier - {id}.", details, OperationOutcome.IssueType.NotFound);
        }

        public static OperationOutcome CreateOrganizationNotFound(string id)
        {

            var details = CreateDetails("ORGANISATION_NOT_FOUND", "Organisation record not found");

            return CreateError($"The ODS code in the custodian and/or author element is not resolvable – {id}.", details, OperationOutcome.IssueType.NotFound);
        }

        public static OperationOutcome CreateInvalidRequest()
        {

            var details = CreateDetails("INVALID_REQUEST_MESSAGE", "Invalid Request Message");

            return CreateError("Invalid Request Message", details, OperationOutcome.IssueType.Value);
        }

        public static OperationOutcome CreateDuplicateRequest(Identifier masterIdentifier)
        {

            var details = CreateDetails("DUPLICATE_REJECTED", "Duplicate DocumentReference");

            return CreateError($"Duplicate masterIdentifier value: {masterIdentifier.Value} system: {masterIdentifier.System}", details, OperationOutcome.IssueType.Duplicate);
        }

        public static OperationOutcome CreateOk()
        {
            return Base(false);
        }

        public static OperationOutcome CreateSuccess()
        {
            var details = CreateDetails("RESOURCE_CREATED", "New resource created", false, Guid.NewGuid().ToString());

            return Create(OperationOutcome.IssueSeverity.Information, OperationOutcome.IssueType.Informational, $"Successfully created resource DocumentReference", details, false);
        }

        public static OperationOutcome CreateDelete(string url, string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                text = Guid.NewGuid().ToString();
            }

            var details = CreateDetails("RESOURCE_DELETED", "Resource removed", false, text);

            return Create(OperationOutcome.IssueSeverity.Information, OperationOutcome.IssueType.Informational, $"Successfully removed resource DocumentReference: {url}", details, false);
        }

        public static CodeableConcept CreateDetails(string code, string display, bool spineError = false, string text = null)
        {

            var details = new CodeableConcept
            {
                Text = text
            };

            if (!string.IsNullOrEmpty(code))
            {
                details.Coding = new List<Coding>
                {
                    new Coding
                    {
                        System = (spineError ? FhirConstants.SystemOpOutcome1 : FhirConstants.SystemOpOutcome),
                        Code = code,
                        Display = display
                    }
                };
            }

            return details;
        }

        private static OperationOutcome Base(bool spineError)
        {
            var outcome = new OperationOutcome
            {
                Id = $"{Guid.NewGuid()}-{0:D20}",
                Meta = new Meta
                {
                    Profile = new List<string>{
                        (spineError ? FhirConstants.SDSpineOpOutcome1 : FhirConstants.SDSpineOpOutcome)
                    }
                }
            };

            return outcome;
        }
    }
}
