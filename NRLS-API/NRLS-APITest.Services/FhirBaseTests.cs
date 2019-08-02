using Hl7.Fhir.Model;
using Microsoft.Extensions.Options;
using Moq;
using NRLS_API.Core.Exceptions;
using NRLS_API.Models.Core;
using NRLS_APITest.Data;
using NRLS_APITest.StubClasses;
using System;
using System.Collections.Generic;
using Xunit;

namespace NRLS_APITest.Services
{
    public class FhirBaseTests : IDisposable
    {
        IOptionsSnapshot<NrlsApiSetting> _nrlsApiSettings;

        public FhirBaseTests()
        {
            var opts = AppSettings.NrlsApiSettings;

            var mock = new Mock<IOptionsSnapshot<NrlsApiSetting>>();
            mock.Setup(op => op.Get(It.IsAny<string>())).Returns(opts);

            _nrlsApiSettings = mock.Object;
        }

        public void Dispose()
        {
            _nrlsApiSettings = null;
        }

        [Fact]
        public void ValidateResource_Valid()
        {

            var baseService = new FhirBaseStub(_nrlsApiSettings);

            try
            {
                baseService.ValidateResourceStub("DocumentReference");
            }
            catch (Exception ex)
            {
                Assert.True(false, "No exception expected, but got: " + ex.Message);
            }
        }

        [Fact]
        public void ValidateResource_Invalid()
        {

            var baseService = new FhirBaseStub(_nrlsApiSettings);

            Assert.Throws<HttpFhirException>(() => {
                baseService.ValidateResourceStub("Patient");
            });
        }

        [Fact]
        public void ParseRead_ExpectBundle()
        {

            var organisations = new List<Organization>
            {
                new Organization
                {
                    Id = "testid",
                    Name = "TestName"
                }
            };

            var bundle = FhirBundle.GetBundle(organisations);
            var baseService = new FhirBaseStub(_nrlsApiSettings);

            var actual = baseService.ParseReadStub(bundle, "testid");

            Assert.Equal(ResourceType.Bundle, actual.ResourceType);
        }

        [Fact]
        public void ParseRead_ExpectOperationOutcomeWhenEmpty()
        {

            var organisations = new List<Organization>();

            var bundle = FhirBundle.GetBundle(organisations);
            var baseService = new FhirBaseStub(_nrlsApiSettings);

            Assert.Throws<HttpFhirException>(() => {
                var actual = baseService.ParseReadStub(bundle, "testid");
            });
        }

        [Fact]
        public void ParseRead_ExpectOperationOutcomeWhenNull()
        {
            var baseService = new FhirBaseStub(_nrlsApiSettings);

            Assert.Throws<HttpFhirException>(() => {
                var actual = baseService.ParseReadStub<Bundle>(null, "testid");
            });
        }

        [Fact]
        public void ParseRead_ExpectOrganization()
        {
            var organisation = new Organization
            {
                Id = "testid",
                Name = "TestName"
            };

            var baseService = new FhirBaseStub(_nrlsApiSettings);

            var actual = baseService.ParseReadStub(organisation, "testid");

            Assert.Equal(ResourceType.Organization, actual.ResourceType);
        }
    }
}
