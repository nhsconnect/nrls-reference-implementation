using Demonstrator.Models.DataModels.Flows;
using Demonstrator.Services.Extensions;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Demonstrator.Models.ViewModels.Flows;
using Demonstrator.Core.Interfaces.Database;
using Demonstrator.Core.Interfaces.Services.Flows;
using MongoDB.Bson;

namespace Demonstrator.Services.Service.Flows
{
    public class ActorOrganisationService : IActorOrganisationService
    {
        private readonly INRLSMongoDBContext _context;

        public ActorOrganisationService(INRLSMongoDBContext context)
        {
            _context = context;
        }

        public async Task<ActorOrganisationViewModel> GetById(string orgId)
        {
            try
            {
                var actorOrganisation = GetModelById(orgId).Result;

                return await actorOrganisation.ToViewModelAsync();
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        public async Task<ActorOrganisation> GetModelById(string orgId)
        {
            try
            {
                var builder = Builders<ActorOrganisation>.Filter;
                var filters = new List<FilterDefinition<ActorOrganisation>>();
                filters.Add(builder.Eq(x => x.IsActive, true));
                filters.Add(builder.Eq(x => x.Id, new ObjectId(orgId)));

                var options = new FindOptions<ActorOrganisation, ActorOrganisation>();
                options.Sort = Builders<ActorOrganisation>.Sort.Ascending(x => x.Name);

                return await _context.ActorOrganisations.FindSync(builder.And(filters), options).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

    }
}
