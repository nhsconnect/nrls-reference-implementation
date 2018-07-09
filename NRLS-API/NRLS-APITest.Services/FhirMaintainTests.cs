using Hl7.Fhir.Model;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using NRLS_API.Core.Interfaces.Database;
using NRLS_API.Models.Core;
using NRLS_API.Services;
using NRLS_APITest.Data;
using Xunit;

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

            var service = new FhirMaintain(settingsMock.Object, dbMock.Object);

            var resource = await service.Create<DocumentReference>(FhirRequests.Valid_Create);

            Assert.Equal("1", resource.VersionId);

            Assert.NotNull(resource.Meta);

            Assert.Equal("1", resource.Meta.VersionId);
        }
    }
}
