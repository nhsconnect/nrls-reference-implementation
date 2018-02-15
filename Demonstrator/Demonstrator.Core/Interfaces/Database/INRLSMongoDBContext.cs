using Demonstrator.Models.DataModels.Flows;
using MongoDB.Driver;

namespace Demonstrator.Core.Interfaces.Database
{
    public interface INRLSMongoDBContext
    {
        IMongoCollection<ActorOrganisation> ActorOrganisations { get; }

        IMongoCollection<GenericSystem> GenericSystems { get; }

        IMongoCollection<Personnel> Personnel { get; }
    }
}
