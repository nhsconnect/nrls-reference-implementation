using Demonstrator.Models.Core.Enums;
using Demonstrator.Models.Core.Models;
using Demonstrator.Models.ViewModels.Flows;
using Demonstrator.Utilities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Demonstrator.Models.DataModels.Flows
{
    public class ActorOrganisation
    {
        public ActorOrganisation()
        {
            Context = new List<ContentView>();
            Benefits = new List<string>();
        }

        [BsonId]
        public ObjectId Id { get; set; }

        public string Type { get; set; }

        public string Name { get; set; }

        public string ImageUrl { get; set; }

        public IList<ContentView> Context { get; set; }

        public string OrgCode { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedOn { get; set; }

        public IList<string> Benefits { get; set; }

        public string BenefitsTitle { get; set; }

        public static ActorOrganisationViewModel ToViewModel(ActorOrganisation model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), "Cannot map null ActorOrganisation");
            }

            var viewModel = new ActorOrganisationViewModel
            {
                Id = model.Id.ToString(),
                Context = model.Context.OrderBy(x => x.Order).ToList(),
                ImageUrl = model.ImageUrl,
                Name = model.Name,
                OrgCode = model.OrgCode,
                Type = EnumHelpers.GetEnum<ActorType>(model.Type),
                Benefits = model.Benefits
            };

            return viewModel;
        }
    }
}
