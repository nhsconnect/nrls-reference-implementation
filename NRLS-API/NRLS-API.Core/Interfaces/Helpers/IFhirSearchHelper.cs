using Hl7.Fhir.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using NRLS_API.Models.Core;

namespace NRLS_API.Core.Interfaces.Helpers
{
    public interface IFhirSearchHelper
    {
        Resource GetResourceProfile(string profileUrl);

        FilterDefinition<BsonDocument> BuildIdQuery(string _id);

        FilterDefinition<BsonDocument> BuildQuery(FhirRequest request);
    }
}
