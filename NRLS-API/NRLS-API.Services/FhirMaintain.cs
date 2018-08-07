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
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

        //public async SystemTasks.Task<(Resource created, bool updated)> CreateWithUpdateTransaction<T>(FhirRequest request, FhirRequest updateRequest, UpdateDefinition<BsonDocument> updates) where T : Resource
        //{
        //    var result = await _context.StartTransactionWithRetry(async (IClientSessionHandle session, Action<IClientSessionHandle> commitTransaction) =>
        //    {
        //        Resource created = null;
        //        bool updated = false;

        //        session.StartTransaction(new TransactionOptions(readConcern: ReadConcern.Snapshot, writeConcern: WriteConcern.WMajority));

        //        try
        //        {
        //            created = await Create<T>(request);
        //            updated = await Update<T>(updateRequest, updates);
        //        }
        //        catch
        //        {
        //            session.AbortTransaction();
        //            throw new HttpFhirException("Error Updating DocumentReference", OperationOutcomeFactory.CreateInternalError($"There has been an internal error when attempting to persist the DocumentReference. Please contact the national helpdesk quoting - {Guid.NewGuid()}"));
        //        }

        //        if(created == null || !updated)
        //        {
        //            session.AbortTransaction();
        //            return (created: created, updated: updated);
        //        }

        //        commitTransaction(session);
        //        return (created: created, updated: updated);
        //    });

        //    return result;
        //}

        public async SystemTasks.Task<OperationOutcome> Delete<T>(FhirRequest request) where T : Resource
        {
            ObjectId id;
            if (!ObjectId.TryParse(request.Id, out id))
            {
                throw new HttpFhirException("Invalid _id parameter", OperationOutcomeFactory.CreateInvalidParameter("Invalid parameter: _id"), HttpStatusCode.BadRequest);
            }

            var builder = Builders<BsonDocument>.Filter;
            var filters = new List<FilterDefinition<BsonDocument>>();
            filters.Add(builder.Eq("_id", id));

            return await DeleteResource<T>(request, builder.And(filters));
        }

        public async SystemTasks.Task<OperationOutcome> DeleteConditional<T>(FhirRequest request) where T : Resource
        {
            //Add identifier on the fly as it is not a standard search parameter
            request.AllowedParameters = request.AllowedParameters.Concat(new[] { "identifier" }).ToArray();

            // IMPORTANT - this query currently does not filter for active/un-deleted pointers
            var filters = _fhirSearchHelper.BuildQuery(request);

            return await DeleteResource<T>(request, filters);
        }

        private async SystemTasks.Task<OperationOutcome> DeleteResource<T>(FhirRequest request, FilterDefinition<BsonDocument> filters) where T : Resource
        {
            ValidateResource(request.StrResourceType);

            try
            {
                // IMPORTANT - In reality the NRLS will only soft delete pointers but not actually delete them like we are doing here
                var deleted =  await _context.Resource(request.StrResourceType).DeleteOneAsync(filters, null, default(System.Threading.CancellationToken));

                OperationOutcome outcome;
                if (deleted.IsAcknowledged && deleted.DeletedCount > 0)
                {
                    outcome = OperationOutcomeFactory.CreateDelete(request.RequestUrl.AbsoluteUri, request.AuditId);
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
