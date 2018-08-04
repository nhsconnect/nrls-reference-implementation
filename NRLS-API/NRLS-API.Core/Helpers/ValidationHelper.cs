using Hl7.Fhir.Model;
using Hl7.Fhir.Specification.Source;
using Hl7.Fhir.Validation;
using NRLS_API.Core.Interfaces.Helpers;
using NRLS_API.Core.Interfaces.Services;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace NRLS_API.Core.Helpers
{
    public class ValidationHelper : IValidationHelper
    {
        public Validator Validator { get; }

        private IResourceResolver _source { get; }

        private readonly IFhirCacheHelper _fhirCacheHelper;

        public ValidationHelper(IFhirCacheHelper fhirCacheHelper)
        {
            _fhirCacheHelper = fhirCacheHelper;

            _source = _fhirCacheHelper.GetSource();

            var ctx = new ValidationSettings()
            {
                ResourceResolver = _source,
                GenerateSnapshot = true,
                EnableXsdValidation = false,
                Trace = false,
                ResolveExteralReferences = true
            };


            Validator = new Validator(ctx);
        }

        public bool ValidCodableConcept(CodeableConcept concept, string validSystem, bool validateFromSet, bool systemRequired, bool codeRequired, bool displayRequired, string valueSet)
        {
            if(concept == null || concept.Coding.Count != 1)
            {
                return false;
            }

            var coding = concept.Coding.ElementAt(0);

            if(systemRequired && (string.IsNullOrEmpty(coding.System) || !coding.System.Equals(validSystem)))
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

            // TODO : parse and validate code from valueset
            // only available code is 736253002
            if (validateFromSet && !string.IsNullOrWhiteSpace(valueSet))
            {
                var values = GetCodableConceptValueSet(valueSet);

                return values?.Compose?.Include?.FirstOrDefault(x => x.System == validSystem)?.Concept?.FirstOrDefault(x => x.Code == coding.Code) != null;
            }

            return true;
        }

        public string GetResourceReferenceId(ResourceReference reference, string systemUrl)
        {
            if (string.IsNullOrEmpty(systemUrl))
            {
                return reference?.Reference;
            }

            return reference?.Reference?.Replace(systemUrl, "");
        }

        public bool ValidReference(ResourceReference reference, string startsWith)
        {
            return reference != null && !string.IsNullOrEmpty(reference.Reference) && !string.IsNullOrEmpty(startsWith) && reference.Reference.StartsWith(startsWith);
        }

        public bool ValidIdentifier(Identifier identifier)
        {
            return identifier != null && !string.IsNullOrWhiteSpace(identifier.System) && Uri.IsWellFormedUriString(identifier.System, UriKind.RelativeOrAbsolute) && !string.IsNullOrWhiteSpace(identifier.Value);
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

        private ValueSet GetCodableConceptValueSet(string systemUrl)
        {
            ValueSet values = _fhirCacheHelper.GetValueSet(systemUrl);

            return values;
        }
    }
}
