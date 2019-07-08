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

namespace NRLS_APITest.Services
{
    public class FhirMaintainTests : IDisposable
    {
        IOptionsSnapshot<NrlsApiSetting> _nrlsApiSettings;
        IFhirSearchHelper _fhirSearchHelper;

        public FhirMaintainTests()
        {
            var opts = AppSettings.NrlsApiSettings;
            var settingsMock = new Mock<IOptionsSnapshot<NrlsApiSetting>>();
            settingsMock.Setup(op => op.Get(It.IsAny<string>())).Returns(opts);

            var fhirsearchHelperMock = new Mock<IFhirSearchHelper>();
            fhirsearchHelperMock.Setup(op => op.BuildQuery(FhirRequests.Valid_Search)).Returns(FilterDefinition<BsonDocument>.Empty);
            fhirsearchHelperMock.Setup(op => op.BuildQuery(It.IsAny<string>())).Returns(FilterDefinition<BsonDocument>.Empty);

            _nrlsApiSettings = settingsMock.Object;
            _fhirSearchHelper = fhirsearchHelperMock.Object;
        }

        public void Dispose()
        {
            _nrlsApiSettings = null;
            _fhirSearchHelper = null;
        }

        [Fact]
        public async void Create_Success_Returns_Resource()
        {
            var testBson = new BsonDocument(new BsonElement("_id", new ObjectId("5b7bcc664af1d03816095dac")));

            var collectionMock = new Mock<IMongoCollection<BsonDocument>>();
            collectionMock.Setup(m => m.InsertOneAsync(It.IsAny<BsonDocument>(), null, default(System.Threading.CancellationToken))).Returns(SystemTasks.Task.Run(() => testBson));

            var dbMock = new Mock<INRLSMongoDBContext>();
            dbMock.Setup(op => op.Resource(It.IsAny<string>())).Returns(collectionMock.Object);

            var service = new FhirMaintain(_nrlsApiSettings, dbMock.Object, _fhirSearchHelper);

            var resource = await service.Create<DocumentReference>(FhirRequests.Valid_Create);

            Assert.IsType<DocumentReference>(resource);

        }

        [Fact]
        public async void CreateWithUpdate_SuccessCreateAndUpdate_Returns_Success()
        {
            var testBson = new BsonDocument(new BsonElement("_id", new ObjectId("5b7bcc664af1d03816095dac")));

            var collectionMock = new Mock<IMongoCollection<BsonDocument>>();
            collectionMock.Setup(m => m.InsertOneAsync(It.IsAny<BsonDocument>(), null, default(System.Threading.CancellationToken))).Returns(SystemTasks.Task.Run(() => testBson));
            collectionMock.Setup(m => m.UpdateOneAsync(It.IsAny<FilterDefinition<BsonDocument>>(), It.IsAny<UpdateDefinition<BsonDocument>>(), null, default(System.Threading.CancellationToken))).Returns(SystemTasks.Task.Run(() => MongoModels.Valid_Update as UpdateResult));

            var dbMock = new Mock<INRLSMongoDBContext>();
            dbMock.Setup(op => op.Resource(It.IsAny<string>())).Returns(collectionMock.Object);

            var service = new FhirMaintain(_nrlsApiSettings, dbMock.Object, _fhirSearchHelper);

            var updates = new UpdateDefinitionBuilder<BsonDocument>()
                .Set("status", DocumentReferenceStatus.Superseded.ToString().ToLowerInvariant());

            var result = await service.CreateWithUpdate<DocumentReference>(FhirRequests.Valid_Create, FhirRequests.Valid_Create, updates);

            Assert.IsType<DocumentReference>(result.created);
            Assert.True(result.updated);
        }

        [Fact]
        public async void CreateWithUpdate_SuccessCreateFailedUpdate_Returns_Fail()
        {
            var testBson = new BsonDocument(new BsonElement("_id", new ObjectId("5b7bcc664af1d03816095dac")));

            var collectionMock = new Mock<IMongoCollection<BsonDocument>>();
            collectionMock.Setup(m => m.InsertOneAsync(It.IsAny<BsonDocument>(), null, default(System.Threading.CancellationToken))).Returns(SystemTasks.Task.Run(() => testBson));
            collectionMock.Setup(m => m.UpdateOneAsync(It.IsAny<FilterDefinition<BsonDocument>>(), It.IsAny<UpdateDefinition<BsonDocument>>(), null, default(System.Threading.CancellationToken))).Returns(SystemTasks.Task.Run(() => MongoModels.Invalid_Update as UpdateResult));
            collectionMock.Setup(m => m.DeleteOneAsync(It.IsAny<FilterDefinition<BsonDocument>>(), null, default(System.Threading.CancellationToken))).Returns(SystemTasks.Task.Run(() => MongoModels.Valid_Delete as DeleteResult));

            var dbMock = new Mock<INRLSMongoDBContext>();
            dbMock.Setup(op => op.Resource(It.IsAny<string>())).Returns(collectionMock.Object);

            var service = new FhirMaintain(_nrlsApiSettings, dbMock.Object, _fhirSearchHelper);

            var updates = new UpdateDefinitionBuilder<BsonDocument>()
                .Set("status", DocumentReferenceStatus.Superseded.ToString().ToLowerInvariant());

            var result = await service.CreateWithUpdate<DocumentReference>(FhirRequests.Valid_Create, FhirRequests.Valid_Create, updates);

            Assert.Null(result.created);
            Assert.False(result.updated);
        }

