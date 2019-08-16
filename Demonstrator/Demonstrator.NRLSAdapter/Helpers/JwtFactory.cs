using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Demonstrator.Core.Resources;

namespace Demonstrator.NRLSAdapter.Helpers
{
    public class JwtFactory
    {
        public static string Generate(JwtScopes scope, string orgCode, string roleProfileId, string asid, string endpoint, string tokenOrigin, string resource, DateTime? tokenStart = null)
        {
            return $"{Header}.{Payload(scope, orgCode, roleProfileId, asid, endpoint, tokenOrigin, resource, tokenStart)}.";
        }

        private static string Header => new JwtHeader().Base64UrlEncode();

        private static string Payload(JwtScopes scope, string orgCode, string roleProfileId, string asid, string endpoint, string tokenOrigin, string resource, DateTime? tokenStart)
        {
            var start = tokenStart.HasValue ? tokenStart.Value : DateTime.UtcNow;
            var claims = new List<Claim>();
            var exp = start.AddMinutes(5);
            var iat = start;

            claims.Add(new Claim(FhirConstants.JwtClientSysIssuer, tokenOrigin, ClaimValueTypes.String));
            claims.Add(new Claim(FhirConstants.JwtIndOrSysIdentifier, $"{FhirConstants.SystemSdsRole}|{roleProfileId}", ClaimValueTypes.String));
            claims.Add(new Claim(FhirConstants.JwtEndpointUrl, endpoint, ClaimValueTypes.String));
            claims.Add(new Claim(FhirConstants.JwtExpiration, EpochTime.GetIntDate(exp).ToString(), ClaimValueTypes.Integer64));
            claims.Add(new Claim(FhirConstants.JwtIssued, EpochTime.GetIntDate(iat).ToString(), ClaimValueTypes.Integer64));
            claims.Add(new Claim(FhirConstants.JwtReasonForRequest, "directcare", ClaimValueTypes.String));
            claims.Add(new Claim(FhirConstants.JwtScope, $"patient/{resource}.{scope.ToString().ToLowerInvariant()}", ClaimValueTypes.String));
            claims.Add(new Claim(FhirConstants.JwtRequestingSystem, $"{FhirConstants.SystemASID}|{asid}", ClaimValueTypes.String)); // Should this not be JsonClaimValueTypes.Json
            claims.Add(new Claim(FhirConstants.JwtRequestingOrganization, $"{FhirConstants.SystemOrgCode}|{orgCode}", ClaimValueTypes.String)); // Should this not be JsonClaimValueTypes.Json
            claims.Add(new Claim(FhirConstants.JwtRequestingUser, $"{FhirConstants.SystemSdsRole}|{roleProfileId}", ClaimValueTypes.String)); // Should this not be JsonClaimValueTypes.Json

            return new JwtPayload(claims).Base64UrlEncode();
        }

    }

    public enum JwtScopes
    {
        Read = 1,
        Write
    }
}
