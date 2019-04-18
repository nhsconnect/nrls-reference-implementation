using Hl7.Fhir.Model;
using NRLS_API.Models.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace NRLS_APITest.Models.Extensions
{
    public class SearchParametersTests
    {
        [Fact]
        public void SearchParameters_GetAllowedPatient()
        {
            var allowed = ResourceType.Patient.GetAllowed();

            Assert.NotNull(allowed);
            Assert.Single(allowed);

            Assert.Contains("identifier", allowed);
        }

        [Fact]
        public void SearchParameters_GetAllowedOrganization()
        {
            var allowed = ResourceType.Organization.GetAllowed();

            Assert.NotNull(allowed);
            Assert.Single(allowed);

            Assert.Contains("identifier", allowed);
        }

        [Fact]
        public void SearchParameters_GetAllowedDocumentReference()
        {
            var allowed = ResourceType.DocumentReference.GetAllowed();

            Assert.NotNull(allowed);
            Assert.Equal(8, allowed.Count());

            Assert.Contains("custodian", allowed);
            Assert.Contains("subject", allowed);
            Assert.Contains("_id", allowed);
            Assert.Contains("type", allowed);
            Assert.Contains("type.coding", allowed);
            Assert.Contains("_format", allowed);
            Assert.Contains("_summary", allowed);
            Assert.Contains("custodian.identifier", allowed);
        }
    }
}
