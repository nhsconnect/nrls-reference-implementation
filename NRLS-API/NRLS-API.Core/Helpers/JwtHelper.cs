using System;
using System.IdentityModel.Tokens.Jwt;
using NRLS_API.Core.Resources;
using System.Linq;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;
using NRLS_API.Models.Core;
using Microsoft.Extensions.Caching.Memory;
using NRLS_API.Core.Enums;
using NRLS_API.Core.Interfaces.Helpers;

namespace NRLS_API.Core.Helpers
{
    public class JwtHelper : IJwtHelper
    {
        private IMemoryCache _cache;

        private string[] _validClaims = { "iss", "sub", "aud", "exp", "iat", "reason_for_request", "scope", "requesting_system", "requesting_organization", "requesting_user" };

        private string[] _validScopes = { "patient/DocumentReference.read", "patient/DocumentReference.write" };

    public JwtHelper(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }

        public Response IsValid(string jwt, JwtScopes reqScope, DateTime? tokenIssued = null)
        {
            var now = tokenIssued ?? DateTime.UtcNow;

            if (string.IsNullOrEmpty(jwt))
            {
                return new Response("The Authorisation header must be supplied");
            }


            if(jwt.StartsWith("Bearer "))
            {
                jwt = jwt.Replace("Bearer ", "");
            }

            #region Base JWT Checks

            //This should be a basic Base64UrlEncoded token
            var claimsHashItems = jwt.Split('.');

            if (claimsHashItems.Count() != 3)
            {
                return new Response("The JWT associated with the Authorisation header must have the 3 sections");
            }

            var signature = claimsHashItems.Skip(2).Take(1).FirstOrDefault();

            if (!string.IsNullOrEmpty(signature))
            {
                return new Response("The JWT associated with the Authorisation header must have the 3 sections");
            }

            var claimsHash = claimsHashItems.Skip(1).Take(1).FirstOrDefault();

            if (string.IsNullOrEmpty(claimsHash))
            {
                return new Response("The JWT associated with the Authorisation header must have the 3 sections");
            }

            claimsHash = claimsHash.Replace('-', '+').Replace('_', '/');

            var decoded = DecodePart(claimsHash);

            IDictionary<string, string> claims;

            try
            {
                claims = JsonConvert.DeserializeObject<IDictionary<string, string>>(decoded);
            }
            catch (JsonReaderException ex)
            {
                var errorMessage = "The Authorisation header must be supplied";

                if (!string.IsNullOrWhiteSpace(ex.Path) && _validClaims.Contains(ex.Path))
                {
                    errorMessage = BaseErrorMessage(ex.Path);
                }

                return new Response(errorMessage);
            }
            catch (Exception ex)
            {
                return new Response("The Authorisation header must be supplied");
            }

            #endregion

            // ### iss
            var iss = claims.FirstOrDefault(x => x.Key.Equals(FhirConstants.JwtClientSysIssuer));

            if (string.IsNullOrWhiteSpace(iss.Value))
            {
                return new Response(BaseErrorMessage(FhirConstants.JwtClientSysIssuer));
            }

            // ### aud
            var aud = claims.FirstOrDefault(x => x.Key.Equals(FhirConstants.JwtEndpointUrl));

            if (string.IsNullOrWhiteSpace(aud.Value))
            {
                return new Response(BaseErrorMessage(FhirConstants.JwtEndpointUrl));
            }

            // ### iat basic
            var iat = claims.FirstOrDefault(x => x.Key.Equals(FhirConstants.JwtIssued));
            long iatVal;

            if (!long.TryParse(iat.Value, out iatVal))
            {
                return new Response(BaseErrorMessage(FhirConstants.JwtIssued));
            }

            // ### exp basic
            var exp = claims.FirstOrDefault(x => x.Key.Equals(FhirConstants.JwtExpiration));
            long expVal;

            if (!long.TryParse(exp.Value, out expVal))
            {
                return new Response(BaseErrorMessage(FhirConstants.JwtExpiration));
            }

            // ### iat and exp checks

            //Temporarily turn off as NRLS API does not validate these
            var issuedDt = EpochTime.DateTime(iatVal);
            var expireDt = EpochTime.DateTime(expVal);

            //if (issuedDt.AddMinutes(5) != expireDt )
            //{
            //    return new Response($"exp {exp.Value} must be 5 minutes greater than iat {iat.Value}");
            //}

            //if (issuedDt > now || expireDt < now)
            //{
            //    return new Response($"iat {iat.Value} must be in the past or now and exp {exp.Value} must be in the future or now");
            //}


            // ### sub

            var sub = claims.FirstOrDefault(x => x.Key.Equals(FhirConstants.JwtIndOrSysIdentifier));

            if (string.IsNullOrEmpty(sub.Value))
            {
                return new Response(BaseErrorMessage(FhirConstants.JwtIndOrSysIdentifier));
            }


            // ### requesting_user
            var userRequired = sub.Value.StartsWith($"{FhirConstants.SystemSdsRole}|");

            var reqUsr = claims.FirstOrDefault(x => x.Key.Equals(FhirConstants.JwtRequestingUser));
            var reqSys = claims.FirstOrDefault(x => x.Key.Equals(FhirConstants.JwtRequestingSystem));

            if (userRequired && string.IsNullOrEmpty(reqUsr.Value))
            {
                return new Response(BaseErrorMessage(FhirConstants.JwtRequestingUser));
            }

            // check reqUser matches sub if reqUsr was supplied, i.e. if this is a user type request rather than an system type request
            if (!string.IsNullOrEmpty(reqUsr.Value) && reqUsr.Value != sub.Value)
            {
                return new Response($"requesting_user ({reqUsr.Value}) and sub ({sub.Value}) claim's values must match");
            }

            var userRoleProfileId = reqUsr.Value?.Replace($"{FhirConstants.SystemSdsRole}|", "");

            if (!string.IsNullOrEmpty(reqUsr.Value) && (!reqUsr.Value.StartsWith(FhirConstants.SystemSdsRole) || string.IsNullOrWhiteSpace(userRoleProfileId)))
            {
                return new Response($"requesting_user ({reqSys.Value}) must be of the form [{FhirConstants.SystemSdsRole}|[SDSRoleProfileID]]");
            }

            // ### requesting_system
            if (string.IsNullOrEmpty(reqSys.Value))
            {
                return new Response(BaseErrorMessage(FhirConstants.JwtRequestingSystem));
            }

            // check reqSys matches sub if this is a system type
            if (string.IsNullOrEmpty(reqUsr.Value) && reqSys.Value != sub.Value)
            {
                return new Response($"requesting_system ({reqSys.Value}) and sub ({sub.Value}) claim's values must match");
            }

            var fromAsid = reqSys.Value.Replace($"{FhirConstants.SystemASID}|", "");
            if (!reqSys.Value.StartsWith(FhirConstants.SystemASID) || string.IsNullOrWhiteSpace(fromAsid))
            {
                return new Response($"requesting_system ({reqSys.Value}) must be of the form [{FhirConstants.SystemASID}|[ASID]]");
            }

            var fromAsidMap = GetFromAsidMap(fromAsid);

            if (fromAsidMap == null)
            {
                return new Response($"The ASID defined in the requesting_system ({fromAsid}) is unknown");
            }

            // ### reason_for_request
            var resForReq = claims.FirstOrDefault(x => x.Key.Equals(FhirConstants.JwtReasonForRequest));

            if (string.IsNullOrWhiteSpace(resForReq.Value))
            {
                return new Response(BaseErrorMessage(FhirConstants.JwtReasonForRequest));
            }
            else if (resForReq.Value != "directcare")
            {
                return new Response($"reason_for_request ({resForReq.Value}) must be 'directcare'");
            }

            // ### scope
            var scope = claims.FirstOrDefault(x => x.Key.Equals(FhirConstants.JwtScope));
            var expScope = $"patient/DocumentReference.{reqScope.ToString().ToLowerInvariant()}";

            if (string.IsNullOrWhiteSpace(scope.Value))
            {
                return new Response(BaseErrorMessage(FhirConstants.JwtScope));
            }
            else if (scope.Value != expScope) // || !_validScopes.Contains(scope.Value)
            {
                //currently 
                return new Response($"scope ({scope.Value}) must match either 'patient/DocumentReference.read' or 'patient/DocumentReference.write'");
            }


            // ### requesting_organization
            var reqOrg = claims.FirstOrDefault(x => x.Key.Equals(FhirConstants.JwtRequestingOrganization));
            var orgCode = reqOrg.Value?.Replace($"{FhirConstants.SystemOrgCode}|", "");

            if (string.IsNullOrWhiteSpace(reqOrg.Value))
            {
                return new Response(BaseErrorMessage(FhirConstants.JwtRequestingOrganization));
            }

            if (!reqOrg.Value.StartsWith(FhirConstants.SystemOrgCode) || string.IsNullOrWhiteSpace(orgCode))
            {
                return new Response($"requesting_organisation ({reqOrg.Value}) must be of the form [{FhirConstants.SystemOrgCode}|[ODSCode]");
            }

            if (!IsOrgInMap(orgCode))
            {
                return new Response($"The ODS code defined in the requesting_organisation({orgCode}) is unknown");
            }

            // ### requesting_organization against requesting_system checks
            if (fromAsidMap.OrgCode != orgCode)
            {
                return new Response($"requesting_system ASID ({fromAsid}) is not associated with the requesting_organisation ODS code ({orgCode})");
            }



            return new Response(true);
        }

