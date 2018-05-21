using System;
using System.IdentityModel.Tokens.Jwt;
using NRLS_API.Core.Resources;
using System.Linq;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace NRLS_API.Core.Helpers
{
    public class JwtHelper
    {
        public static bool IsValid(string jwt, JwtScopes reqScope, DateTime? tokenIssued = null)
        {
            var now = tokenIssued ?? DateTime.UtcNow;

            if (string.IsNullOrEmpty(jwt))
            {
                return false;
            }

            
            if(jwt.StartsWith("Bearer "))
            {
                jwt = jwt.Replace("Bearer ", "");
            }

            //This should be a basic Base64UrlEncoded token
            var claimsHash = jwt.Split('.').Skip(1).Take(1).FirstOrDefault();

            if (string.IsNullOrEmpty(claimsHash))
            {
                return false;
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
                return false;
            }

            var iss = claims.FirstOrDefault(x => x.Key.Equals(FhirConstants.JwtClientSysIssuer));

            if (string.IsNullOrEmpty(iss.Value))
            {
                return false;
            }

            var sub = claims.FirstOrDefault(x => x.Key.Equals(FhirConstants.JwtIndOrSysIdentifier));

            if (string.IsNullOrEmpty(sub.Value))
            {
                return false;
            }

            var aud = claims.FirstOrDefault(x => x.Key.Equals(FhirConstants.JwtEndpointUrl));

            if (string.IsNullOrEmpty(aud.Value))
            {
                return false;
            }

            var iat = claims.FirstOrDefault(x => x.Key.Equals(FhirConstants.JwtIssued));
            long iatVal;

            if (!long.TryParse(iat.Value, out iatVal))
            {
                return false;
            }

            var exp = claims.FirstOrDefault(x => x.Key.Equals(FhirConstants.JwtExpiration));
            long expVal;

            if (!long.TryParse(exp.Value, out expVal))
            {
                return false;
            }

            var issuedDt = EpochTime.DateTime(iatVal);
            var expireDt = EpochTime.DateTime(expVal);

            if (issuedDt.AddMinutes(5) != expireDt || issuedDt > now || expireDt < now)
            {
                return false;
            }

            var resForReq = claims.FirstOrDefault(x => x.Key.Equals(FhirConstants.JwtReasonForRequest));

            if (resForReq.Value != "directcare")
            {
                return false;
            }

            var scope = claims.FirstOrDefault(x => x.Key.Equals(FhirConstants.JwtScope));
            var expScope = $"patient/DocumentReference.{reqScope.ToString().ToLowerInvariant()}";

            if (scope.Value != expScope)
            {
                return false;
            }

            var reqSys = claims.FirstOrDefault(x => x.Key.Equals(FhirConstants.JwtRequestingSystem));

            if (string.IsNullOrEmpty(reqSys.Value))
            {
                return false;
            }

            var reqOrg = claims.FirstOrDefault(x => x.Key.Equals(FhirConstants.JwtRequestingOrganization));

            if (string.IsNullOrEmpty(reqOrg.Value))
            {
                return false;
            }

            var reqUsr = claims.FirstOrDefault(x => x.Key.Equals(FhirConstants.JwtRequestingUser));

            if (string.IsNullOrEmpty(reqUsr.Value))
            {
                return false;
            }

            return true;
        }
    }

    public enum JwtScopes
    {
        Read = 1,
        Write
    }
}
