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
            filters.Add(builder.Eq(x => x.IsActive, true));

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

        public async Task<bool> DeletePointerMap(ObjectId id)
        {
            //Find and delete Pointer Map
            var update = new UpdateDefinitionBuilder<NrlsPointerMap>().Set(n => n.IsActive, false);
            var deleted = _context.NrlsPointerMaps.UpdateOne(item => item.Id == id, update);

            return await Task.Run(() => deleted.IsAcknowledged && deleted.ModifiedCount > 0);

        }
    }
}
