using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Demonstrator.Models.DataModels.Flows
{
    public class GenericSystem
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string Name { get; set; }

        public string FModule { get; set; }

        public string Asid { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
