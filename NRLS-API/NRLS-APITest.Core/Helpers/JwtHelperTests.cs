﻿using Microsoft.Extensions.Caching.Memory;
using Moq;
using NRLS_API.Core.Enums;
using NRLS_API.Core.Helpers;
using NRLS_API.Core.Interfaces.Services;
using NRLS_API.Models.Core;
using NRLS_API.Models.ViewModels.Core;
using NRLS_APITest.Data;
using NRLS_APITest.StubClasses;
using System;
using System.Collections.Generic;
using Xunit;

namespace NRLS_APITest.Core.Helpers
{
    public class JwtHelperTests : IDisposable
    {
        private ISdsService _sdsService;

        public JwtHelperTests()
        {
            var sdsMock = new Mock<ISdsService>();
            sdsMock.Setup(op => op.GetFor(It.IsAny<string>())).Returns((SdsViewModel)null);
            sdsMock.Setup(op => op.GetFor(It.Is<string>(x => x == "20000000017"))).Returns(SdsViewModels.SdsAsid20000000017);
            sdsMock.Setup(op => op.GetFor(It.Is<string>(x => x == "002"))).Returns(SdsViewModels.SdsAsid002);

            sdsMock.Setup(op => op.GetFor(It.IsAny<string>(), null)).Returns((SdsViewModel)null);
            sdsMock.Setup(op => op.GetFor(It.Is<string>(x => x == "ORG1"), null)).Returns(SdsViewModels.SdsAsid20000000017);

            _sdsService = sdsMock.Object;
        }

        public void Dispose()
        {
            _sdsService = null;
        }

        [Theory]
        [InlineData("eyJhbGciOiJub25lIiwidHlwIjoiSldUIn0.eyJpc3MiOiJodHRwczovL2RlbW9uc3RyYXRvci5jb20iLCJzdWIiOiJodHRwczovL2ZoaXIubmhzLnVrL0lkL3Nkcy1yb2xlLXByb2ZpbGUtaWR8ZmFrZVJvbGVJZCIsImF1ZCI6Imh0dHBzOi8vbnJscy5jb20vZmhpci9kb2N1bWVudHJlZmVyZW5jZSIsImV4cCI6MTUyMjU3NzEzMCwiaWF0IjoxNTIyNTc2ODMwLCJyZWFzb25fZm9yX3JlcXVlc3QiOiJkaXJlY3RjYXJlIiwic2NvcGUiOiJwYXRpZW50L0RvY3VtZW50UmVmZXJlbmNlLndyaXRlIiwicmVxdWVzdGluZ19zeXN0ZW0iOiJodHRwczovL2ZoaXIubmhzLnVrL0lkL2FjY3JlZGl0ZWQtc3lzdGVtfDIwMDAwMDAwMDE3IiwicmVxdWVzdGluZ19vcmdhbml6YXRpb24iOiJodHRwczovL2ZoaXIubmhzLnVrL0lkL29kcy1vcmdhbml6YXRpb24tY29kZXxPUkcxIiwicmVxdWVzdGluZ191c2VyIjoiaHR0cHM6Ly9maGlyLm5ocy51ay9JZC9zZHMtcm9sZS1wcm9maWxlLWlkfGZha2VSb2xlSWQifQ.")]
        [InlineData("Bearer eyJhbGciOiJub25lIiwidHlwIjoiSldUIn0.eyJpc3MiOiJodHRwczovL2RlbW9uc3RyYXRvci5jb20iLCJzdWIiOiJodHRwczovL2ZoaXIubmhzLnVrL0lkL3Nkcy1yb2xlLXByb2ZpbGUtaWR8ZmFrZVJvbGVJZCIsImF1ZCI6Imh0dHBzOi8vbnJscy5jb20vZmhpci9kb2N1bWVudHJlZmVyZW5jZSIsImV4cCI6MTUyMjU3NzEzMCwiaWF0IjoxNTIyNTc2ODMwLCJyZWFzb25fZm9yX3JlcXVlc3QiOiJkaXJlY3RjYXJlIiwic2NvcGUiOiJwYXRpZW50L0RvY3VtZW50UmVmZXJlbmNlLndyaXRlIiwicmVxdWVzdGluZ19zeXN0ZW0iOiJodHRwczovL2ZoaXIubmhzLnVrL0lkL2FjY3JlZGl0ZWQtc3lzdGVtfDIwMDAwMDAwMDE3IiwicmVxdWVzdGluZ19vcmdhbml6YXRpb24iOiJodHRwczovL2ZoaXIubmhzLnVrL0lkL29kcy1vcmdhbml6YXRpb24tY29kZXxPUkcxIiwicmVxdWVzdGluZ191c2VyIjoiaHR0cHM6Ly9maGlyLm5ocy51ay9JZC9zZHMtcm9sZS1wcm9maWxlLWlkfGZha2VSb2xlSWQifQ.")]
        public void JWT_Check_ValidToken(string token)
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

