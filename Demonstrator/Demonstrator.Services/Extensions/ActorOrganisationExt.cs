using Demonstrator.Models.DataModels.Flows;
using Demonstrator.Models.ViewModels.Flows;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demonstrator.Services.Extensions
{
    public static class ActorOrganisationExt
    {
        public static async Task<List<ActorOrganisationViewModel>> ToViewModelListAsync(this IAsyncCursor<ActorOrganisation> source)
        {
            var viewModels = source.ToList().Select(ActorOrganisation.ToViewModel).ToList();

            return await Task.Run(() => viewModels);
        }
    }
}
