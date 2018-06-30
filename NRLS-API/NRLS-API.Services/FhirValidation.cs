using Hl7.Fhir.Model;
using Hl7.Fhir.Validation;
using NRLS_API.Core.Factories;
using NRLS_API.Core.Helpers;
using NRLS_API.Core.Interfaces.Services;
using NRLS_API.Core.Resources;
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
            //status
            if(pointer.Status == null)
            {
                return OperationOutcomeFactory.CreateInvalidResource(null);
            }
            else if (!pointer.Status.HasValue || !EnumHelpers.IsValidName<DocumentReferenceStatus>(pointer.Status.ToString()))
            {
                return OperationOutcomeFactory.CreateInvalidResource("status");
            }

            //type
            if(pointer.Type == null)
            {
                return OperationOutcomeFactory.CreateInvalidResource(null);
            }
            else if(!_validationHelper.ValidCodableConcept(pointer.Type, FhirConstants.SystemPointerType, true, true, true, true, FhirConstants.VsRecordType))
            {
                return OperationOutcomeFactory.CreateInvalidResource("type");
            }

            //subject
            if (pointer.Subject != null)
            {
                var validNhsNumber =  ValidatePatientReference(pointer.Subject);

                if(validNhsNumber != null)
                {
                    return validNhsNumber;
                }
            }
            else
            {
                return OperationOutcomeFactory.CreateInvalidResource(null);
            }

            //validate orgcode is real done outside of here
            //author
            if (pointer.Author != null && pointer.Author.Count == 1)
            {
                var validAuthor = ValidateOrganisationReference(pointer.Author.First(), "author");

                if (validAuthor != null)
                {
                    return validAuthor;
                }
            }
            else
            {
                return OperationOutcomeFactory.CreateInvalidResource(null);
            }


            //validate orgcode for match against fromASID and is real done outside of here
            //custodian
            if (pointer.Custodian != null)
            {
                var validCustodian = ValidateOrganisationReference(pointer.Custodian, "custodian");

                if (validCustodian != null)
                {
                    return validCustodian;
                }
            }
            else
            {
                return OperationOutcomeFactory.CreateInvalidResource(null);
            }

            //indexed
            if (pointer.Indexed == null)
            {
                return OperationOutcomeFactory.CreateInvalidResource(null);
            }
            else if (!pointer.Indexed.HasValue || !FhirDateTime.IsValidValue(pointer.Indexed.Value.ToString(DateTimeFormat)))
            {
                return OperationOutcomeFactory.CreateInvalidResource("indexed");
            }

            //attachment
            if (pointer.Content != null)
            {
                var validContent = ValidateContent(pointer.Content);

                if(validContent != null)
                {
                    return validContent;
                }
            }
            else
            {
                return OperationOutcomeFactory.CreateInvalidResource(null);
            }

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


            if (!_validationHelper.ValidCodableConcept(concept, FhirConstants.SystemPointerType, true, false, true, false, FhirConstants.VsRecordType))
            {
                return OperationOutcomeFactory.CreateInvalidParameter("Invalid parameter", $"The given code is not from the expected terminology system - {FhirConstants.SystemPointerType}");
            }

            return null;
        }

        public OperationOutcome ValidatePatientReference(ResourceReference reference)
        {
            if (!_validationHelper.ValidReference(reference, FhirConstants.SystemPDS))
            {
                return OperationOutcomeFactory.CreateInvalidParameter("Invalid parameter", $"The given resource URL does not conform to the expected format - {FhirConstants.SystemPDS}[NHS Number]");
            }

            var nhsNumber = reference.Reference.Replace(FhirConstants.SystemPDS, "");

            if (!_validationHelper.ValidNhsNumber(nhsNumber))
            {
                return OperationOutcomeFactory.CreateInvalidNhsNumberRes(nhsNumber);
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

            var orgCode = parameterVal.Replace(FhirConstants.SystemODS, "");

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

        public string GetOrganizationReferenceId(ResourceReference reference)
        {
            return _validationHelper.GetResourceReferenceId(reference, FhirConstants.SystemODS);
        }

        public OperationOutcome ValidateOrganisationReference(ResourceReference reference, string type)
        {
            if (!_validationHelper.ValidReference(reference, FhirConstants.SystemODS))
            {
                return OperationOutcomeFactory.CreateInvalidParameter("Invalid parameter", $"The given resource URL does not conform to the expected format - {FhirConstants.SystemODS}/[ODS Code]");
            }

            var orgCode = reference.Reference.Replace(FhirConstants.SystemODS, "");

            if (string.IsNullOrWhiteSpace(orgCode))
            {
                return OperationOutcomeFactory.CreateInvalidResource(type);
            }
            

            return null;
        }

        public OperationOutcome ValidateContent(List<DocumentReference.ContentComponent> contents)
        {
            if (contents == null || contents.Count == 0)
            {
                return OperationOutcomeFactory.CreateInvalidResource("content");
            }

            foreach (var content in contents)
            {
                //attachment
                if(content.Attachment == null)
                {
                    return OperationOutcomeFactory.CreateInvalidResource("attachment");
                }

                //attachment.contentType
                //TODO validate content type format
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

                //attachment.creation can be empty
                var creation = content.Attachment.Creation;
                if (!string.IsNullOrEmpty(creation) && !FhirDateTime.IsValidValue(creation))
                {
                    return OperationOutcomeFactory.CreateInvalidResource("creation", $"The attachment creation date value is not a valid dateTime type: {creation}.");
                }
            }

            return null;
        }

    }
}
