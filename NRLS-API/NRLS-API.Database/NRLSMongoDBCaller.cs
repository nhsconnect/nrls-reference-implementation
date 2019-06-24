using MongoDB.Driver;
using NRLS_API.Core.Interfaces.Database;
using NRLS_API.Models.Core;
using NRLS_API.Models.ViewModels.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NRLS_API.Database
{
    public class NRLSMongoDBCaller : INRLSMongoDBCaller
    {
        private readonly INRLSMongoDBContext _context;

        public NRLSMongoDBCaller(INRLSMongoDBContext context)
        {
            _context = context;
        }

        //Not here to test Mongo so creating this wrapper to avoid Moq extension errors with FindAsync
        public async Task<IEnumerable<SdsViewModel>> FindSds(FilterDefinition<Sds> filter)
        {

            var entries = await _context.Sds.FindAsync(filter);

            var viewModels = entries.ToList().Select(Sds.ToViewModel).ToList();

            return viewModels;
        }


    }
}
