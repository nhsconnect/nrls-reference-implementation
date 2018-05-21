using Hl7.Fhir.Model;
using MongoDB.Bson;
using NRLS_API.Core.Factories;
using NRLS_APITest.Comparer;
using Xunit;

namespace NRLS_APITest.Core
{
    public class BsonToResourceFactoryTests
    {
        [Fact]
        public void ToResource_Is_Valid()
        {
            var bsonDocItems = new BsonElement[] { new BsonElement("_id", "5ae838bf461bbbf5792a5460"), new BsonElement("resourceType", "Organization"), new BsonElement("name", "test") };

            var bsonDoc = new BsonDocument();
            bsonDoc.AddRange(bsonDocItems);

            var expected = new Organization
            {
                Id = "5ae838bf461bbbf5792a5460",
                Name = "test"
            };

            var actual = BsonToResourceFactory.ToResource<Organization>(bsonDoc);

            Assert.Equal(expected, actual, Comparers.ModelComparer<Organization>());
        }
    }
}
