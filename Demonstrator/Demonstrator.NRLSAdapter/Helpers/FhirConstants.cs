namespace Demonstrator.NRLSAdapter.Helpers
{
    public class FhirConstants
    {
        public const string SystemNhsNumber = "https://fhir.nhs.uk/Id/nhs-number";
        public const string SystemOrgCode = "https://fhir.nhs.uk/Id/ods-organization-code";
        public const string SystemPDS = "https://demographics.spineservices.nhs.uk/STU3/Patient/";
        public const string SystemODS = "https://directory.spineservices.nhs.uk/STU3/Organization/";
        //public const string SystemPDS = "Patient/";

        public const string CodingSystemPointerType = "https://fhir.nhs.uk/STU3/ValueSet/CarePlanType-1";

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
