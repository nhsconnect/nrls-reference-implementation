using Hl7.Fhir.Model;
using NRLS_API.Core.Resources;
using NRLS_API.Models.Core;
using System;
using System.Collections.Generic;

namespace NRLS_API.Core.Helpers
{
    public class NrlsPointerHelper
    {

        public static FhirRequest CreateOrgSearch(FhirRequest request, string orgCode)
        {
            if (!string.IsNullOrWhiteSpace(orgCode))
            {
                var queryParameters = new List<Tuple<string, string>>
                {
                    new Tuple<string, string>("identifier", $"{FhirConstants.SystemOrgCode}|{orgCode}")
                };

                return  FhirRequest.Copy(request, ResourceType.Organization, null, queryParameters, request.ProfileUri);

            }

            return null;
        }

        public static FhirRequest CreateMasterIdentifierSearch(FhirRequest request, Identifier masterId, string nhsNumber)
        {
            if (masterId != null && !string.IsNullOrWhiteSpace(nhsNumber))
            {
                var queryParameters = new List<Tuple<string, string>>
                {
                    new Tuple<string, string>("identifier", $"{masterId.System}|{masterId.Value}"),
                    new Tuple<string, string>("subject", $"{FhirConstants.SystemPDS}{nhsNumber}")
                };

                return FhirRequest.Copy(request, ResourceType.DocumentReference, null, queryParameters, request.ProfileUri);
            }

            return null;
        }
    }
}
