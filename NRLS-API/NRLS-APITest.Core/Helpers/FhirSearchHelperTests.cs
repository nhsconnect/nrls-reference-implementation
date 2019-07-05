using Hl7.Fhir.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using NRLS_API.Core.Exceptions;
using NRLS_API.Core.Helpers;
using NRLS_API.Core.Interfaces.Helpers;
using NRLS_API.Core.Resources;
using NRLS_APITest.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace NRLS_APITest.Core.Helpers
{
    public class FhirSearchHelperTests : IDisposable
    {
        private IFhirResourceHelper _fhirResourceHelper;

        public FhirSearchHelperTests()
        {
            var mockCacheHelper = new Mock<IFhirResourceHelper>();
            mockCacheHelper.Setup(op => op.GetResourceProfile(It.Is<string>(s => s.Equals(FhirConstants.SystemNrlsProfile)))).Returns(FhirResources.SD_NrlsPointer);

            _fhirResourceHelper = mockCacheHelper.Object;
        }

        public void Dispose()
        {
            _fhirResourceHelper = null;
        }

        [Fact]
        public void ValidParams_Returns_MongoQuery()
        {
            var searchHelper = new FhirSearchHelper(_fhirResourceHelper);

            var request = searchHelper.BuildQuery(FhirRequests.Valid_Search);

            Assert.NotEqual(FilterDefinition<BsonDocument>.Empty, request);
        }

        [Fact]
        public void NoParams_Returns_EmptyQuery()
        {
            var searchHelper = new FhirSearchHelper(_fhirResourceHelper);

            var request = searchHelper.BuildQuery(FhirRequests.Valid_Search_No_Params);

            Assert.Equal(FilterDefinition<BsonDocument>.Empty, request);
        }

        [Fact]
        public void InvalidParams_Returns_EmptyQuery()
        {
            var searchHelper = new FhirSearchHelper(_fhirResourceHelper);

            var request = searchHelper.BuildQuery(FhirRequests.Invalid_Search_Invalid_Params);

            Assert.Equal(FilterDefinition<BsonDocument>.Empty, request);
        }

        [Fact]
        public void ValidDeleteParams_Returns_MongoQuery()
        {
            var searchHelper = new FhirSearchHelper(_fhirResourceHelper);

            var request = searchHelper.BuildQuery(FhirRequests.Valid_ConditionalDelete);

            Assert.NotEqual(FilterDefinition<BsonDocument>.Empty, request);
        }

        [Fact]
        public void InvalidDeleteParams_Returns_EmptyQuery()
        {
            var searchHelper = new FhirSearchHelper(_fhirResourceHelper);

            var request = searchHelper.BuildQuery(FhirRequests.Invalid_ConditionalDelete_NoSearchValues);

            Assert.Equal(FilterDefinition<BsonDocument>.Empty, request);
        }

        [Fact]
        public void Valid_Id_Returns_ObjectId()
        {
            var searchHelper = new FhirSearchHelper(_fhirResourceHelper);

            var filter = searchHelper.BuildQuery("5b7bcc664af1d03816095dac");

            Assert.IsAssignableFrom<FilterDefinition<BsonDocument>>(filter);
        }

        [Fact]
        public void Invalid_Id_Throws_Exception()
        {
            var searchHelper = new FhirSearchHelper(_fhirResourceHelper);


            Assert.Throws<HttpFhirException>(() =>
            {
                var filter = searchHelper.BuildQuery("badId");
            });
        }


        [Fact]
        public void Valid_ToBundle()
        {
            var searchHelper = new FhirSearchHelper(_fhirResourceHelper);

            var list = new List<DocumentReference> { NrlsPointers.Valid };
            var bundleId = Guid.NewGuid();

            var bundle = searchHelper.ToBundle(FhirRequests.Valid_Read, list, bundleId);

            Assert.IsType<Bundle>(bundle);
            Assert.Equal(bundleId.ToString(), bundle.Id);

            Assert.NotNull(bundle.Meta);
            Assert.NotNull(bundle.Meta.LastUpdated);
            Assert.Equal(Bundle.BundleType.Searchset, bundle.Type);
            Assert.Equal(1, bundle.Total);
            Assert.NotNull(bundle.Link);
            Assert.Single(bundle.Link);

            var firstLink = bundle.Link.First();
            Assert.Equal("https://testdomain/testurl", firstLink.Url);
            Assert.Equal("_self", firstLink.Relation);

            Assert.NotNull(bundle.Entry);
            Assert.Single(bundle.Entry);

            var firstEntry = bundle.Entry.First();
            Assert.NotNull(firstEntry.Search);
            Assert.Equal(Bundle.SearchEntryMode.Match, firstEntry.Search.Mode);
            Assert.Equal("https://testdomain/nrls-ri/DocumentReference/5ab13f41957d0ad5d93a1339", firstEntry.FullUrl);
            Assert.NotNull(firstEntry.Resource);
            Assert.Equal(ResourceType.DocumentReference, firstEntry.Resource.ResourceType);
            Assert.Equal("5ab13f41957d0ad5d93a1339", firstEntry.Resource.Id);
        }

        [Fact]
        public void Valid_ToSummaryBundle()
        {
            var searchHelper = new FhirSearchHelper(_fhirResourceHelper);

            var list = new List<DocumentReference> { NrlsPointers.Valid };

            var request = FhirRequests.Valid_Read;
            request.IsSummary = true;

            var bundle = searchHelper.ToBundle(request, list, null);

            Assert.IsType<Bundle>(bundle);
            Assert.NotNull(bundle.Id);
 
            Assert.NotNull(bundle.Entry);
            Assert.Empty(bundle.Entry);

        }

    }

}
