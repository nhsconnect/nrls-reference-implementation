using Hl7.Fhir.Model;
using Hl7.Fhir.Specification.Source;
using Hl7.Fhir.Validation;

namespace NRLS_API.Core.Interfaces.Services
{
    public interface IValidationHelper
    {
        //IResourceResolver Resolver { get; }

        Validator Validator { get; }

        bool ValidCodableConcept(CodeableConcept concept, string validSystem, bool validateFromSet = false, bool systemRequired = true, bool codeRequired = true, bool displayRequired = true);

        bool ValidReference(ResourceReference reference, string startsWith);

        bool ValidNhsNumber(string nhsNumber);
    }
}
