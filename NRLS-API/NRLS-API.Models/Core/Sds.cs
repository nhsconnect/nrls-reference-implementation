using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using NRLS_API.Models.ViewModels.Core;
using System;
using System.Collections.Generic;

namespace NRLS_API.Models.Core
{
    public class Sds
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public Guid PartyKey { get; set; }

        public string Fqdn { get; set; }

        public IEnumerable<Uri> EndPoints { get; set; }

        public string OdsCode { get; set; }

        public IEnumerable<string> Interactions { get; set; }

        public long Asid { get; set; }

        public string Thumbprint { get; set; }

        public bool Active { get; set; }

        public static string CacheKey = "Sds";

        public static SdsViewModel ToViewModel(Sds model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), "Cannot map null SdsViewModel");
            }

            var viewModel = new SdsViewModel
            {
                Id = model.Id.ToString(),
                PartyKey = model.PartyKey,
                Fqdn = model.Fqdn,
                EndPoints = model.EndPoints,
                OdsCode = model.OdsCode,
                Interactions = model.Interactions,
                Asid = $"{model.Asid}",
                Thumbprint = model.Thumbprint,
                Active = model.Active
            };

            return viewModel;
        }

    }
}
