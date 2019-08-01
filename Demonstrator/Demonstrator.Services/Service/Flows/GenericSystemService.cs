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

namespace Demonstrator.Services.Service.Flows
{
    public class GenericSystemService : IGenericSystemService
    {
        private readonly INRLSMongoDBContext _context;

        public GenericSystemService(INRLSMongoDBContext context)
        {
            _context = context;
        }

        public async Task<GenericSystemViewModel> GetById(string systemId)
        {
            try
            {
                var builder = Builders<GenericSystem>.Filter;
                var filters = new List<FilterDefinition<GenericSystem>>();
                filters.Add(builder.Eq(x => x.IsActive, true));
                filters.Add(builder.Eq(x => x.Id, new ObjectId(systemId)));

                var options = new FindOptions<GenericSystem, GenericSystem>();
                options.Sort = Builders<GenericSystem>.Sort.Ascending(x => x.Name);

                return await _context.GenericSystems.FindSync(builder.And(filters), options).FirstOrDefault().ToViewModelAsync();
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        public async Task<IEnumerable<GenericSystemViewModel>> GetAll()
        {

            try
            {
                var builder = Builders<GenericSystem>.Filter;
                var filters = new List<FilterDefinition<GenericSystem>>();
                filters.Add(builder.Eq(x => x.IsActive, true));

                var options = new FindOptions<GenericSystem, GenericSystem>();
                options.Sort = Builders<GenericSystem>.Sort.Ascending(x => x.Name);

                return await _context.GenericSystems.FindSync(builder.And(filters), options).ToViewModelListAsync();
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }
    }
}
