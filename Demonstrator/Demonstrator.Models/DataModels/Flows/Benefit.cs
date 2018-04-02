using Demonstrator.Models.ViewModels.Flows;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Demonstrator.Models.DataModels.Flows
{
    public class Benefit
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string Text { get; set; }

        public IList<string> Categories { get; set; }

        public int Order { get; set; }

        public string Type { get; set; }

        public bool IsActive { get; set; }

        public static BenefitViewModel ToViewModel(Benefit model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), "Cannot map null Benefit");
            }

            var viewModel = new BenefitViewModel
            {
                Id = model.Id.ToString(),
                Text = model.Text,
                Categories = model.Categories,
                Order = model.Order
            };

            return viewModel;
        }
    }
}