        private string DecodePart(string partHash)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(partHash.PadRight(partHash.Length + (4 - partHash.Length % 4) % 4, '=')));
        }

        private bool IsOrgInMap(string orgCode)
        {
            var clientAsidMap = GetAsidMap();

            if (clientAsidMap != null && clientAsidMap.ClientAsids.Any(d => d.Value != null && !string.IsNullOrEmpty(orgCode) && d.Value.OrgCode == orgCode))
            {
                return true;
            }

            return false;
        }

        private ClientAsid GetFromAsidMap(string fromASID)
        {

            var clientAsidMap = GetAsidMap();

            if (clientAsidMap == null || !clientAsidMap.ClientAsids.ContainsKey(fromASID))
            {
                return null;
            }

            return clientAsidMap.ClientAsids[fromASID];
        }

        private string BaseErrorMessage(string claimName)
        {
            return $"The mandatory claim {claimName} from the JWT associated with the Authorisation header is missing";
        }

        private ClientAsidMap GetAsidMap()
        {
            ClientAsidMap clientAsidMap;

            if (!_cache.TryGetValue<ClientAsidMap>(ClientAsidMap.Key, out clientAsidMap))
            {
                return null;
            }

            if(clientAsidMap.ClientAsids == null)
            {
                return null;
            }

            return clientAsidMap;
        }
    }
}
