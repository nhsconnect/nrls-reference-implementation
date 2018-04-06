namespace NRLS_API.Core.Resources
{
    public class FhirConstants
    {
        public const string SDSpineOpOutcome = "https://fhir.nhs.uk/STU3/StructureDefinition/Spine-OperationOutcome-1";

        public const string SystemNhsNumber = "https://fhir.nhs.uk/Id/nhs-number";
        public const string SystemOrgCode = "https://fhir.nhs.uk/Id/ods-organization-code";
        public const string SystemPDS = "https://demographics.spineservices.nhs.uk/STU3/Patient/";
        public const string SystemOpOutcome = "https://fhir.nhs.uk/STU3/ValueSet/Spine-ErrorOrWarningCode-1";

        public const string BaseInteractionId = "urn:nhs:names:services:nrls:fhir:rest:~interaction~:documentreference";

        public static string ReadInteractionId => GenerateInteraction("read");

        public static string SearchInteractionId => GenerateInteraction("search");

        public static string CreateInteractionId => GenerateInteraction("create");

        public static string UpdateInteractionId => GenerateInteraction("update");

        public static string DeleteInteractionId => GenerateInteraction("delete");


        private static string GenerateInteraction(string interaction)
        {
            return BaseInteractionId.Replace("~interaction~", interaction);
        }
    }
}
