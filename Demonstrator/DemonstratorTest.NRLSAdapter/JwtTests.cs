using Demonstrator.NRLSAdapter.Helpers;
using System;
using Xunit;

namespace DemonstratorTest.NRLSAdapter
{
    public class JwtTests
    {
        [Fact]
        public void JwtFactory_Valid_JWT()
        {
            //Token generated using:
            //
            // scope = JwtScopes.Write
            // orgCode = ORG1
            // roleProfileId = fakeRoleId
            // asid = 20000000017
            // endpoint = https://nrls.com/fhir/documentreference
            // tokenOrigin = https://demonstrator.com
            // tokenStart = 2018-04-01T10:00:30+00:00

            var expectedToken = "eyJhbGciOiJub25lIiwidHlwIjoiSldUIn0.eyJpc3MiOiJodHRwczovL2RlbW9uc3RyYXRvci5jb20iLCJzdWIiOiJodHRwczovL2ZoaXIubmhzLnVrL0lkL3Nkcy1yb2xlLXByb2ZpbGUtaWR8ZmFrZVJvbGVJZCIsImF1ZCI6Imh0dHBzOi8vbnJscy5jb20vZmhpci9kb2N1bWVudHJlZmVyZW5jZSIsImV4cCI6MTUyMjU3NzEzMCwiaWF0IjoxNTIyNTc2ODMwLCJyZWFzb25fZm9yX3JlcXVlc3QiOiJkaXJlY3RjYXJlIiwic2NvcGUiOiJwYXRpZW50L0RvY3VtZW50UmVmZXJlbmNlLndyaXRlIiwicmVxdWVzdGluZ19zeXN0ZW0iOiJodHRwczovL2ZoaXIubmhzLnVrL0lkL2FjY3JlZGl0ZWQtc3lzdGVtfDIwMDAwMDAwMDE3IiwicmVxdWVzdGluZ19vcmdhbml6YXRpb24iOiJodHRwczovL2ZoaXIubmhzLnVrL0lkL29kcy1vcmdhbml6YXRpb24tY29kZXxPUkcxIiwicmVxdWVzdGluZ191c2VyIjoiaHR0cHM6Ly9maGlyLm5ocy51ay9JZC9zZHMtcm9sZS1wcm9maWxlLWlkfGZha2VSb2xlSWQifQ.";
            var tokenStart = new DateTime(2018, 4, 1, 10, 00, 30, DateTimeKind.Utc);

            var actualToken = JwtFactory.Generate(JwtScopes.Write, "ORG1", "fakeRoleId", "20000000017", "https://nrls.com/fhir/documentreference", "https://demonstrator.com", tokenStart);

            Assert.Equal(expectedToken, actualToken);
        }

        [Fact]
        public void JwtFactory_Invalid_JWT()
        {
            //Token generated using:
            //
            // scope = JwtScopes.Write
            // orgCode = ORG1
            // roleProfileId = fakeRoleId
            // asid = 20000000017
            // endpoint = https://nrls.com/fhir/documentreference
            // tokenOrigin = https://demonstrator.com
            // tokenStart = 2018-04-01T10:00:30+00:00

            var expectedToken = "eyJhbGciOiJub25lIiwidHlwIjoiSldUIn0.eyJpc3MiOiJodHRwczovL2RlbW9uc3RyYXRvci5jb20iLCJzdWIiOiJodHRwczovL2ZoaXIubmhzLnVrL0lkL3Nkcy1yb2xlLXByb2ZpbGUtaWR8ZmFrZVJvbGVJZCIsImF1ZCI6Imh0dHBzOi8vbnJscy5jb20vZmhpci9kb2N1bWVudHJlZmVyZW5jZSIsImV4cCI6MTUyMjU3NzEzMCwiaWF0IjoxNTIyNTc2ODMwLCJyZWFzb25fZm9yX3JlcXVlc3QiOiJkaXJlY3RjYXJlIiwic2NvcGUiOiJwYXRpZW50L0RvY3VtZW50UmVmZXJlbmNlLndyaXRlIiwicmVxdWVzdGluZ19zeXN0ZW0iOiJodHRwczovL2ZoaXIubmhzLnVrL0lkL2FjY3JlZGl0ZWQtc3lzdGVtfDIwMDAwMDAwMDE3IiwicmVxdWVzdGluZ19vcmdhbml6YXRpb24iOiJodHRwczovL2ZoaXIubmhzLnVrL0lkL29kcy1vcmdhbml6YXRpb24tY29kZXxPUkcxIiwicmVxdWVzdGluZ191c2VyIjoiaHR0cHM6Ly9maGlyLm5ocy51ay9JZC9zZHMtcm9sZS1wcm9maWxlLWlkfGZha2VSb2xlSWQifQ.";
            var tokenStart = new DateTime(2018, 4, 1, 20, 00, 30, DateTimeKind.Utc);

            var actualToken = JwtFactory.Generate(JwtScopes.Write, "ORG2", "fakeRoleId", "20000000017", "https://nrls.com/fhir/documentreference", "https://demonstrator.com", tokenStart);

            Assert.NotEqual(expectedToken, actualToken);
        }

    }
}
