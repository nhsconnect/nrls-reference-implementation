using Demonstrator.Models.DataModels.Flows;
using Demonstrator.Models.ViewModels.Flows;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demonstrator.Services.Extensions
{
    public static class PersonnelExt
    {
        public static async Task<List<PersonnelViewModel>> ToViewModelListAsync(this IAsyncCursor<Personnel> source)
        {
            var viewModels = source.ToList().Select(Personnel.ToViewModel).ToList();

            return await Task.Run(() => viewModels);
        }

        public static async Task<PersonnelViewModel> ToViewModelAsync(this Personnel source)
        {
            var viewModel = Personnel.ToViewModel(source);

            return await Task.Run(() => viewModel);
        }
    }
}
