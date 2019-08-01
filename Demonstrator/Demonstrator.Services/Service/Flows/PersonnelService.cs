using Demonstrator.Models.DataModels.Flows;
using Demonstrator.Services.Extensions;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Demonstrator.Models.ViewModels.Flows;
using Demonstrator.Core.Interfaces.Database;
using Demonstrator.Core.Interfaces.Services.Flows;

namespace Demonstrator.Services.Service.Flows
{
    public class PersonnelService : IPersonnelService
    {
        private readonly INRLSMongoDBContext _context;

        public PersonnelService(INRLSMongoDBContext context)
        {
            _context = context;
        }

        public async Task<PersonnelViewModel> GetModelBySystemId(string systemId)
        {
            try
            {
                var builder = Builders<Personnel>.Filter;
                var filters = new List<FilterDefinition<Personnel>>();
                filters.Add(builder.Eq(x => x.IsActive, true));
                filters.Add(builder.AnyEq(x => x.SystemIds, systemId));

                var options = new FindOptions<Personnel, Personnel>();
                options.Sort = Builders<Personnel>.Sort.Ascending(x => x.Name);

                var personnel = await _context.Personnel.FindSync(builder.And(filters), options).FirstOrDefaultAsync();
                
                return await personnel.ToViewModelAsync();
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }
    }
}
