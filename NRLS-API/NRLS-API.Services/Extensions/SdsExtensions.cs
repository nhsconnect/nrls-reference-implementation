﻿using MongoDB.Driver;
using NRLS_API.Models.Core;
using NRLS_API.Models.ViewModels.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NRLS_API.Services.Extensions
{

    public static class SdsExt
    {
        public static async Task<List<SdsViewModel>> ToViewModelListAsync(this IAsyncCursor<Sds> source)
        {
            var viewModels = source.ToList().Select(Sds.ToViewModel).ToList();

            return await Task.Run(() => viewModels);
        }

        public static async Task<SdsViewModel> ToViewModelAsync(this Sds source)
        {
            var viewModel = Sds.ToViewModel(source);

            return await Task.Run(() => viewModel);
        }
    }
   
}
