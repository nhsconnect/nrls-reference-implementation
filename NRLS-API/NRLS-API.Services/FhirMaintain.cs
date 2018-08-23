using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using NRLS_API.Core.Exceptions;
using NRLS_API.Core.Factories;
using NRLS_API.Core.Interfaces.Database;
using NRLS_API.Core.Interfaces.Helpers;
using NRLS_API.Core.Interfaces.Services;
using NRLS_API.Models.Core;
using System;
using SystemTasks = System.Threading.Tasks;


namespace NRLS_API.Services
{
    public class FhirMaintain : FhirBase, IFhirMaintain
    {

        private readonly INRLSMongoDBContext _context;
        private readonly IFhirSearchHelper _fhirSearchHelper;


        public FhirMaintain(IOptionsSnapshot<ApiSetting> nrlsApiSetting, INRLSMongoDBContext context, IFhirSearchHelper fhirSearchHelper) : base(nrlsApiSetting, "NrlsApiSetting")
        {
            _context = context;
            _fhirSearchHelper = fhirSearchHelper;
        }

        public async SystemTasks.Task<Resource> Create<T>(FhirRequest request) where T : Resource
        {
            ValidateResource(request.StrResourceType);

            try
            {
                var pointerJson = new FhirJsonSerializer().SerializeToString(request.Resource);

                var document = BsonSerializer.Deserialize<BsonDocument>(pointerJson);

                await _context.Resource(request.StrResourceType).InsertOneAsync(document);

                BsonElement documentId;
                var hasId = document.TryGetElement("_id", out documentId);
                request.Resource.Id = documentId.Value?.ToString();

                return await SystemTasks.Task.Run(() => request.Resource);
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        public async SystemTasks.Task<(Resource created, bool updated)> CreateWithUpdate<T>(FhirRequest request, FhirRequest updateRequest, UpdateDefinition<BsonDocument> updates) where T : Resource
        {
 
            Resource created = null;
            bool updated = false;

            //Can only do transactional style rollbacks in MongoDB 4.0 with replication
            try
            {
                created = await Create<T>(request);

                if (created != null)
                {
                    updated = await Update<T>(updateRequest, updates);

                    //If update failed revert create
                    if (!updated)
                    {
                        var deleteRequest = FhirRequest.Create(created.Id, created.ResourceType);
                        await Delete<T>(deleteRequest);

                        created = null;
                    }
                }

            }
            catch(Exception ex)
            {
                // log or manage the exception
                throw ex;
            }

            return (created: created, updated: updated);
        }

        public async SystemTasks.Task<bool> Update<T>(FhirRequest request, UpdateDefinition<BsonDocument> updates) where T : Resource
        {
            var filter = _fhirSearchHelper.BuildIdQuery(request.Id);

            try
            {
                var previous = await _context.Resource(request.StrResourceType).UpdateOneAsync(filter, updates);

                return await SystemTasks.Task.Run(() => previous.IsAcknowledged && previous.ModifiedCount > 0);
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }

        }

        public async SystemTasks.Task<bool> Delete<T>(FhirRequest request) where T : Resource
        {
            var filter = _fhirSearchHelper.BuildIdQuery(request.Id);

            return await DeleteResource<T>(request, filter);
        }

        public async SystemTasks.Task<bool> DeleteConditional<T>(FhirRequest request) where T : Resource
        {
            // IMPORTANT - this query currently does not filter for active/un-deleted pointers
            var filters = _fhirSearchHelper.BuildQuery(request);

            return await DeleteResource<T>(request, filters);
        }       

        private async SystemTasks.Task<bool> DeleteResource<T>(FhirRequest request, FilterDefinition<BsonDocument> filters) where T : Resource
        {
            ValidateResource(request.StrResourceType);

            try
            {
                // IMPORTANT - In reality the NRLS will only soft delete pointers but not actually delete them like we are doing here
                var deleted =  await _context.Resource(request.StrResourceType).DeleteOneAsync(filters, null, default(System.Threading.CancellationToken));

                return await SystemTasks.Task.Run(() => (deleted.IsAcknowledged && deleted.DeletedCount > 0));

            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }


    }
}
