using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Demonstrator.Models.DataModels.Patients
{
    public class CarePlan
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public bool InvolveFamilyOrCarer { get; set; }

        public string SignsFeelingUnwell { get; set; }

        public string PotentialTriggers { get; set; }

        public string WhatHelpsInCrisis { get; set; }

        public string EmergencyLocation { get; set; }

        public string EmergencyNumber { get; set; }

        public string CrisisNumber { get; set; }

        public bool PatientAcceptsPlan { get; set; }

        public DateTime PlanCreated { get; set; }

        public DateTime PlanUpdated { get; set; }

        public bool Active { get; set; }
    }
}
