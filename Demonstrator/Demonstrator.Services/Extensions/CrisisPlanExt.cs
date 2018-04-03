using Demonstrator.Models.DataModels.Epr;
using Demonstrator.Models.ViewModels.Epr;
using System.Threading.Tasks;

namespace Demonstrator.Services.Extensions
{
    public static class CrisisPlanExt
    {
        public static async Task<CrisisPlanViewModel> ToViewModelAsync(this CrisisPlan source)
        {
            var viewModel = CrisisPlan.ToViewModel(source);

            return await Task.Run(() => viewModel);
        }
    }
}
