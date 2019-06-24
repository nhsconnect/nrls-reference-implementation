using MongoDB.Driver;
using NRLS_API.Models.Core;
using NRLS_API.Models.ViewModels.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NRLS_API.Core.Interfaces.Database
{
    public interface INRLSMongoDBCaller
    {
        Task<IEnumerable<SdsViewModel>> FindSds(FilterDefinition<Sds> filter);
    }
}
