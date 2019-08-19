using Hl7.Fhir.Model;
using System.Collections.Generic;

namespace NRLS_API.Core.Interfaces.Services
{
    public interface IValidationHelper
    {
        OperationOutcome ValidateResource<T>(T resource, string resourceSchema) where T : Resource;

        bool ValidCodableConcept(CodeableConcept concept, int maxCodings, string validSystem, bool validateFromSet = false, bool systemRequired = true, bool codeRequired = true, bool displayRequired = true, string valueSet = null);

        bool ValidCoding(List<Coding> codings, int maxCodings, string validSystem, bool validateFromSet, bool systemRequired, bool codeRequired, bool displayRequired, string valueSet);

        bool ValidReference(ResourceReference reference, string startsWith);

        (bool valid, string issue) ValidIdentifier(Identifier identifier, string name);

        string GetResourceReferenceId(ResourceReference reference, string systemUrl);

        bool ValidTokenParameter(string parameterVal, string expectedSystemPrefix = null, bool allowOptionalSystemOrValue = true);

        bool ValidReferenceParameter(string parameterVal, string systemPrefix);

        string GetOrganisationParameterIdentifierId(string parameterVal);

        string GetOrganisationParameterId(string parameterVal);

        string GetReferenceParameterId(string system, string parameterVal);

        string GetTokenParameterId(string parameterVal, string systemPrefix);

        bool ValidNhsNumber(string nhsNumber);
    }
}
