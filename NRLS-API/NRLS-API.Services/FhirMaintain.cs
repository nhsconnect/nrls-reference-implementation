using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using NRLS_API.Core.Factories;
using NRLS_API.Core.Interfaces.Database;
using NRLS_API.Core.Interfaces.Services;
using NRLS_API.Models.Core;
using System;
using System.Collections.Generic;
using SystemTasks = System.Threading.Tasks;


namespace NRLS_API.Services
{
    public class FhirMaintain : FhirBase, IFhirMaintain
    {

        private readonly INRLSMongoDBContext _context;
      
        public FhirMaintain(IOptions<NrlsApiSetting> nrlsApiSetting, INRLSMongoDBContext context) : base(nrlsApiSetting)
        {
            _context = context;
        }

        public async SystemTasks.Task<Resource> Create<T>(FhirRequest request) where T : Resource
        {
            ValidateResource(request.StrResourceType);

            try
            {
                //At present NRLS spec states updates are performed by delete and create so version will always be 1
                request.Resource.VersionId = "1";

                request.Resource.Meta = request.Resource.Meta ?? new Meta();
                request.Resource.Meta.LastUpdated = DateTime.UtcNow;
                request.Resource.Meta.VersionId = "1";

                var pointerJson = new FhirJsonSerializer().SerializeToString(request.Resource);

                var document = BsonSerializer.Deserialize<BsonDocument>(pointerJson);

                _context.Resource(request.StrResourceType).InsertOne(document);

                request.Resource.Id = document.GetElement("_id").Value.ToString();


                return await SystemTasks.Task.Run(() => request.Resource);
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        public async SystemTasks.Task<OperationOutcome> Delete<T>(FhirRequest request) where T : Resource
        {
            ValidateResource(request.StrResourceType);

            try
            {
                // IMPORTANT - In reality the NRLS will only soft delete pointers but not actually delete them like we are doing here
                var builder = Builders<BsonDocument>.Filter;
                var filters = new List<FilterDefinition<BsonDocument>>();
                filters.Add(builder.Eq("_id", new ObjectId(request.Id)));

                var deleted = await _context.Resource(request.StrResourceType).DeleteOneAsync(builder.And(filters));

                OperationOutcome outcome;
                if(deleted.IsAcknowledged && deleted.DeletedCount > 0)
                {
                    outcome = OperationOutcomeFactory.CreateDelete(request.RequestUrl.AbsoluteUri);
                }
                else
                {
                    outcome = OperationOutcomeFactory.CreateNotFound(request.Id);
                }

                return await SystemTasks.Task.Run(() => outcome);

            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }


    }
}
