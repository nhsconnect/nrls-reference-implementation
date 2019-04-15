using MongoDB.Bson;
using MongoDB.Driver;
using NRLS_API.Models.Core;

namespace NRLS_API.Core.Interfaces.Database
{
    public interface INRLSMongoDBContext
    {
        IMongoCollection<BsonDocument> Resource(string resourceType);

        IMongoCollection<Sds> Sds { get; }

    }
}