        [Fact]
        public void Create_Failure_Throws_Exception()
        {
            var testBson = new BsonDocument(new BsonElement("_id", new ObjectId("5b7bcc664af1d03816095dac")));

            var collectionMock = new Mock<IMongoCollection<BsonDocument>>();
            collectionMock.Setup(m => m.InsertOneAsync(It.IsAny<BsonDocument>(), null, default(System.Threading.CancellationToken))).Returns(SystemTasks.Task.Run(() => testBson));

            var dbMock = new Mock<INRLSMongoDBContext>();
            dbMock.Setup(op => op.Resource(It.IsAny<string>())).Returns(collectionMock.Object);

            var service = new FhirMaintain(_nrlsApiSettings, dbMock.Object, _fhirSearchHelper);


            Assert.ThrowsAsync<Exception>(async () =>
            {
                var resource = await service.Create<DocumentReference>(FhirRequests.Invalid_Create_NoDocument);
            });

        }

        [Fact]
        public async void Update_Success_Returns_True()
        {
            var collectionMock = new Mock<IMongoCollection<BsonDocument>>();
            collectionMock.Setup(m => m.UpdateOneAsync(It.IsAny<FilterDefinition<BsonDocument>>(), It.IsAny<UpdateDefinition<BsonDocument>>(), null, default(System.Threading.CancellationToken))).Returns(SystemTasks.Task.Run(() => MongoModels.Valid_Update as UpdateResult));

            var dbMock = new Mock<INRLSMongoDBContext>();
            dbMock.Setup(op => op.Resource(It.IsAny<string>())).Returns(collectionMock.Object);

            var service = new FhirMaintain(_nrlsApiSettings, dbMock.Object, _fhirSearchHelper);

            var updates = new UpdateDefinitionBuilder<BsonDocument>()
                .Set("status", DocumentReferenceStatus.Superseded.ToString().ToLowerInvariant());

            var resource = await service.Update<DocumentReference>(FhirRequests.Valid_Create, updates);

            Assert.True(resource);
        }

        [Fact]
        public async void Update_NoRecordUpdated_Returns_False()
        {
            var collectionMock = new Mock<IMongoCollection<BsonDocument>>();
            collectionMock.Setup(m => m.UpdateOneAsync(It.IsAny<FilterDefinition<BsonDocument>>(), It.IsAny<UpdateDefinition<BsonDocument>>(), null, default(System.Threading.CancellationToken))).Returns(SystemTasks.Task.Run(() => MongoModels.Invalid_Update as UpdateResult));

            var dbMock = new Mock<INRLSMongoDBContext>();
            dbMock.Setup(op => op.Resource(It.IsAny<string>())).Returns(collectionMock.Object);

            var service = new FhirMaintain(_nrlsApiSettings, dbMock.Object, _fhirSearchHelper);

            var updates = new UpdateDefinitionBuilder<BsonDocument>()
                .Set("status", DocumentReferenceStatus.Superseded.ToString().ToLowerInvariant());

            var resource = await service.Update<DocumentReference>(FhirRequests.Valid_Create, updates);

            Assert.False(resource);
        }

        [Fact]
        public async void Delete_Success_Returns_True()
        {

            var collectionMock = new Mock<IMongoCollection<BsonDocument>>();
            collectionMock.Setup(m => m.DeleteOneAsync(It.IsAny<FilterDefinition<BsonDocument>>(), null, default(System.Threading.CancellationToken))).Returns(SystemTasks.Task.Run(() => MongoModels.Valid_Delete as DeleteResult));

            var dbMock = new Mock<INRLSMongoDBContext>();
            dbMock.Setup(op => op.Resource(It.IsAny<string>())).Returns(collectionMock.Object);

            var service = new FhirMaintain(_nrlsApiSettings, dbMock.Object, _fhirSearchHelper);

            var result = await service.Delete<DocumentReference>(FhirRequests.Valid_Delete_Path_Id);

            Assert.True(result);
        }

        [Fact]
        public async void Delete_NoRecordDeleted_Returns_False()
        {
            var collectionMock = new Mock<IMongoCollection<BsonDocument>>();
            collectionMock.Setup(m => m.DeleteOneAsync(It.IsAny<FilterDefinition<BsonDocument>>(), null, default(System.Threading.CancellationToken))).Returns(SystemTasks.Task.Run(() => MongoModels.Invalid_Delete as DeleteResult));

            var dbMock = new Mock<INRLSMongoDBContext>();
            dbMock.Setup(op => op.Resource(It.IsAny<string>())).Returns(collectionMock.Object);

            var service = new FhirMaintain(_nrlsApiSettings, dbMock.Object, _fhirSearchHelper);

            var result = await service.Delete<DocumentReference>(FhirRequests.Invalid_Delete_NotFound);

            Assert.False(result);
        }

        //Conditional Delete
        //Checks are in NrlsMaintain so nothing really to test here
        //General delete tests above cover other scenarios
    }
}