            var issued = new DateTime(2018, 4, 1, 10, 0, 30, DateTimeKind.Utc);

            var jwtHelper = new JwtHelper(_sdsService);

            var actual = jwtHelper.IsValid(token, JwtScopes.Write, issued);

            Assert.True(actual.Success);
        }

        [Fact]
        public void JWT_Check_InvalidScope()
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

            var issued = new DateTime(2018, 4, 1, 10, 0, 30, DateTimeKind.Utc);

            var token = "eyJhbGciOiJub25lIiwidHlwIjoiSldUIn0.eyJpc3MiOiJodHRwczovL2RlbW9uc3RyYXRvci5jb20iLCJzdWIiOiJodHRwczovL2ZoaXIubmhzLnVrL0lkL3Nkcy1yb2xlLXByb2ZpbGUtaWR8ZmFrZVJvbGVJZCIsImF1ZCI6Imh0dHBzOi8vbnJscy5jb20vZmhpci9kb2N1bWVudHJlZmVyZW5jZSIsImV4cCI6MTUyMjU3NzEzMCwiaWF0IjoxNTIyNTc2ODMwLCJyZWFzb25fZm9yX3JlcXVlc3QiOiJkaXJlY3RjYXJlIiwic2NvcGUiOiJwYXRpZW50L0RvY3VtZW50UmVmZXJlbmNlLndyaXRlIiwicmVxdWVzdGluZ19zeXN0ZW0iOiJodHRwczovL2ZoaXIubmhzLnVrL0lkL2FjY3JlZGl0ZWQtc3lzdGVtfDIwMDAwMDAwMDE3IiwicmVxdWVzdGluZ19vcmdhbml6YXRpb24iOiJodHRwczovL2ZoaXIubmhzLnVrL0lkL29kcy1vcmdhbml6YXRpb24tY29kZXxPUkcxIiwicmVxdWVzdGluZ191c2VyIjoiaHR0cHM6Ly9maGlyLm5ocy51ay9JZC9zZHMtcm9sZS1wcm9maWxlLWlkfGZha2VSb2xlSWQifQ.";

            var jwtHelper = new JwtHelper(_sdsService);

            var actual = jwtHelper.IsValid(token, JwtScopes.Read, issued);

