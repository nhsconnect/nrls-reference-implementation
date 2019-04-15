﻿using Demonstrator.Models.DataModels.Flows;
using Demonstrator.Models.DataModels.Epr;
using MongoDB.Driver;
using Demonstrator.Models.DataModels.Base;

namespace Demonstrator.Core.Interfaces.Database
{
    public interface INRLSMongoDBContext
    {
        IMongoCollection<ActorOrganisation> ActorOrganisations { get; }

        IMongoCollection<GenericSystem> GenericSystems { get; }

        IMongoCollection<Personnel> Personnel { get; }

        IMongoCollection<NrlsPointerMap> NrlsPointerMaps { get; }

        IMongoCollection<CrisisPlan> CrisisPlans { get; }

        IMongoCollection<Benefit> Benefits { get; }

        IMongoCollection<Sds> Sds { get; }
    }
}
