using Demonstrator.Models.Core.Enums;
using Demonstrator.Models.Core.Models;
using Demonstrator.Models.DataModels.Flows;
using Demonstrator.Services.Extensions;
using Microsoft.Extensions.Options;
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
    public class ActorOrganisationService : IActorOrganisationService
    {
        private readonly INRLSMongoDBContext _context;

        public ActorOrganisationService(INRLSMongoDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ActorOrganisationViewModel>> GetAll(ActorType aoType)
        {
            try
            {
                var builder = Builders<ActorOrganisation>.Filter;
                var filters = new List<FilterDefinition<ActorOrganisation>>();
                filters.Add(builder.Eq(x => x.Type, aoType.ToString()));
                filters.Add(builder.Eq(x => x.IsActive, true));

                var options = new FindOptions<ActorOrganisation, ActorOrganisation>();
                options.Sort = Builders<ActorOrganisation>.Sort.Ascending(x => x.Name);

                return await _context.ActorOrganisations.FindSync(builder.And(filters), options).ToViewModelListAsync();
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }
    }
}
