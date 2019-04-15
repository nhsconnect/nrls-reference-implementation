using System;
using Demonstrator.Core.Resources;
using System.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using Demonstrator.Models.Core.Models;
using Microsoft.Extensions.Caching.Memory;
using Demonstrator.Models.Core.Enums;
using Demonstrator.Core.Interfaces.Helpers;
using Demonstrator.Utilities;
using Demonstrator.Core.Interfaces.Services;

namespace Demonstrator.Core.Helpers
{
    public class JwtHelper : IJwtHelper
    {
        private ISdsService _sdsService;

        private string[] _validClaims = { "iss", "sub", "aud", "exp", "iat", "reason_for_request", "scope", "requesting_system", "requesting_organization", "requesting_user" };

        private string[] _validScopes = { "patient/DocumentReference.read", "patient/DocumentReference.write" };

        public JwtHelper(ISdsService sdsService)
        {
            _sdsService = sdsService;
        }

        public Response IsValidUser(string jwt)
        {

            var parsedJwt = ParseJwt(jwt);
            var claims = parsedJwt.Data;

            // ### sub
            var subClaim = CheckSub(claims);
            if (!subClaim.Success)
            {
                return subClaim;
            }

            // ### requesting_User
            var userClaim = CheckUser(claims, subClaim.Message);
            if (!userClaim.Success)
            {
                return userClaim;
            }

            return new Response(true);
        }

        public Response IsValid(string jwt, JwtScopes reqScope, DateTime? tokenIssued)
        {
            var now = tokenIssued ?? DateTime.UtcNow;

            var parsedJwt = ParseJwt(jwt);
            var claims = parsedJwt.Data;

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
            //var issuedDt = EpochTime.DateTime(iatVal);
            //var expireDt = EpochTime.DateTime(expVal);


            //if (issuedDt.AddMinutes(5) != expireDt )
            //{
            //    return new Response($"exp {exp.Value} must be 5 minutes greater than iat {iat.Value}");
            //}

            //if (issuedDt > now || expireDt < now)
            //{
            //    return new Response($"iat {iat.Value} must be in the past or now and exp {exp.Value} must be in the future or now");
            //}


            // ### sub
            var subClaim = CheckSub(claims);
            if (!subClaim.Success)
            {
                return subClaim;
            }

            // ### requesting_User
            var userClaim = CheckUser(claims, subClaim.Message);
            if(!userClaim.Success)
            {
                return userClaim;
            }

            // ### requesting_system
            var reqSys = claims.FirstOrDefault(x => x.Key.Equals(FhirConstants.JwtRequestingSystem));

            if (string.IsNullOrEmpty(reqSys.Value))
            {
                return new Response(BaseErrorMessage(FhirConstants.JwtRequestingSystem));
            }

            // check reqSys matches sub if this is a system type i.e. if no user claim
            if (string.IsNullOrEmpty(userClaim.Message) && reqSys.Value != subClaim.Message)
            {
                return new Response($"requesting_system ({reqSys.Value}) and sub ({subClaim.Message}) claim's values must match");
            }

            var fromAsid = reqSys.Value.Replace($"{FhirConstants.SystemASID}|", "");
            if (!reqSys.Value.StartsWith(FhirConstants.SystemASID) || string.IsNullOrWhiteSpace(fromAsid))
            {
                return new Response($"requesting_system ({reqSys.Value}) must be of the form [{FhirConstants.SystemASID}|[ASID]]");
            }

            var fromAsidMap = _sdsService.GetFor(fromAsid);

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

            if (_sdsService.GetFor(orgCode, null) == null)
            {
                return new Response($"The ODS code defined in the requesting_organisation({orgCode}) is unknown");
            }

            // ### requesting_organization against requesting_system checks
            if (fromAsidMap.OdsCode != orgCode)
            {
                return new Response($"requesting_system ASID ({fromAsid}) is not associated with the requesting_organisation ODS code ({orgCode})");
            }

            return new Response(true);
        }

