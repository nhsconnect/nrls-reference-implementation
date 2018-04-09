using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace Demonstrator.NRLSAdapter.Helpers
{
    public class JwtFactory
    {
        public static string Generate(JwtScopes scope, string orgCode, string roleProfileId, string asid, string endpoint, string tokenOrigin)
        {
            return $"{Header}.{Payload(scope, orgCode, roleProfileId, asid, endpoint, tokenOrigin)}.";
        }

        private static string Header => new JwtHeader().Base64UrlEncode();

        private static string Payload(JwtScopes scope, string orgCode, string roleProfileId, string asid, string endpoint, string tokenOrigin)
        {
            var claims = new List<Claim>();
            var exp = DateTime.UtcNow.AddMinutes(5);
            var iat = DateTime.UtcNow;

            claims.Add(new Claim(FhirConstants.JwtClientSysIssuer, tokenOrigin, ClaimValueTypes.String));
            claims.Add(new Claim(FhirConstants.JwtIndOrSysIdentifier, $"{FhirConstants.IdsSdsRolePofileId}|{roleProfileId}", ClaimValueTypes.String));
            claims.Add(new Claim(FhirConstants.JwtEndpointUrl, endpoint, ClaimValueTypes.String));
            claims.Add(new Claim(FhirConstants.JwtExpiration, EpochTime.GetIntDate(exp).ToString(), ClaimValueTypes.Integer64));
            claims.Add(new Claim(FhirConstants.JwtIssued, EpochTime.GetIntDate(iat).ToString(), ClaimValueTypes.Integer64));
            claims.Add(new Claim(FhirConstants.JwtReasonForRequest, "directcare", ClaimValueTypes.String));
            claims.Add(new Claim(FhirConstants.JwtScope, $"patient/DocumentReference.{scope.ToString().ToLowerInvariant()}", ClaimValueTypes.String));
            claims.Add(new Claim(FhirConstants.JwtRequestingSystem, $"{FhirConstants.IdsAccrSystem}|{asid}", ClaimValueTypes.String)); // Should this not be JsonClaimValueTypes.Json
            claims.Add(new Claim(FhirConstants.JwtRequestingOrganization, $"{FhirConstants.IdsOrgCode}|{orgCode}", ClaimValueTypes.String)); // Should this not be JsonClaimValueTypes.Json
            claims.Add(new Claim(FhirConstants.JwtRequestingUser, $"{FhirConstants.IdsSdsRolePofileId}|{roleProfileId}", ClaimValueTypes.String)); // Should this not be JsonClaimValueTypes.Json

            return new JwtPayload(claims).Base64UrlEncode();
        }

    }

    public enum JwtScopes
    {
        Read = 1,
        Write
    }
}
