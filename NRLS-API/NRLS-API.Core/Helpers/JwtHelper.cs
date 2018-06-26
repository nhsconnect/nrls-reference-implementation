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

            //This should be a basic Base64UrlEncoded token
            var claimsHashItems = jwt.Split('.');
            var claimsHash = claimsHashItems.Skip(1).Take(1).FirstOrDefault();

            if (claimsHashItems.Count() != 3 || string.IsNullOrEmpty(claimsHash))
            {
                return new Response("The JWT associated with the Authorisation header must have the 3 sections");
            }

            claimsHash = claimsHash.Replace('-', '+').Replace('_', '/');

            var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(claimsHash.PadRight(claimsHash.Length + (4 - claimsHash.Length % 4) % 4, '=')));

            IDictionary<string, string> claims;

            try
            {
                claims = JsonConvert.DeserializeObject<IDictionary<string, string>>(decoded);
            }
            catch
            {
                return new Response("The Authorisation header must be supplied");
            }

            var iss = claims.FirstOrDefault(x => x.Key.Equals(FhirConstants.JwtClientSysIssuer));

            if (string.IsNullOrEmpty(iss.Value))
            {
                return new Response("The mandatory claim iss from the JWT associated with the Authorisation header is missing");
            }

            var aud = claims.FirstOrDefault(x => x.Key.Equals(FhirConstants.JwtEndpointUrl));

            if (string.IsNullOrEmpty(aud.Value))
            {
                return new Response("The mandatory claim aud from the JWT associated with the Authorisation header is missing");
            }

            var iat = claims.FirstOrDefault(x => x.Key.Equals(FhirConstants.JwtIssued));
            long iatVal;

            if (!long.TryParse(iat.Value, out iatVal))
            {
                return new Response("The mandatory claim iat from the JWT associated with the Authorisation header is missing");
            }

            var exp = claims.FirstOrDefault(x => x.Key.Equals(FhirConstants.JwtExpiration));
            long expVal;

            if (!long.TryParse(exp.Value, out expVal))
            {
                return new Response("The mandatory claim iat from the JWT associated with the Authorisation header is missing");
            }

            var issuedDt = EpochTime.DateTime(iatVal);
            var expireDt = EpochTime.DateTime(expVal);

            if (issuedDt.AddMinutes(5) != expireDt )
            {
                return new Response($"exp {exp.Value} must be 5 minutes greater than iat {iat.Value}");
            }

            if (issuedDt > now || expireDt < now)
            {
                return new Response($"iat {iat.Value} must be in the past or now and exp {exp.Value} must be in the future or now");
            }

            var resForReq = claims.FirstOrDefault(x => x.Key.Equals(FhirConstants.JwtReasonForRequest));

            if (resForReq.Value != "directcare")
            {
                return new Response($"reason_for_request ({resForReq.Value}) must be ‘directcare’");
            }

            var scope = claims.FirstOrDefault(x => x.Key.Equals(FhirConstants.JwtScope));
            var expScope = $"patient/DocumentReference.{reqScope.ToString().ToLowerInvariant()}";

            if (scope.Value != expScope || !new string[2] { "patient/DocumentReference.read", "patient/DocumentReference.write" }.Contains(scope.Value))
            {
                return new Response($"scope ({scope.Value}) must match either 'patient/DocumentReference.read' or 'patient/DocumentReference.write'");
            }

            var sub = claims.FirstOrDefault(x => x.Key.Equals(FhirConstants.JwtIndOrSysIdentifier));

            if (string.IsNullOrEmpty(sub.Value))
            {
                return new Response("The mandatory claim sub from the JWT associated with the Authorisation header is missing");
            }

            var reqSys = claims.FirstOrDefault(x => x.Key.Equals(FhirConstants.JwtRequestingSystem));
            var fromAsid = reqSys.Value.Replace(FhirConstants.SystemASID, "").Replace("|", "");

            if (string.IsNullOrEmpty(reqSys.Value))
            {
                return new Response("The mandatory claim requesting_system from the JWT associated with the Authorisation header is missing");
            }

            if (!reqSys.Value.StartsWith(FhirConstants.SystemASID) || string.IsNullOrWhiteSpace(fromAsid))
            {
                return new Response($"requesting_system ({reqSys.Value}) must be of the form [{FhirConstants.SystemASID}|[ASID]]");
            }

            var fromAsidMap = GetFromAsidMap(fromAsid);

            if (fromAsidMap == null)
            {
                return new Response($"The ASID defined in the requesting_system ({fromAsid}) is unknown");
            }

            var reqOrg = claims.FirstOrDefault(x => x.Key.Equals(FhirConstants.JwtRequestingOrganization));
            var orgCode = reqOrg.Value.Replace(FhirConstants.SystemOrgCode, "").Replace("|", "");

            if (string.IsNullOrEmpty(reqOrg.Value))
            {
                return new Response("The mandatory claim requesting_organization from the JWT associated with the Authorisation header is missing");
            }

            if (!reqOrg.Value.StartsWith(FhirConstants.SystemOrgCode) || string.IsNullOrWhiteSpace(orgCode))
            {
                return new Response($"requesting_organisation ({reqOrg.Value}) must be of the form [{FhirConstants.SystemOrgCode}|[ODSCode]");
            }

            if(fromAsidMap.OrgCode != orgCode)
            {
                return new Response($"requesting_system ASID ({fromAsid}) is not associated with the requesting_organisation ODS code ({orgCode})");
            }

            var reqUsr = claims.FirstOrDefault(x => x.Key.Equals(FhirConstants.JwtRequestingUser));

            if (!string.IsNullOrEmpty(reqUsr.Value) && reqUsr.Value != sub.Value)
            {
                return new Response($"requesting_user ({reqUsr.Value}) and sub ({sub.Value}) claim's values must match");
            }

            if (string.IsNullOrEmpty(reqUsr.Value) && reqSys.Value != sub.Value)
            {
                return new Response($"requesting_system ({reqSys.Value}) and sub ({sub.Value}) claim's values must match");
            }

            return new Response(true);
        }

        private ClientAsid GetFromAsidMap(string fromASID)
        {
            ClientAsidMap clientAsidMap;

            if (!_cache.TryGetValue<ClientAsidMap>(ClientAsidMap.Key, out clientAsidMap))
            {
                return null;
            }

            if (clientAsidMap.ClientAsids == null || !clientAsidMap.ClientAsids.ContainsKey(fromASID))
            {
                return null;
            }

            return clientAsidMap.ClientAsids[fromASID];
        }
    }
}
