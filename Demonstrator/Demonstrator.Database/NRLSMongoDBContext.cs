using Demonstrator.Models.Core.Models;
using Demonstrator.Models.DataModels.Flows;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Demonstrator.Database
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

        public IMongoCollection<ActorOrganisation> ActorOrganisations
        {
            get
            {
                return _database.GetCollection<ActorOrganisation>("ActorOrganisation");
            }
        }
    }
}
