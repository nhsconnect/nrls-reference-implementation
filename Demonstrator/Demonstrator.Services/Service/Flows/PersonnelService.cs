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
    public class PersonnelService : IPersonnelService
    {
        private readonly INRLSMongoDBContext _context;

        public PersonnelService(INRLSMongoDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PersonnelViewModel>> GetAll()
        {
            try
            {
                var builder = Builders<Personnel>.Filter;
                var filters = new List<FilterDefinition<Personnel>>();
                filters.Add(builder.Eq(x => x.IsActive, true));

                var options = new FindOptions<Personnel, Personnel>();
                options.Sort = Builders<Personnel>.Sort.Ascending(x => x.Name);

                return await _context.Personnel.FindSync(builder.And(filters), options).ToViewModelListAsync();
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        public async Task<PersonnelViewModel> GetById(string personnelId)
        {
            try
            {
                var personnel = GetModelById(personnelId).Result;

                return await personnel.ToViewModelAsync();
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        public async Task<Personnel> GetModelById(string personnelId)
        {
            try
            {
                var builder = Builders<Personnel>.Filter;
                var filters = new List<FilterDefinition<Personnel>>();
                filters.Add(builder.Eq(x => x.IsActive, true));
                filters.Add(builder.Eq(x => x.Id, new ObjectId(personnelId)));

                var options = new FindOptions<Personnel, Personnel>();
                options.Sort = Builders<Personnel>.Sort.Ascending(x => x.Name);

                return await _context.Personnel.FindSync(builder.And(filters), options).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }
    }
}
