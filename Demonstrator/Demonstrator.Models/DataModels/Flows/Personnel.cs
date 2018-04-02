using Demonstrator.Models.ViewModels.Flows;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Demonstrator.Models.DataModels.Flows
{
    public class Personnel
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string Name { get; set; }

        public string PersonnelType { get; set; }

        public string ImageUrl { get; set; }

        public string Context { get; set; }

        public bool UsesNrls { get; set; }

        public List<string> SystemIds { get; set; }

        public string OrganisationId { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedOn { get; set; }

        public IList<string> Benefits { get; set; }

        public string BenefitsTitle { get; set; }

        public static PersonnelViewModel ToViewModel(Personnel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), "Cannot map null Personnel");
            }

            var viewModel = new PersonnelViewModel
            {
                Id = model.Id.ToString(),
                Context = model.Context,
                ImageUrl = model.ImageUrl,
                Name = model.Name,
                PersonnelType = model.PersonnelType,
                ActorOrganisationId = model.OrganisationId,
                SystemIds = model.SystemIds,
                UsesNrls = model.UsesNrls,
                Benefits = model.Benefits
            };

            return viewModel;
        }
    }
}
