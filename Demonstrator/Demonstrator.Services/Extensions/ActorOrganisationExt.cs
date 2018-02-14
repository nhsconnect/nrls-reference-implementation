using Demonstrator.Models.Core.Enums;
using Demonstrator.Models.DataModels.Flows;
using Demonstrator.Models.ViewModels.Flows;
using Demonstrator.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Demonstrator.Services.Extensions
{
    public static class ActorOrganisationExt
    {
        public static async Task<IEnumerable<ActorOrganisationViewModel>> ToViewModelAsync(this Task<List<ActorOrganisation>> models)
        {
            var viewModels = new List<ActorOrganisationViewModel>();

            foreach (var model in models.Result)
            {
                viewModels.Add(new ActorOrganisationViewModel
                {
                    Id = model.Id.ToString(),
                    Context = model.Context,
                    ImageUrl = model.ImageUrl,
                    Name = model.Name,
                    OrgCode = model.OrgCode,
                    Type = EnumHelpers.GetEnum<ActorType>(model.Type)
                });
            }

            return await Task.Run(() => viewModels);

        }
    }
}
