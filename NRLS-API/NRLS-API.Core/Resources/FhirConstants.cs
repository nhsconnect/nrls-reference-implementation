﻿namespace NRLS_API.Core.Resources
{
    public class FhirConstants
    {
        public const string SystemNrlsProfile = "https://fhir.nhs.uk/STU3/StructureDefinition/NRL-DocumentReference-1";

        public const string SDSpineOpOutcome = "https://fhir.nhs.uk/STU3/StructureDefinition/Spine-OperationOutcome-1";

        public const string SDSpineOpOutcome1 = "https://fhir.nhs.uk/StructureDefinition/spine-operationoutcome-1-0";

        public const string SystemNhsNumber = "https://fhir.nhs.uk/Id/nhs-number";

        public const string SystemOrgCode = "https://fhir.nhs.uk/Id/ods-organization-code";

        public const string SystemODS = "https://directory.spineservices.nhs.uk/STU3/Organization/";

        public const string SystemPDS = "https://demographics.spineservices.nhs.uk/STU3/Patient/";

        public const string SystemASID = "https://fhir.nhs.uk/Id/accredited-system";

        public const string SystemSdsRole = "https://fhir.nhs.uk/Id/sds-role-profile-id";

        public const string SystemOpOutcome = "https://fhir.nhs.uk/STU3/CodeSystem/Spine-ErrorOrWarningCode-1";

        public const string SystemOpOutcome1 = "https://fhir.nhs.uk/STU3/ValueSet/spine-response-code-1-0";

        public const string SystemPointerType = "http://snomed.info/sct";

        public const string SystemPointerClass = "http://snomed.info/sct";

        public const string SystemPointerFormat = "https://fhir.nhs.uk/STU3/CodeSystem/NRL-FormatCode-1";

        //ValueSets
        public const string VsRecordType = "https://fhir.nhs.uk/STU3/ValueSet/NRL-RecordType-1";

        public const string VsRecordClass = "https://fhir.nhs.uk/STU3/ValueSet/NRL-RecordClass-1";

        public const string VsRecordFormat = "https://fhir.nhs.uk/STU3/ValueSet/NRL-FormatCode-1";

        //Headers
        public const string HeaderFromAsid = "fromASID";

        public const string HeaderToAsid = "toASID";

        public const string HeaderSspInterationId = "Ssp-InteractionID";

        public const string HeaderFSspVersion = "Ssp-Version";

        public const string HeaderSspTradeId = "Ssp-TraceID";

        public const string HeaderSspFromAsid = "Ssp-From";

        public const string HeaderSspToAsid = "Ssp-To";

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
        public const string BaseInteractionId = "urn:nhs:names:services:~service~:~resourceOrOperation~.~interaction~";

        public static string ReadInteractionId => GenerateInteraction("read", "DocumentReferenceRead", "nrl");

        public static string SearchInteractionId => GenerateInteraction("read", "DocumentReference", "nrl");

        public static string CreateInteractionId => GenerateInteraction("write", "DocumentReference", "nrl");

        public static string UpdateInteractionId => GenerateInteraction("write", "DocumentReferencePatch", "nrl");

        public static string DeleteInteractionId => GenerateInteraction("write", "DocumentReference", "nrl");

        public static string ReadBinaryInteractionId => GenerateInteraction("read", "SspRetrieval", "nrl");

        private static string GenerateInteraction(string interaction, string resource, string service, bool isFhirRest = true)
        {
            return BaseInteractionId.Replace("~interaction~", interaction).Replace("~resourceOrOperation~", resource).Replace("~service~", service);
        }
    }
}
