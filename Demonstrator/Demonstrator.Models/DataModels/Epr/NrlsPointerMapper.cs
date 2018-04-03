using Demonstrator.Models.Core.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Demonstrator.Models.DataModels.Epr
{
    public class NrlsPointerMapper
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string NrlsPointerId { get; set; }

        public string RecordId { get; set; }

        public RecordType RecordType { get; set; }

        public bool IsActive { get; set; }
    }
}
