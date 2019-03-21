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

        public const string SystemType = "http://snomed.info/sct";


        //ValueSets
        //public const string CodingSystemPointerType = "https://fhir.nhs.uk/STU3/ValueSet/CarePlanType-1";
        public const string CodingSystemPointerType = "http://snomed.info/sct";
        

        //Headers
        public const string HeaderFromAsid = "fromASID";

        public const string HeaderToAsid = "toASID";

        public const string HeaderSspInterationId = "Ssp-InteractionID";

        public const string HeaderFSspVersion = "Ssp-Version";

        public const string HeaderSspTraceId = "Ssp-TraceID";

        public const string HeaderSspTo = "Ssp-To";

        public const string HeaderSspFrom = "Ssp-From";


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
        public const string BaseInteractionId = "urn:nhs:names:services:~service~:fhir:rest:~interaction~:~resource~";

        public static string ReadInteractionId => GenerateInteraction("read", "documentreference", "nrls");

        public static string SearchInteractionId => GenerateInteraction("search", "documentreference", "nrls");

        public static string CreateInteractionId => GenerateInteraction("create", "documentreference", "nrls");

        public static string UpdateInteractionId => GenerateInteraction("update", "documentreference", "nrls");

        public static string DeleteInteractionId => GenerateInteraction("delete", "documentreference", "nrls");

        public static string ReadBinaryInteractionId => GenerateInteraction("read", "binary", "nrls");

        private static string GenerateInteraction(string interaction, string resource, string service)
        {
            return BaseInteractionId.Replace("~interaction~", interaction).Replace("~resource~", resource).Replace("~service~", service);
        }
    }
}
