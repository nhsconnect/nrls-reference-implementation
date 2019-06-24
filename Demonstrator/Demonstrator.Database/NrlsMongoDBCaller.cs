using Demonstrator.Core.Interfaces.Database;
using Demonstrator.Models.DataModels.Base;
using Demonstrator.Models.ViewModels.Base;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demonstrator.Database
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
