using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Demonstrator.Models.DataModels.Epr
{
    public class MedicalRecord
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public int Version { get; set; }

        public string RecordType { get; set; }

        public bool Active { get; set; }

        public string PatientNhsNumber { get; set; }
    }
}
