using Demonstrator.Models.DataModels.Base;
using Demonstrator.Models.ViewModels.Base;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Demonstrator.Core.Interfaces.Database
{
    public interface INRLSMongoDBCaller
    {
        Task<IEnumerable<SdsViewModel>> FindSds(FilterDefinition<Sds> filter);
    }
}
