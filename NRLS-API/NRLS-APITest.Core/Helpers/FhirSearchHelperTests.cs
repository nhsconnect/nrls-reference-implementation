using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using NRLS_API.Core.Helpers;
using NRLS_API.Core.Interfaces.Helpers;
using NRLS_API.Core.Resources;
using NRLS_APITest.Data;
using System;
using Xunit;

namespace NRLS_APITest.Core.Helpers
{
    public class FhirSearchHelperTests : IDisposable
    {
        private IFhirCacheHelper _fhirCacheHelper;

        public FhirSearchHelperTests()
        {
            var mockCacheHelper = new Mock<IFhirCacheHelper>();
            mockCacheHelper.Setup(op => op.GetResourceProfile(It.Is<string>(s => s.Equals(FhirConstants.SystemNrlsProfile)))).Returns(FhirResources.SD_NrlsPointer);

            _fhirCacheHelper = mockCacheHelper.Object;
        }

        public void Dispose()
        {
            _fhirCacheHelper = null;
        }

        [Fact]
        public void ValidParams_Returns_MongoQuery()
        {
            var searchHelper = new FhirSearchHelper(_fhirCacheHelper);

            var request = searchHelper.BuildQuery(FhirRequests.Valid_Search);

            Assert.NotEqual(FilterDefinition<BsonDocument>.Empty, request);
        }

        [Fact]
        public void NoParams_Returns_EmptyQuery()
        {
            var searchHelper = new FhirSearchHelper(_fhirCacheHelper);

            var request = searchHelper.BuildQuery(FhirRequests.Valid_Search_No_Params);

            Assert.Equal(FilterDefinition<BsonDocument>.Empty, request);
        }

        [Fact]
        public void InvalidParams_Returns_EmptyQuery()
        {
            var searchHelper = new FhirSearchHelper(_fhirCacheHelper);

            var request = searchHelper.BuildQuery(FhirRequests.Invalid_Search_Invalid_Params);

            Assert.Equal(FilterDefinition<BsonDocument>.Empty, request);
        }

    }

}
