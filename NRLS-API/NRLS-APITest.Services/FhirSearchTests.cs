using Hl7.Fhir.Model;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using NRLS_API.Core.Interfaces.Database;
using NRLS_API.Core.Interfaces.Helpers;
using NRLS_API.Models.Core;
using NRLS_API.Services;
using NRLS_APITest.Data;
using SystemTasks = System.Threading.Tasks;
using Xunit;
using System;
using System.Collections.Generic;
using NRLS_APITest.StubClasses;

namespace NRLS_APITest.Services
{
    public class FhirSearchTests : IDisposable
    {
        IOptionsSnapshot<NrlsApiSetting> _nrlsApiSettings;
        IFhirSearchHelper _fhirSearchHelper;
        INRLSMongoDBCaller _nrlsMongoDBCaller;

        public FhirSearchTests()
        {
            var opts = AppSettings.NrlsApiSettings;
            var settingsMock = new Mock<IOptionsSnapshot<NrlsApiSetting>>();
            settingsMock.Setup(op => op.Get(It.IsAny<string>())).Returns(opts);

            var list = new List<DocumentReference> { NrlsPointers.Valid, NrlsPointers.Valid_With_Alt_Custodian };

            var bundle = FhirBundle.GetBundle(list);

            var fhirsearchHelperMock = new Mock<IFhirSearchHelper>();
            fhirsearchHelperMock.Setup(op => op.BuildQuery(FhirRequests.Valid_Search)).Returns(FilterDefinition<BsonDocument>.Empty);
            fhirsearchHelperMock.Setup(op => op.BuildQuery(It.IsAny<string>())).Returns(FilterDefinition<BsonDocument>.Empty);
            fhirsearchHelperMock.Setup(op => op.ToBundle<DocumentReference>(It.IsAny<FhirRequest>(), It.IsAny<List<DocumentReference>>(), It.IsAny<Guid?>())).Returns(bundle);

            _nrlsApiSettings = settingsMock.Object;
            _fhirSearchHelper = fhirsearchHelperMock.Object;

            IEnumerable<BsonDocument> testBsons = new List<BsonDocument> { MongoModels.BsonDocumentReferenceA, MongoModels.BsonDocumentReferenceB };

            var nrlsMongoDBCaller = new Mock<INRLSMongoDBCaller>();
            nrlsMongoDBCaller.Setup(m => m.FindResource(It.IsAny<string>(), It.IsAny<FilterDefinition<BsonDocument>>())).Returns(SystemTasks.Task.Run(() => MongoStubs.AsyncCursor(testBsons)));

            _nrlsMongoDBCaller = nrlsMongoDBCaller.Object;
        }

        public void Dispose()
        {
            _nrlsApiSettings = null;
            _fhirSearchHelper = null;
            _nrlsMongoDBCaller = null;
        }

        [Fact]
        public async void Find_Success_Bundle()
        {
            //var testBson = new BsonDocument(new BsonElement("_id", new ObjectId("5b7bcc664af1d03816095dac")));

            //var collectionMock = new Mock<IMongoCollection<BsonDocument>>();
            //collectionMock.Setup(m => m.InsertOneAsync(It.IsAny<BsonDocument>(), null, default(System.Threading.CancellationToken))).Returns(SystemTasks.Task.Run(() => testBson));

            //var dbMock = new Mock<INRLSMongoDBContext>();
            //dbMock.Setup(op => op.Resource(It.IsAny<string>())).Returns(collectionMock.Object);

            var service = new FhirSearch(_nrlsApiSettings, _nrlsMongoDBCaller, _fhirSearchHelper);

            var resource = await service.Find<DocumentReference>(FhirRequests.Valid_Create, false);

            Assert.IsType<Bundle>(resource);

            Assert.Equal(2, resource.Total);
            Assert.Equal(2, resource.Entry.Count);

        }

        [Fact]
        public async void Find_Success_Bundle_Single()
        {
            var service = new FhirSearch(_nrlsApiSettings, _nrlsMongoDBCaller, _fhirSearchHelper);

            var resource = await service.Find<DocumentReference>(FhirRequests.Valid_Create, true);

            Assert.IsType<Bundle>(resource);

            Assert.Equal(1, resource.Total);
            Assert.Single(resource.Entry);

        }

        [Fact]
        public async void Get_Success()
        {

            var service = new FhirSearch(_nrlsApiSettings, _nrlsMongoDBCaller, _fhirSearchHelper);

            var resource = await service.Get<DocumentReference>(FhirRequests.Valid_Read);

            Assert.NotNull(resource);
            Assert.IsType<DocumentReference>(resource);

        }

    }
}
