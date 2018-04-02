using Demonstrator.Models.DataModels.Flows;
using Demonstrator.Models.ViewModels.Flows;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demonstrator.Services.Extensions
{
    public static class BenefitExt
    {
        public static async Task<List<BenefitViewModel>> ToViewModelListAsync(this IAsyncCursor<Benefit> source)
        {
            var viewModels = source.ToList().Select(Benefit.ToViewModel).ToList();

            return await Task.Run(() => viewModels);
        }

        public static async Task<BenefitViewModel> ToViewModelAsync(this Benefit source)
        {
            var viewModel = Benefit.ToViewModel(source);

            return await Task.Run(() => viewModel);
        }
    }
}
