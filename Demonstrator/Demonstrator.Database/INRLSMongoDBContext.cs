using Demonstrator.Models.DataModels.Flows;
using MongoDB.Driver;

namespace Demonstrator.Database
{
    public interface INRLSMongoDBContext
    {
        IMongoCollection<ActorOrganisation> ActorOrganisations { get; }
    }
}
