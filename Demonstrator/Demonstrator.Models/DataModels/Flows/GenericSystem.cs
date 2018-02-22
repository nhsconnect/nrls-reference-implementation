using Demonstrator.Models.Core.Enums;
using Demonstrator.Models.ViewModels.Flows;
using Demonstrator.Utilities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Demonstrator.Models.DataModels.Flows
{
    public class GenericSystem
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string Name { get; set; }

        public string FModule { get; set; }

        public string Asid { get; set; }

        public string Context { get; set; }

        public bool IsActive { get; set; }

        public List<string> ActionTypes { get; set; }

        public DateTime CreatedOn { get; set; }

        public static GenericSystemViewModel ToViewModel(GenericSystem model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), "Cannot map null GenericSystem");
            }

            var viewModel = new GenericSystemViewModel
            {
                Id = model.Id.ToString(),
                Context = model.Context,
                Asid = model.Asid,
                FModule = model.FModule,
                ActionTypes = model.ActionTypes.Select(x => EnumHelpers.GetEnum<ActorType>(x)).ToList(),
                Name = model.Name
             };

            return viewModel;
        }
    }
}
