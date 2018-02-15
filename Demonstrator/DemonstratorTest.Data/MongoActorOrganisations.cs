using Demonstrator.Models.DataModels.Flows;
using MongoDB.Bson;
using System;
using System.Collections.Generic;

namespace DemonstratorTest.Data.Helpers
{
    public static class MongoActorOrganisations
    {
        public static IList<ActorOrganisation> ActorOrganisations
        {
            get
            {
                return new List<ActorOrganisation>
                {
                    new ActorOrganisation
                    {
                        Id = new ObjectId("5a82f9ffcb969daa58d33377"),
                        Type = "Consumer",
                        Name = "Ambulance Service",
                        ImageUrl = "....",
                        Context = "Some general context....",
                        IsActive = true,
                        CreatedOn = DateTime.Parse("2018-02-08T10:00:00"),
                        OrgCode = "AMSR01"
                    }
                };
            }
        }
    }
}
