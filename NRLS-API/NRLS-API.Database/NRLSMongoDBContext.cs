using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using NRLS_API.Core.Interfaces.Database;
using NRLS_API.Models;

namespace NRLS_API.Database
{
    public class NRLSMongoDBContext : INRLSMongoDBContext
    {
        private readonly IMongoDatabase _database = null;

        public NRLSMongoDBContext(IOptions<DbSetting> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            if (client != null)
                _database = client.GetDatabase(settings.Value.Database);
        }

        public IMongoCollection<BsonDocument> Resource(string resourceType)
        {
            return _database.GetCollection<BsonDocument>(resourceType);
        }

    }
}
