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
using System.Collections.Generic;
using SystemTasks = System.Threading.Tasks;
using Xunit;
using NRLS_APITest.Comparer;

namespace NRLS_APITest.Services
{
    public class FhirMaintainTests
    {
        [Fact]
        public async void Create_Valid()
        {
            var opts = AppSettings.NrlsApiSettings;

            var settingsMock = new Mock<IOptionsSnapshot<NrlsApiSetting>>();
            settingsMock.Setup(op => op.Get(It.IsAny<string>())).Returns(opts);

            var collectionMock = new Mock<IMongoCollection<BsonDocument>>();
            collectionMock.Setup(m => m.InsertOne(It.IsAny<BsonDocument>(), null, default(System.Threading.CancellationToken)));

            var dbMock = new Mock<INRLSMongoDBContext>();
            dbMock.Setup(op => op.Resource(It.IsAny<string>())).Returns(collectionMock.Object);

            var fhirsearchHelperMock = new Mock<IFhirSearchHelper>();
            fhirsearchHelperMock.Setup(op => op.BuildQuery(FhirRequests.Valid_Search)).Returns(FilterDefinition<BsonDocument>.Empty);

            var service = new FhirMaintain(settingsMock.Object, dbMock.Object, fhirsearchHelperMock.Object);

            var resource = await service.Create<DocumentReference>(FhirRequests.Valid_Create);

            Assert.Equal("1", resource.VersionId);

            Assert.NotNull(resource.Meta);

            Assert.Equal("1", resource.Meta.VersionId);
        }

        [Fact]
        public async void Delete_Valid()
        {
            var opts = AppSettings.NrlsApiSettings;

            var settingsMock = new Mock<IOptionsSnapshot<NrlsApiSetting>>();
            settingsMock.Setup(op => op.Get(It.IsAny<string>())).Returns(opts);

            var collectionMock = new Mock<IMongoCollection<BsonDocument>>();
            collectionMock.Setup(m => m.DeleteOneAsync(It.IsAny<FilterDefinition<BsonDocument>>(), null, default(System.Threading.CancellationToken))).Returns(SystemTasks.Task.Run(() => MongoModels.Valid_Delete as DeleteResult));

            var dbMock = new Mock<INRLSMongoDBContext>();
            dbMock.Setup(op => op.Resource(It.IsAny<string>())).Returns(collectionMock.Object);

            var fhirsearchHelperMock = new Mock<IFhirSearchHelper>();
            fhirsearchHelperMock.Setup(op => op.BuildQuery(FhirRequests.Valid_Search)).Returns(FilterDefinition<BsonDocument>.Empty);

            var service = new FhirMaintain(settingsMock.Object, dbMock.Object, fhirsearchHelperMock.Object);

            var resource = await service.Delete<DocumentReference>(FhirRequests.Valid_Delete);

            var expected = OperationOutcomes.Deleted;

            Assert.Equal(expected, resource, Comparers.ModelComparer<OperationOutcome>());
        }

        [Fact]
        public async void Delete_Invalid()
        {
            var opts = AppSettings.NrlsApiSettings;

            var settingsMock = new Mock<IOptionsSnapshot<NrlsApiSetting>>();
            settingsMock.Setup(op => op.Get(It.IsAny<string>())).Returns(opts);

            var collectionMock = new Mock<IMongoCollection<BsonDocument>>();
            collectionMock.Setup(m => m.DeleteOneAsync(It.IsAny<FilterDefinition<BsonDocument>>(), null, default(System.Threading.CancellationToken))).Returns(SystemTasks.Task.Run(() => MongoModels.Invalid_Delete as DeleteResult));

            var dbMock = new Mock<INRLSMongoDBContext>();
            dbMock.Setup(op => op.Resource(It.IsAny<string>())).Returns(collectionMock.Object);

            var fhirsearchHelperMock = new Mock<IFhirSearchHelper>();
            fhirsearchHelperMock.Setup(op => op.BuildQuery(FhirRequests.Valid_Search)).Returns(FilterDefinition<BsonDocument>.Empty);

            var service = new FhirMaintain(settingsMock.Object, dbMock.Object, fhirsearchHelperMock.Object);

            var resource = await service.Delete<DocumentReference>(FhirRequests.Valid_Delete);

            var expected = OperationOutcomes.NotFound;

            Assert.Equal(expected, resource, Comparers.ModelComparer<OperationOutcome>());
        }
    }
}