        private DataResponse<IDictionary<string, string>> ParseJwt(string jwt)
        {
            var response = new DataResponse<IDictionary<string, string>>(true);

            if (string.IsNullOrEmpty(jwt))
            {
                response.SetError("The Authorisation header must be supplied");
                return response;
            }

            if (jwt.StartsWith("Bearer "))
            {
                jwt = jwt.Replace("Bearer ", "");
            }

            //This should be a basic Base64UrlEncoded token
            var claimsHashItems = jwt.Split('.');

            if (claimsHashItems.Count() != 3)
            {
                response.SetError("The JWT associated with the Authorisation header must have the 3 sections");
                return response;
            }

            //Skip header parsing
            //var header = claimsHashItems.First();

            //Skip sig check for now as no service available to validate. 
            //Current guidance is not to hash the sign for self generated JWTs.
            //var signature = claimsHashItems.Last();

            //if (!string.IsNullOrEmpty(signature))
            //{
            //    return new Response("The JWT associated with the Authorisation header must have an empty signature.");
            //}

            var claimsHash = claimsHashItems.Skip(1).Take(1).FirstOrDefault();

            if (string.IsNullOrEmpty(claimsHash))
            {
                response.SetError("The JWT associated with the Authorisation header must have a body of claims.");
                return response;
            }


            var decoded = StringHelper.Base64UrlDecode(claimsHash);
            response = ParseClaims(decoded);

            if (!response.Success)
            {
                response.SetError(response.Message);
                return response;
            }

            return response;
        }

        private DataResponse<IDictionary<string, string>> ParseClaims(string input)
        {
            var response = new DataResponse<IDictionary<string, string>>(true);

            try
            {
                response.Data = JsonConvert.DeserializeObject<IDictionary<string, string>>(input);
            }
            catch (JsonReaderException ex)
            {
                var errorMessage = "The Authorisation header must be supplied as a valid JWT.";

                if (!string.IsNullOrWhiteSpace(ex.Path) && _validClaims.Contains(ex.Path))
                {
                    errorMessage = BaseErrorMessage(ex.Path);
                }

                response.SetError(errorMessage);
            }
            catch (Exception ex)
            {
                response.SetError("The Authorisation header must be supplied");
            }

            return response;
        }

        private Response CheckUser(IDictionary<string, string> claims, string subValue)
        {

            var userRequired = subValue.StartsWith($"{FhirConstants.SystemSdsRole}|");

            var reqUsr = claims.FirstOrDefault(x => x.Key.Equals(FhirConstants.JwtRequestingUser));

            if (userRequired && string.IsNullOrEmpty(reqUsr.Value))
            {
                return new Response(BaseErrorMessage(FhirConstants.JwtRequestingUser));
            }

            // check reqUser matches sub if reqUsr was supplied, i.e. if this is a user type request rather than an system type request
            if (!string.IsNullOrEmpty(reqUsr.Value) && reqUsr.Value != subValue)
            {
                return new Response($"requesting_user ({reqUsr.Value}) and sub ({subValue}) claim's values must match");
            }

            var userRoleProfileId = reqUsr.Value?.Replace($"{FhirConstants.SystemSdsRole}|", "");

            if (!string.IsNullOrEmpty(reqUsr.Value) && (!reqUsr.Value.StartsWith(FhirConstants.SystemSdsRole) || string.IsNullOrWhiteSpace(userRoleProfileId)))
            {
                return new Response($"requesting_user ({reqUsr.Value}) must be of the form [{FhirConstants.SystemSdsRole}|[SDSRoleProfileID]]");
            }

            return new Response(true, reqUsr.Value);
        }

        private Response CheckSub(IDictionary<string, string> claims)
        {
            var sub = claims.FirstOrDefault(x => x.Key.Equals(FhirConstants.JwtIndOrSysIdentifier));

            if (string.IsNullOrEmpty(sub.Value))
            {
                return new Response(BaseErrorMessage(FhirConstants.JwtIndOrSysIdentifier));
            }

            return new Response(true, sub.Value);
        }

        private string BaseErrorMessage(string claimName)
        {
            return $"The mandatory claim {claimName} from the JWT associated with the Authorisation header is missing";
        }

    }
}
