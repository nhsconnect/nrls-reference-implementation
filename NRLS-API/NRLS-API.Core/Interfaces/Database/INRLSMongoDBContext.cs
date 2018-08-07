using MongoDB.Bson;
using MongoDB.Driver;

namespace NRLS_API.Core.Interfaces.Database
{
    public interface INRLSMongoDBContext
    {
        IMongoCollection<BsonDocument> Resource(string resourceType);
    }
}
