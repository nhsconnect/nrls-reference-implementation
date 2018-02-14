using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Demonstrator.Models.DataModels.Flows
{
    public class Personnel
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string Name { get; set; }

        public string ImageUrl { get; set; }

        public string Context { get; set; }

        public bool UsesNrls { get; set; }

        public string SystemId { get; set; }

        public string OrganisationId { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
