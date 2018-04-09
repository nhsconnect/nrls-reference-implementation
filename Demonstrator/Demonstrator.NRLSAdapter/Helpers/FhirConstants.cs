namespace Demonstrator.NRLSAdapter.Helpers
{
    public class FhirConstants
    {
        //Ids
        public const string IdsNhsNumber = "https://fhir.nhs.uk/Id/nhs-number";

        public const string IdsOrgCode = "https://fhir.nhs.uk/Id/ods-organization-code";

        public const string IdsSdsRolePofileId = "https://fhir.nhs.uk/Id/sds-role-profile-id";

        public const string IdsAccrSystem = "https://fhir.nhs.uk/Id/accredited-system";


        //Systems
        public const string SystemPDS = "https://demographics.spineservices.nhs.uk/STU3/Patient/";

        public const string SystemODS = "https://directory.spineservices.nhs.uk/STU3/Organization/";


        //ValueSets
        public const string CodingSystemPointerType = "https://fhir.nhs.uk/STU3/ValueSet/CarePlanType-1";


        //Headers
        public const string HeaderFromAsid = "fromASID";

        public const string HeaderToAsid = "toASID";

        public const string HeaderSspInterationId = "Ssp-InteractionID";

        public const string HeaderFSspVersion = "Ssp-Version";

        public const string HeaderSspTradeId = "Ssp-TraceID";


        //JWT
        public const string JwtClientSysIssuer = "iss";

        public const string JwtIndOrSysIdentifier = "sub";

        public const string JwtEndpointUrl = "aud";

        public const string JwtExpiration = "exp";

        public const string JwtIssued = "iat";

        public const string JwtReasonForRequest = "reason_for_request";

        public const string JwtScope = "scope";

        public const string JwtRequestingSystem = "requesting_system";

        public const string JwtRequestingOrganization = "requesting_organization";

        public const string JwtRequestingUser = "requesting_user";


        //Interactions
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
