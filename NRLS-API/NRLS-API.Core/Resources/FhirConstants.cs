using System;
using System.Collections.Generic;
using System.Text;

namespace NRLS_API.Core.Resources
{
    public class FhirConstants
    {
        public const string SDSpineOpOutcome = "https://fhir.nhs.uk/STU3/StructureDefinition/Spine-OperationOutcome-1";

        public const string SystemNhsNumber = "https://fhir.nhs.uk/Id/nhs-number";
        public const string SystemOrgCode = "https://fhir.nhs.uk/Id/ods-organization-code";
        public const string SystemPDS = "https://demographics.spineservices.nhs.uk/STU3/Patient/";
        public const string SystemOpOutcome = "https://fhir.nhs.uk/STU3/ValueSet/Spine-ErrorOrWarningCode-1";
    }
}
