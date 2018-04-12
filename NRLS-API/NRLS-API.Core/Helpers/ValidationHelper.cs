using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Specification.Source;
using Hl7.Fhir.Validation;
using NRLS_API.Core.Interfaces.Services;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace NRLS_API.Core.Helpers
{
    public class ValidationHelper : IValidationHelper
    {
        public Validator Validator { get; }

        private IResourceResolver _source { get; }

        public ValidationHelper()
        {

            var basePath = DirectoryHelper.GetBaseDirectory();

            var zip = Path.Combine(basePath, "Data\\definitions.xml.zip");

            _source = new CachedResolver(new MultiResolver(new WebResolver(uri => new FhirClient("https://fhir.nhs.uk/STU3")), new ZipSource(zip)));

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

        public bool ValidCodableConcept(CodeableConcept concept, string validSystem, bool validateFromSet, bool systemRequired, bool codeRequired, bool displayRequired)
        {
            if(concept == null || concept.Coding.Count != 1)
            {
                return false;
            }

            var coding = concept.Coding.ElementAt(0);

            if(systemRequired && string.IsNullOrEmpty(coding.System)) // || !coding.System.Equals(validSystem)
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

            // TODO
            //if (validateFromSet)
            //{
            //    var values = GetCodableConceptValueSet(validSystem);
            //
            //    if(values != null)
            //    {
            //      
            //    }
            //}

            return true;
        }

        public bool ValidReference(ResourceReference reference, string startsWith)
        {
            return !string.IsNullOrEmpty(reference.Reference) && !string.IsNullOrEmpty(startsWith) && reference.Reference.StartsWith(startsWith);
        }

        public bool ValidNhsNumber(string nhsNumber)
        {
            if (string.IsNullOrEmpty(nhsNumber))
            {
                return false;
            }

            int nhsNumberLength = 10;
            nhsNumber = nhsNumber.Trim();

            if (nhsNumber.Length != nhsNumberLength || !Regex.Match(nhsNumber, "(\\d+)").Success)
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

        private ValueSet GetCodableConceptValueSet(string systemUrl)
        {
            ValueSet values = _source.FindValueSet(systemUrl);

            return values;
        }
    }
}
