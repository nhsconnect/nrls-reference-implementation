using Demonstrator.Models.Core.Enums;
using Demonstrator.Models.DataModels.Epr;
using MongoDB.Bson;
using System.Threading.Tasks;

namespace Demonstrator.Core.Interfaces.Services.Epr
{
    public interface IPointerMapService
    {
        Task<NrlsPointerMap> FindPointerMap(string recordId, RecordType recordType);

        void CreatePointerMap(string pointerId, ObjectId recordId, RecordType recordType);
    }
}
