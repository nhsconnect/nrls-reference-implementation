using Hl7.Fhir.Model;
using NRLS_API.Core.Helpers;
using NRLS_API.Core.Resources;
using NRLS_APITest.Comparer;
using NRLS_APITest.Data;
using System;
using System.Collections.Generic;
using Xunit;

namespace NRLS_APITest.Core.Helpers
{
    public class NrlsPointerHelperTests
    {
        [Fact]
        public void CreateOrg_Returns_Null()
        {

            var request = NrlsPointerHelper.CreateOrgSearch(FhirRequests.Valid_Create, null);

            Assert.Null(request);
        }

        [Fact]
        public void CreateOrg_Returns_Valid_Search()
        {
            var request = NrlsPointerHelper.CreateOrgSearch(FhirRequests.Valid_Create, "TestOrg");

            var expected = new List<Tuple<string, string>>
            {
                new Tuple<string, string>("identifier", $"{FhirConstants.SystemOrgCode}|TestOrg")
            };

            Assert.Equal(expected, request.QueryParameters, Comparers.ModelComparer<IEnumerable<Tuple<string, string>>>());
        }

        [Fact]
        public void CreatePatient_Returns_Valid_Search()
        {
            var request = NrlsPointerHelper.CreatePatientSearch(FhirRequests.Valid_Create, "1445545101");

            var expected = new List<Tuple<string, string>>
            {
                new Tuple<string, string>("identifier", $"{FhirConstants.SystemNhsNumber}|1445545101")
            };

            Assert.Equal(expected, request.QueryParameters, Comparers.ModelComparer<IEnumerable<Tuple<string, string>>>());

            Assert.Equal("Patient", request.StrResourceType);

            Assert.Equal("http://hl7.org/fhir/STU3/patient.html", request.ProfileUri);
        }

        [Fact]
        public void CreateMasterId_Returns_Null()
        {

            var request = NrlsPointerHelper.CreateMasterIdentifierSearch(FhirRequests.Valid_Create, null, "nhsNumber");

            Assert.Null(request);
        }

        [Fact]
        public void CreateMasterId_Returns_Valid_Search()
        {
            var masterIdentifier = new Identifier("validSystem", "validValue");

            var request = NrlsPointerHelper.CreateMasterIdentifierSearch(FhirRequests.Valid_Create, masterIdentifier, "nhsNumber");

            var expectedParams = new List<Tuple<string, string>>
            {
                new Tuple<string, string>("identifier", "validSystem|validValue"),
                new Tuple<string, string>("subject", $"{FhirConstants.SystemPDS}nhsNumber")
            };

            Assert.Equal(expectedParams, request.QueryParameters, Comparers.ModelComparer<IEnumerable<Tuple<string, string>>>());

            Assert.Contains("identifier", request.AllowedParameters);
        }

        [Fact]
        public void CreateReferenceSearch_Valid_HasLogicalId()
        {

            var request = NrlsPointerHelper.CreateReferenceSearch(FhirRequests.Valid_Create, "logicalIdA");

            Assert.Equal("logicalIdA", request.Id);
            Assert.Equal(ResourceType.DocumentReference, request.ResourceType);
        }

        [Fact]
        public void CreateReferenceSearch_Invalid_NoLogicalId()
        {

            var request = NrlsPointerHelper.CreateReferenceSearch(FhirRequests.Valid_Create, null);

            Assert.Null(request);
        }

        [Fact]
        public void CreateReferenceSearch_Invalid_EmptyLogicalId()
        {

            var request = NrlsPointerHelper.CreateReferenceSearch(FhirRequests.Valid_Create, "");

            Assert.Null(request);
        }
    }

}
