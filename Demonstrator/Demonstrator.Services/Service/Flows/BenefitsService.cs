using Demonstrator.Models.DataModels.Flows;
using Demonstrator.Services.Extensions;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Demonstrator.Models.ViewModels.Flows;
using MongoDB.Bson;
using Demonstrator.Core.Interfaces.Database;
using Demonstrator.Core.Interfaces.Services.Flows;
using System.Linq;

namespace Demonstrator.Services.Service.Flows
{
    public class BenefitsService : IBenefitsService
    {
        private readonly INRLSMongoDBContext _context;

        public BenefitsService(INRLSMongoDBContext context)
        {
            _context = context;
        }

        public async Task<List<BenefitViewModel>> GetByIdList(IList<string> idList)
        {
            try
            {

                var builder = Builders<Benefit>.Filter;
                var filters = new List<FilterDefinition<Benefit>>();
                filters.Add(builder.Eq(x => x.IsActive, true));
                filters.Add(builder.In(x => x.Id, idList.Select(i => new ObjectId(i))));

                var options = new FindOptions<Benefit, Benefit>();
                options.Sort = Builders<Benefit>.Sort.Ascending(x => x.Order);

                return await _context.Benefits.FindSync(builder.And(filters), options).ToViewModelListAsync();
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }
    }
}