            Assert.False(actual.Success);
        }

        [Fact]
        public void JWT_Check_InvalidIssuedDate()
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

            var issued = new DateTime(2018, 4, 1, 10, 5, 30, DateTimeKind.Utc);

            var token = "eyJhbGciOiJub25lIiwidHlwIjoiSldUIn0.eyJpc3MiOiJodHRwczovL2RlbW9uc3RyYXRvci5jb20iLCJzdWIiOiJodHRwczovL2ZoaXIubmhzLnVrL0lkL3Nkcy1yb2xlLXByb2ZpbGUtaWR8ZmFrZVJvbGVJZCIsImF1ZCI6Imh0dHBzOi8vbnJscy5jb20vZmhpci9kb2N1bWVudHJlZmVyZW5jZSIsImV4cCI6MTUyMjU3NzEzMCwiaWF0IjoxNTIyNTc2ODMwLCJyZWFzb25fZm9yX3JlcXVlc3QiOiJkaXJlY3RjYXJlIiwic2NvcGUiOiJwYXRpZW50L0RvY3VtZW50UmVmZXJlbmNlLndyaXRlIiwicmVxdWVzdGluZ19zeXN0ZW0iOiJodHRwczovL2ZoaXIubmhzLnVrL0lkL2FjY3JlZGl0ZWQtc3lzdGVtfDIwMDAwMDAwMDE3IiwicmVxdWVzdGluZ19vcmdhbml6YXRpb24iOiJodHRwczovL2ZoaXIubmhzLnVrL0lkL29kcy1vcmdhbml6YXRpb24tY29kZXxPUkcxIiwicmVxdWVzdGluZ191c2VyIjoiaHR0cHM6Ly9maGlyLm5ocy51ay9JZC9zZHMtcm9sZS1wcm9maWxlLWlkfGZha2VSb2xlSWQifQ.";

            var jwtHelper = new JwtHelper(_sdsService);

            var actual = jwtHelper.IsValid(token, JwtScopes.Read, issued);

            Assert.False(actual.Success);
        }

        [Theory]
        [InlineData("eyJhbGciOiJub25lIiwidHlwIjoiSldUIn0.blaaablaaa.")]
        [InlineData(".blaaablaaa.")]
        [InlineData("eyJpc3MiOiJodHRwczovL2RlbW9uc3RyYXRvci5jb20iLCJzdWIiOiJodHRwczovL2ZoaXIubmhzLnVrL0lkL3Nkcy1yb2xlLXByb2ZpbGUtaWR8ZmFrZVJvbGVJZCIsImF1ZCI6Imh0dHBzOi8vbnJscy5jb20vZmhpci9kb2N1bWVudHJlZmVyZW5jZSIsImV4cCI6MTUyMjU3NzEzMCwiaWF0IjoxNTIyNTc2ODMwLCJyZWFzb25fZm9yX3JlcXVlc3QiOiJkaXJlY3RjYXJlIiwic2NvcGUiOiJwYXRpZW50L0RvY3VtZW50UmVmZXJlbmNlLndyaXRlIiwicmVxdWVzdGluZ19zeXN0ZW0iOiJodHRwczovL2ZoaXIubmhzLnVrL0lkL2FjY3JlZGl0ZWQtc3lzdGVtfDIwMDAwMDAwMDE3IiwicmVxdWVzdGluZ19vcmdhbml6YXRpb24iOiJodHRwczovL2ZoaXIubmhzLnVrL0lkL29kcy1vcmdhbml6YXRpb24tY29kZXxPUkcxIiwicmVxdWVzdGluZ191c2VyIjoiaHR0cHM6Ly9maGlyLm5ocy51ay9JZC9zZHMtcm9sZS1wcm9maWxlLWlkfGZha2VSb2xlSWQifQ")]
        [InlineData("eyJhbGciOiJub25lIiwidHlwIjoiSldUIn0.eyJpc3MiOiJodHRwczovL2RlbW9uc3RyYXRvci5jb20iLCJzdWIiOiJodHRwczovL2ZoaXIubmhzLnVrL0lkL3Nkcy1yb2xlLXByb2ZpbGUtaWR8ZmFrZVJvbGVJZCIsImF1ZCI6Imh0dHBzOi8vbnJscy5jb20vZmhpci9kb2N1bWVudHJlZmVyZW5jZSIsImV4cCI6MTUyMjU3NzEzMCwiaWF0IjoxNTIyNTc2ODMwLCJyZWFzb25fZm9yX3JlcXVlc3QiOiJkaXJlY3RjYXJlIiwic2NvcGUiOiJwYXRpZW50L0RvY3VtZW50UmVmZXJlbmNlLndyaXRlIiwicmVxdWVzdGluZ19zeXN0ZW0iOiJodHRwczovL2ZoaXIubmhzLnVrL0lkL2FjY3JlZGl0ZWQtc3lzdGVtfDIwMDAwMDAwMDE3IiwicmVxdWVzdGluZ19vcmdhbml6YXRpb24iOiJodHRwczovL2ZoaXIubmhzLnVrL0lkL29kcy1vcmdhbml6YXRpb24tY29kZXxPUkcxIiwicmVxdWVzdGluZ191c2VyIjoiaHR0cHM6Ly9maGlyLm5ocy51ay9JZC9zZHMtcm9sZS1wcm9maWxlLWlkfGZha2VSb2xlSWQifQblaaaa")]
        [InlineData("Bearer blaaaa")]
        [InlineData(null)]
        public void JWT_Check_InvalidToken(string token)
        {
            var issued = new DateTime(2018, 4, 1, 10, 0, 30, DateTimeKind.Utc);

            var jwtHelper = new JwtHelper(_sdsService);

            var actual = jwtHelper.IsValid(token, JwtScopes.Write, issued);

            Assert.False(actual.Success);
        }
    }
}
