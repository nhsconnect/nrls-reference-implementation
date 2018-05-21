using MongoDB.Bson;
using MongoDB.Driver;
using NRLS_API.Core.Helpers;
using NRLS_APITest.Data;
using Xunit;

namespace NRLS_APITest.Core.Helpers
{
    public class FhirSearchHelperTests
    {
        [Fact]
        public void ValidParams_Returns_MongoQuery()
        {

            var request = FhirSearchHelper.BuildQuery(FhirRequests.Valid_Search);

            Assert.NotEqual(FilterDefinition<BsonDocument>.Empty, request);
        }

        [Fact]
        public void NoParams_Returns_EmptyQuery()
        {

            var request = FhirSearchHelper.BuildQuery(FhirRequests.Valid_Search_No_Params);

            Assert.Equal(FilterDefinition<BsonDocument>.Empty, request);
        }

        [Fact]
        public void InvalidParams_Returns_EmptyQuery()
        {

            var request = FhirSearchHelper.BuildQuery(FhirRequests.Invalid_Search_Invalid_Params);

            Assert.Equal(FilterDefinition<BsonDocument>.Empty, request);
        }

    }

}
