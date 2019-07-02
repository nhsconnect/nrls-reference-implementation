using Hl7.Fhir.Model;
using Hl7.Fhir.Validation;
using NRLS_API.Core.Factories;
using NRLS_API.Core.Helpers;
using NRLS_API.Core.Interfaces.Services;
using NRLS_API.Core.Resources;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NRLS_API.Services
{
    public class FhirValidation : IFhirValidation
    {
        private readonly IValidationHelper _validationHelper;

        private const string DateTimeFormat = "yyyy-MM-ddTHH:mm:ss.fffzzz";

        public FhirValidation(IValidationHelper validationHelper)
        {
            _validationHelper = validationHelper;
        }

        public OperationOutcome ValidProfile<T>(T resource, string customProfile) where T : Resource
        {
            var customProfiles = new List<string>();

            if (!string.IsNullOrEmpty(customProfile))
            {
                customProfiles.Add(customProfile);
            }

            var result = _validationHelper.Validator.Validate(resource, customProfiles.ToArray());

            return result;
        }

        public OperationOutcome ValidPointer(DocumentReference pointer)
        {

            //Base NRL Pointer Validation
            var profileCheck = ValidProfile<DocumentReference>(pointer, FhirConstants.SystemNrlsProfile);

            if (!profileCheck.Success)
            {
                var error = profileCheck.Issue.FirstOrDefault(x => x.Severity.HasValue && new OperationOutcome.IssueSeverity[] { OperationOutcome.IssueSeverity.Error, OperationOutcome.IssueSeverity.Fatal }.Contains(x.Severity.Value));

                //NOTE: structure of property and diagnostics will not match NRL output
                return OperationOutcomeFactory.CreateInvalidResource(error.Location.First(), error.Details.Text);
            }

            //Below checks cover what profile checker can't check


            //master identifier
            if (pointer.MasterIdentifier != null)
            {
                var masterIdentifierCheck = _validationHelper.ValidIdentifier(pointer.MasterIdentifier, "masterIdentifier");

                if (!masterIdentifierCheck.valid)
                {
                    return OperationOutcomeFactory.CreateInvalidResource(masterIdentifierCheck.issue, "If the masterIdentifier is supplied then the value and system properties are mandatory");
                }
            }

            //status
            if (!GetValidStatus(pointer.Status).HasValue)
            {
                return OperationOutcomeFactory.CreateInvalidResource("status", "The status of a new DocumentReference can only be \"current\"");
            }

            //subject 
            //Just NHS Number Validation
            var validNhsNumber =  ValidatePatientReference(pointer.Subject);

            if(validNhsNumber != null)
            {
                return validNhsNumber;
            }

            //author
            //validate orgcode is done outside of here
            var validAuthor = ValidateOrganisationReference(pointer.Author.First(), "author");

            if (validAuthor != null)
            {
                return validAuthor;
            }


            //custodian
            //validate orgcode for match against fromASID is done outside of here
            var validCustodian = ValidateOrganisationReference(pointer.Custodian, "custodian");

            if (validCustodian != null)
            {
                return validCustodian;
            }

            //relatesTo
            //Only require basic checks here
            //Additional checks are carried out in NrlsMaintain.ValidateConditionalUpdate
            var relatesTo = GetValidRelatesTo(pointer.RelatesTo);
            if (pointer.RelatesTo.Count > 0 && relatesTo.element == null)
            {
                return OperationOutcomeFactory.CreateInvalidResource(relatesTo.issue);
            }

            //Content
            var validContent = ValidateContent(pointer.Content);

            if(validContent != null)
            {
                return validContent;
            }

            //Context
            //Checked in profile checker
            //WARNING: practice setting valueset is stored locally

            return OperationOutcomeFactory.CreateOk();
        }

        public OperationOutcome ValidTypeParameter(string type)
        {

            if(!_validationHelper.ValidReferenceParameter(type, FhirConstants.SystemPointerType))
            {
                return OperationOutcomeFactory.CreateInvalidParameter("Invalid parameter", $"The given terminology system is not of the expected value - {FhirConstants.SystemPointerType}");
            }

            var typeCode = _validationHelper.GetTokenParameterId(type, FhirConstants.SystemPointerType);

            var concept = new CodeableConcept(FhirConstants.VsRecordType, typeCode);


            if (!_validationHelper.ValidCodableConcept(concept, 1, FhirConstants.SystemPointerType, true, false, true, false, FhirConstants.VsRecordType))
            {
                return OperationOutcomeFactory.CreateInvalidParameter("Invalid parameter", $"The given code is not from the expected terminology system - {FhirConstants.SystemPointerType}");
            }

            return null;
        }

        public OperationOutcome ValidSummaryParameter(string summary)
        {

            var allowedSummaries = new[] { "count" };

            if (string.IsNullOrWhiteSpace(summary) || !allowedSummaries.Contains(summary.ToLowerInvariant()))
            {
                return OperationOutcomeFactory.CreateInvalidParameter("Invalid parameter", $"Unsupported search parameter - _summary={summary}");
            }

            return null;
        }

        public OperationOutcome ValidatePatientReference(ResourceReference reference)
        {
            if (!_validationHelper.ValidReference(reference, FhirConstants.SystemPDS))
            {
                return OperationOutcomeFactory.CreateInvalidResource("subject");
            }

            var nhsNumber = GetSubjectReferenceId(reference);

            if (!_validationHelper.ValidNhsNumber(nhsNumber))
            {
                return OperationOutcomeFactory.CreateInvalidNhsNumberRes(nhsNumber);
            }
            
            return null;
        }

        public OperationOutcome ValidateIdentifierParameter(string paramName, string parameterVal)
        {
            if (!_validationHelper.ValidTokenParameter(parameterVal, null, false))
            {
                return OperationOutcomeFactory.CreateInvalidParameter("Invalid parameter", $"The given parameter {paramName} does not confirm to the expected format - [system]|[value]");
            }

            return null;
        }

        public OperationOutcome ValidatePatientParameter(string parameterVal)
        {
            if(!_validationHelper.ValidReferenceParameter(parameterVal, FhirConstants.SystemPDS))
            {
                return OperationOutcomeFactory.CreateInvalidParameter("Invalid parameter", $"The given resource URL does not conform to the expected format - {FhirConstants.SystemPDS}[NHS Number]");
            }

            var nhsNumber = parameterVal.Replace(FhirConstants.SystemPDS, "");

            if (!_validationHelper.ValidNhsNumber(nhsNumber))
            {
                return OperationOutcomeFactory.CreateInvalidNhsNumber();
            }

            return null;
        }

        public OperationOutcome ValidateCustodianParameter(string parameterVal)
        {
            var valid = true;
            if (!_validationHelper.ValidReferenceParameter(parameterVal, FhirConstants.SystemODS))
            {
                valid = false;
            }

            var orgCode = GetOrganizationParameterId(parameterVal);

            if (string.IsNullOrWhiteSpace(orgCode))
            {
                valid = false;
            }

            if (!valid)
            {
                return OperationOutcomeFactory.CreateInvalidParameter("Invalid parameter", $"The given resource URL does not conform to the expected format - {FhirConstants.SystemODS}[ODS Code]");
            }

            return null;
        }

        //Temp method to check custodian.identifier
        public OperationOutcome ValidateCustodianIdentifierParameter(string parameterVal)
        {
            var valid = true;
            if (!_validationHelper.ValidTokenParameter(parameterVal, FhirConstants.SystemOrgCode, false))
            {
                valid = false;
            }

            var orgCode = GetOrganizationParameterIdentifierId(parameterVal);

            if (string.IsNullOrWhiteSpace(orgCode))
            {
                valid = false;
            }

            if (!valid)
            {
                return OperationOutcomeFactory.CreateInvalidParameter("Invalid parameter", $"The given resource URL does not conform to the expected format - {FhirConstants.SystemOrgCode}|[ODS Code]");
            }

            return null;
        }

        public string GetOrganizationParameterId(string parameterVal)
        {
            return _validationHelper.GetOrganisationParameterId(parameterVal);
        }

        public string GetOrganizationParameterIdentifierId(string parameterVal)
        {
            return _validationHelper.GetOrganisationParameterIdentifierId(parameterVal);
        }

        public string GetOrganizationReferenceId(ResourceReference reference)
        {
            return _validationHelper.GetResourceReferenceId(reference, FhirConstants.SystemODS);
        }

        public string GetSubjectReferenceId(ResourceReference reference)
        {
            return _validationHelper.GetResourceReferenceId(reference, FhirConstants.SystemPDS);
        }

        public (DocumentReference.RelatesToComponent element, string issue) GetValidRelatesTo(IList<DocumentReference.RelatesToComponent> relatesToElm)
        {
            //profile checker checks these if we have relatesTo:
            // - there is just one relatesTo
            // - it has a code of replaces
            // - it has a target
            // - has within target, an identifer or reference
            var relatesTo = relatesToElm.FirstOrDefault();

            if(relatesTo == null)
            {
                return (null, null);
            }

            var checkIdentifier = string.IsNullOrWhiteSpace(relatesTo.Target.Reference);

            if (checkIdentifier)
            {
                if (relatesTo.Target.Identifier == null)
                {
                    //reference property takes priority, if both are missing then error is with reference
                    return (null, "relatesTo.target.reference");
                }

                if (string.IsNullOrWhiteSpace(relatesTo.Target.Identifier.System))
                {
                    return (null, "relatesTo.target.identifier.system");
                }

                if (string.IsNullOrWhiteSpace(relatesTo.Target.Identifier.Value))
                {
                    return (null, "relatesTo.target.identifier.value");
                }
            }


            return (relatesTo, "relatesTo");

        }

        public DocumentReferenceStatus? GetValidStatus(DocumentReferenceStatus? status)
        {
            if(status.HasValue && status == DocumentReferenceStatus.Current)
            {
                return status.Value;
            }

            return null;
        }

        public OperationOutcome ValidateOrganisationReference(ResourceReference reference, string type)
        {
            if (!_validationHelper.ValidReference(reference, FhirConstants.SystemODS))
            {
                return OperationOutcomeFactory.CreateInvalidParameter("Invalid parameter", $"The given resource URL does not conform to the expected format - {FhirConstants.SystemODS}/[ODS Code]");
            }

            var orgCode = GetOrganizationReferenceId(reference);

            if (string.IsNullOrWhiteSpace(orgCode))
            {
                return OperationOutcomeFactory.CreateInvalidResource(type);
            }
            

            return null;
        }

        public OperationOutcome ValidateContent(List<DocumentReference.ContentComponent> contents)
        { 
            foreach (var content in contents)
            {
                //attachment.contentType
                //TODO validate contenttype format
                var contentType = content.Attachment.ContentType;
                if (string.IsNullOrEmpty(contentType))
                {
                    return OperationOutcomeFactory.CreateInvalidResource("contenttype");
                }

                //attachment.url
                var url = content.Attachment.Url;
                if (string.IsNullOrEmpty(url) || !FhirUri.IsValidValue(url))
                {
                    return OperationOutcomeFactory.CreateInvalidResource("url");
                }
            }

            return null;
        }

    }
}
