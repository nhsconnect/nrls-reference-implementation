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
        public void Returns_Null()
        {

            var request = NrlsPointerHelper.CreateOrgSearch(FhirRequests.Valid_Create, null);

            Assert.Null(request);
        }

        [Fact]
        public void Returns_Valid_Search()
        {
            var request = NrlsPointerHelper.CreateOrgSearch(FhirRequests.Valid_Create, "TestOrg");

            var expected = new List<Tuple<string, string>>
            {
                new Tuple<string, string>("identifier", $"{FhirConstants.SystemOrgCode}|TestOrg")
            };

            Assert.Equal(expected, request.QueryParameters, Comparers.ModelComparer<IEnumerable<Tuple<string, string>>>());
        }
    }

}
