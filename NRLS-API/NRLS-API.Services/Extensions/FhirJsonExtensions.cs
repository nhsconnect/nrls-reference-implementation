using Hl7.Fhir.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using NRLS_API.Core.Factories;
using System.Collections.Generic;
using System.Linq;
using SystemTask = System.Threading.Tasks;

namespace NRLS_API.Services.Extensions
{
    public static class FhirJsonExtensions
    {
        public static async SystemTask.Task<List<T>> ToFhirListAsync<T>(this IAsyncCursor<BsonDocument> source) where T : Resource
        {
            var fhirModels = source.ToList().Select(BsonToResourceFactory.ToResource<T>).ToList();

            return await SystemTask.Task.Run(() => fhirModels);
        }

        public static async SystemTask.Task<T> ToFhirAsync<T>(this BsonDocument source) where T : Resource
        {
            var fhirModel = BsonToResourceFactory.ToResource<T>(source);

            return await SystemTask.Task.Run(() => fhirModel);
        }
    }
}
