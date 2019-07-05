using Hl7.Fhir.Model;
using NRLS_API.Core.Interfaces.Helpers;
using NRLS_API.Core.Interfaces.Services;
using NRLS_API.Core.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace NRLS_API.Core.Helpers
{
    public class ValidationHelper : IValidationHelper
    {
        private readonly IFhirResourceHelper _fhirResourceHelper;

        public ValidationHelper(IFhirResourceHelper fhirResourceHelper)
        {
            _fhirResourceHelper = fhirResourceHelper;
        }

        public OperationOutcome ValidateResource<T>(T resource, string resourceSchema) where T : Resource
        {
            return _fhirResourceHelper.ValidateResource(resource, resourceSchema);
        }

        public bool ValidCodableConcept(CodeableConcept concept, int maxCodings, string validSystem, bool validateFromSet, bool systemRequired, bool codeRequired, bool displayRequired, string valueSet)
        {
            if (concept == null)
            {
                return false;
            }

            return ValidCoding(concept.Coding, maxCodings, validSystem, validateFromSet, systemRequired, codeRequired, displayRequired, valueSet);
        }

        public bool ValidCoding(List<Coding> codings, int maxCodings, string validSystem, bool validateFromSet, bool systemRequired, bool codeRequired, bool displayRequired, string valueSet)
        {
            if (codings.Count != maxCodings)
            {
                return false;
            }

            ValueSet values = null;
            // TODO : parse and validate code from valueset
            // only available code is 736253002
            // not currently checking display but this should be validated
            if (validateFromSet && !string.IsNullOrWhiteSpace(valueSet))
            {
                values = GetCodableConceptValueSet(valueSet);
            }

            foreach (var coding in codings)
            {
                if (systemRequired && (string.IsNullOrEmpty(coding.System) || !coding.System.Equals(validSystem)))
                {
                    return false;
                }

                if (codeRequired && string.IsNullOrEmpty(coding.Code))
                {
                    return false;
                }

                if (displayRequired && string.IsNullOrEmpty(coding.Display))
                {
                    return false;
                }

                if (validateFromSet && !string.IsNullOrWhiteSpace(valueSet))
                {
                    return values?.Compose?.Include?.FirstOrDefault(x => x.System == validSystem)?.Concept?.FirstOrDefault(x => x.Code == coding.Code) != null;
                }
            }

            return true;
        }

        public string GetResourceReferenceId(ResourceReference reference, string systemUrl)
        {
            if (string.IsNullOrEmpty(systemUrl))
            {
                return reference?.Reference;
            }

            return reference?.Reference?.Replace(systemUrl, "").Trim();
        }

        public bool ValidReference(ResourceReference reference, string startsWith)
        {
            return reference != null && !string.IsNullOrEmpty(reference.Reference) && !string.IsNullOrEmpty(startsWith) && reference.Reference.StartsWith(startsWith);
        }

        public (bool valid, string issue) ValidIdentifier(Identifier identifier, string name)
        {
            if (identifier != null)
            {
                if (string.IsNullOrWhiteSpace(identifier.System) || !Uri.IsWellFormedUriString(identifier.System, UriKind.RelativeOrAbsolute))
                {
                    return (false, $"{name}.system");
                }

                if(string.IsNullOrWhiteSpace(identifier.Value))
                {
                    return (false, $"{name}.value");
                }

                return (true, null);
            }

            return (false, $"{name}");
        }

        public bool ValidNhsNumber(string nhsNumber)
        {
            if (string.IsNullOrEmpty(nhsNumber))
            {
                return false;
            }

            int nhsNumberLength = 10;
            nhsNumber = nhsNumber.Trim();

            nhsNumber = Regex.Replace(nhsNumber, "([^\\d]+)", "");

            if (nhsNumber.Length != nhsNumberLength)
            {
                return false;
            }


            string checkDigit = nhsNumber.Substring(nhsNumberLength - 1, 1);
            int checkNumber = Convert.ToInt16(checkDigit);

            var multiplers = new int[9];
            multiplers[0] = 10;
            multiplers[1] = 9;
            multiplers[2] = 8;
            multiplers[3] = 7;
            multiplers[4] = 6;
            multiplers[5] = 5;
            multiplers[6] = 4;
            multiplers[7] = 3;
            multiplers[8] = 2;

            int currentNumber = 0;
            int currentSum = 0;

            for (int i = 0; i < 9; i++)
            {
                currentNumber = Convert.ToInt16(nhsNumber.Substring(i, 1));
                currentSum = currentSum + (currentNumber * multiplers[i]);
            }

            int remainder = currentSum % 11;
            int total = 11 - remainder;

            if (total.Equals(11))
            {
                total = 0;
            }

            return total.Equals(checkNumber);
            

        }

        public bool ValidReferenceParameter(string parameterVal, string systemPrefix)
        {
            return (!string.IsNullOrEmpty(parameterVal) && parameterVal.StartsWith(systemPrefix));
        }

        public bool ValidTokenParameter(string parameterVal, string expectedSystemPrefix = null, bool allowOptionalSystemOrValue = true)
        {
            if(string.IsNullOrWhiteSpace(parameterVal))
            {
                return false;
            }

            var systemAndValue = parameterVal.Split('|');

            if (allowOptionalSystemOrValue)
            {

                if(systemAndValue.Count() < 1 || systemAndValue.Count() > 2)
                {
                    return false;
                }

                if(!string.IsNullOrEmpty(expectedSystemPrefix) && !parameterVal.StartsWith($"{expectedSystemPrefix}|"))
                {
                    return false;
                }

                if (systemAndValue.Count() > 1 && ((!string.IsNullOrWhiteSpace(systemAndValue.ElementAt(0)) && !Uri.IsWellFormedUriString(systemAndValue.ElementAt(0), UriKind.RelativeOrAbsolute))))
                {
                    return false;
                }

                return true;
            }

            if (systemAndValue.Count() != 2 || string.IsNullOrWhiteSpace(systemAndValue.ElementAt(0)) || !Uri.IsWellFormedUriString(systemAndValue.ElementAt(0), UriKind.RelativeOrAbsolute) || string.IsNullOrWhiteSpace(systemAndValue.ElementAt(1)))
            {
                return false;
            }

            return string.IsNullOrEmpty(expectedSystemPrefix) || parameterVal.StartsWith($"{expectedSystemPrefix}|");
        }

        public string GetTokenParameterId(string parameterVal, string systemPrefix)
        {
            return !string.IsNullOrEmpty(parameterVal) ? parameterVal.Replace($"{systemPrefix}|", "") : null ;
        }

        public string GetOrganisationParameterIdentifierId(string parameterVal)
        {
            return !string.IsNullOrEmpty(parameterVal) && parameterVal.StartsWith($"{FhirConstants.SystemOrgCode}|") ? parameterVal.Replace($"{FhirConstants.SystemOrgCode}|", "") : null;
        }

        public string GetOrganisationParameterId(string parameterVal)
        {
            return !string.IsNullOrEmpty(parameterVal) && parameterVal.StartsWith(FhirConstants.SystemODS) ? parameterVal.Replace(FhirConstants.SystemODS, "") : null;
        }

        private ValueSet GetCodableConceptValueSet(string systemUrl)
        {
            ValueSet values = _fhirResourceHelper.GetValueSet(systemUrl);

            return values;
        }
    }
}
