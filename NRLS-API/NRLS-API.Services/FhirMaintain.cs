using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using NRLS_API.Core.Helpers;
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

            //TODO change to validation service
            //var profileHelper = new ProfileHelper(); 
            //profileHelper.ValidProfile(request.Resource, "https://fhir.nhs.uk/STU3/StructureDefinition/NRLS-DocumentReference-1");

            try
            {
                //validate pointer
                request.Resource.VersionId = "1";

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

        public async SystemTasks.Task<bool> Delete<T>(FhirRequest request) where T : Resource
        {
            ValidateResource(request.StrResourceType);

            try
            {
                // IMPORTANT - In reality the NRLS will only soft delete pointers but not actually delete them like we are doing here
                var builder = Builders<BsonDocument>.Filter;
                var filters = new List<FilterDefinition<BsonDocument>>();
                filters.Add(builder.Eq("_id", new ObjectId(request.Id)));

                var deleted = await _context.Resource(request.StrResourceType).DeleteOneAsync(builder.And(filters));

                return await SystemTasks.Task.Run(() => deleted.IsAcknowledged && deleted.DeletedCount > 0);

            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }


    }
}
