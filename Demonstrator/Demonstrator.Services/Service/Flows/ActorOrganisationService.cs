using Demonstrator.Database;
using Demonstrator.Models.Core.Enums;
using Demonstrator.Models.Core.Models;
using Demonstrator.Models.DataModels.Flows;
using Demonstrator.Services.Interface.Flows;
using Demonstrator.Services.Extensions;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Demonstrator.Models.ViewModels.Flows;
using MongoDB.Bson;

namespace Demonstrator.Services.Service.Flows
{
    public class ActorOrganisationService : IActorOrganisationService
    {
        private readonly INRLSMongoDBContext _context;

        public ActorOrganisationService(INRLSMongoDBContext context, IOptions<DbSetting> settings)
        {
            _context = context;
        }

        public async Task<IEnumerable<ActorOrganisationViewModel>> GetAll(ActorType aoType)
        {
            try
            {
                var sort = Builders<ActorOrganisation>.Sort.Ascending(t => t.Name);
                var filters = new BsonDocument
                {
                    {
                        "Type",
                        aoType.ToString()
                    },
                    {
                        "IsActive",
                        "true"
                    }
                };

                return await _context.ActorOrganisations
                    .FindSync(filters, 
                              new FindOptions<ActorOrganisation, ActorOrganisation>()
                              {
                                Sort = sort
                              })
                    .ToListAsync()
                    .ToViewModelAsync();
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }
    }
}
