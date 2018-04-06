using Demonstrator.Core.Interfaces.Database;
using Demonstrator.Core.Interfaces.Services.Epr;
using Demonstrator.Models.Core.Enums;
using Demonstrator.Models.DataModels.Epr;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Demonstrator.Services.Service.Epr
{
    public class PointerMapService : IPointerMapService
    {
        private readonly INRLSMongoDBContext _context;

        public PointerMapService(INRLSMongoDBContext context)
        {
            _context = context;
        }

        public async Task<NrlsPointerMap> FindPointerMap(string recordId, RecordType recordType)
        {
            var builder = Builders<NrlsPointerMap>.Filter;
            var filters = new List<FilterDefinition<NrlsPointerMap>>();

            filters.Add(builder.Eq(x => x.RecordId, recordId));
            filters.Add(builder.Eq(x => x.RecordType, recordType.ToString()));

            return await _context.NrlsPointerMaps.FindSync(builder.And(filters), null).FirstOrDefaultAsync();
        }

        public void CreatePointerMap(string pointerId, ObjectId recordId, RecordType recordType)
        {
            var pointerMapper = new NrlsPointerMap
            {
                IsActive = true,
                NrlsPointerId = pointerId,
                RecordId = recordId.ToString(),
                RecordType = recordType.ToString()
            };

            _context.NrlsPointerMaps.InsertOne(pointerMapper);
        }
    }
}
