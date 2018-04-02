using Demonstrator.Models.DataModels.Flows;
using MongoDB.Bson;
using System.Collections.Generic;

namespace DemonstratorTest.Data.Helpers
{
    public static class MongoBenefit
    {
        public static IList<Benefit> Benefits
        {
            get
            {
                return new List<Benefit>
                {
                    new Benefit
                    {
                        Id = new ObjectId("5a8417f68317338c8e080a62"),
                        Text = "Benefit",
                        Categories = new List<string>{ "Test" },
                        IsActive = true,
                        Order = 1,
                        Type = "Test"
                    }
                };
            }
        }

    }
}
