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
                var benefits = new List<Benefit>();
                benefits.AddRange(EfficiencyBenefits);
                benefits.AddRange(HealthBenefits);
                benefits.AddRange(SafteyBenefits);

                return benefits;
            }
        }

        public static IList<Benefit> EfficiencyBenefits
        {
            get
            {
                return new List<Benefit>
                {
                    new Benefit
                    {
                        Id = new ObjectId("5a8417f68317338c8e080a62"),
                        Text = "Benefit E1",
                        Categories = new List<string>{ "Efficiency" },
                        IsActive = true,
                        Order = 1,
                        Type = "Test"
                    },
                    new Benefit
                    {
                        Id = new ObjectId("5a8417f68317338c8e080a63"),
                        Text = "Benefit E2",
                        Categories = new List<string>{ "Efficiency" },
                        IsActive = false,
                        Order = 2,
                        Type = "Test"
                    }
                };
            }
        }

        public static IList<Benefit> SafteyBenefits
        {
            get
            {
                return new List<Benefit>
                {
                    new Benefit
                    {
                        Id = new ObjectId("5a8417f68317338c8e080a64"),
                        Text = "Benefit S1",
                        Categories = new List<string>{ "Saftey" },
                        IsActive = true,
                        Order = 1,
                        Type = "Test"
                    },
                    new Benefit
                    {
                        Id = new ObjectId("5a8417f68317338c8e080a65"),
                        Text = "Benefit S2",
                        Categories = new List<string>{ "Saftey" },
                        IsActive = true,
                        Order = 2,
                        Type = "Test"
                    }
                };
            }
        }

        public static IList<Benefit> HealthBenefits
        {
            get
            {
                return new List<Benefit>
                {
                    new Benefit
                    {
                        Id = new ObjectId("5a8417f68317338c8e080a66"),
                        Text = "Benefit H1",
                        Categories = new List<string>{ "Health" },
                        IsActive = false,
                        Order = 1,
                        Type = "Test"
                    }
                };
            }
        }

    }
}
