using Hl7.Fhir.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using NRLS_API.Models.Core;
using System;
using System.Collections.Generic;

namespace NRLS_API.Core.Interfaces.Helpers
{
    public interface IFhirSearchHelper
    {
        Resource GetResourceProfile(string profileUrl);

        Bundle ToBundle<T>(FhirRequest request, List<T> resources, Guid? bundleId = null) where T : Resource;

        FilterDefinition<BsonDocument> BuildQuery(string _id);

        FilterDefinition<BsonDocument> BuildQuery(FhirRequest request);
    }
}
