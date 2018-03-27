using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using MongoDB.Bson;

namespace NRLS_API.Core.Factories
{
    public class BsonToResourceFactory
    {
        public static T ToResource<T>(BsonDocument bsonDocument) where T : Resource
        {
            var id = bsonDocument.GetElement("_id");
            bsonDocument.Remove("_id");

            var json = bsonDocument.ToJson();
            var resource = new FhirJsonParser().Parse<T>(json);
            resource.Id = id.Value.ToString();

            return resource;
        }
    }
}
