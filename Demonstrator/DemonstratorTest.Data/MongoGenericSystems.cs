using Demonstrator.Models.DataModels.Flows;
using MongoDB.Bson;
using System;
using System.Collections.Generic;

namespace DemonstratorTest.Data.Helpers
{
    public static class MongoGenericSystems
    {
        public static IList<GenericSystem> GenericSystems
        {
            get
            {
                return new List<GenericSystem>
                {
                    new GenericSystem
                    {
                        Id = new ObjectId("5a8417338317338c8e0809e5"),
                        Name = "Ambulance Service Call Handler",
                        FModule = "Ambulance_Service_Call_Handler",
                        Asid = "200000000115",
                        Context = "Some context...",
                        IsActive = true,
                        CreatedOn = DateTime.Parse("2018-02-08T10:00:00")
                    }
                };
            }
        }
    }
}
