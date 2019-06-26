using Hl7.Fhir.Model;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using NRLS_API.Core.Interfaces.Database;
using NRLS_API.Core.Interfaces.Helpers;
using NRLS_API.Core.Interfaces.Services;
using NRLS_API.Models.Core;
using NRLS_API.Services.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NRLS_API.Services
{
    public class FhirSearch : FhirBase, IFhirSearch
    {
        private readonly INRLSMongoDBContext _context;
        private readonly IFhirSearchHelper _fhirSearchHelper;

        public FhirSearch(IOptionsSnapshot<ApiSetting> apiSetting, INRLSMongoDBContext context, IFhirSearchHelper fhirSearchHelper) : base(apiSetting, "NrlsApiSetting")
        {
            _context = context;
            _fhirSearchHelper = fhirSearchHelper;
        }

        public async Task<Resource> GetAsBundle<T>(FhirRequest request) where T : Resource
        {

            try
            {
                var resource = await Get<T>(request);

                var documents = new List<DocumentReference>();

                if (resource != null)
                {
                    documents.Add(resource as DocumentReference);
                }

                //Get now returns bundle as per updated spec
                var bundle = ToBundle(request, documents);

                return bundle;
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        public async Task<Resource> Get<T>(FhirRequest request) where T : Resource
        {
            ValidateResource(request.StrResourceType);

            var filter = _fhirSearchHelper.BuildIdQuery(request.Id);

            try
            {
                //var options = new FindOptions<BsonDocument, BsonDocument>();
                //options.Sort = Builders<Personnel>.Sort.Ascending(x => x.Name);

                var resource = await _context.Resource(request.StrResourceType).FindSync<BsonDocument>(filter).FirstOrDefaultAsync();

                Resource document = await resource?.ToFhirAsync<T>();

                return document;
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        public async Task<Resource> GetByMasterId<T>(FhirRequest request) where T : Resource
        {
            ValidateResource(request.StrResourceType);

            //validate request

            try
            {
                //Add identifier on the fly as it is not a standard search parameter
                request.AllowedParameters = request.AllowedParameters.Concat(new[] { "identifier" }).ToArray();

                // IMPORTANT - this query currently does not filter for active/un-deleted pointers
                var query = _fhirSearchHelper.BuildQuery(request);

                //var options = new FindOptions<BsonDocument, BsonDocument>();
                //options.Sort = Builders<Personnel>.Sort.Ascending(x => x.Name);

                var resource = await _context.Resource(request.StrResourceType).FindSync<BsonDocument>(query).FirstOrDefaultAsync();

                Resource document;

                var documents = new List<DocumentReference>();

                if (resource != null)
                {
                    document = await resource?.ToFhirAsync<T>();
                    documents.Add(document as DocumentReference);
                }

                //Get now returns bundle as per updated spec
                var bundle = ToBundle(request, documents);

                return bundle;
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        public async Task<Resource> Find<T>(FhirRequest request) where T : Resource
        {
            ValidateResource(request.StrResourceType);

            //validate request

            try
            {
                // IMPORTANT - this query currently does not filter for active/un-deleted pointers
                var query = _fhirSearchHelper.BuildQuery(request);

                var resources = await _context.Resource(request.StrResourceType).FindSync<BsonDocument>(query).ToFhirListAsync<T>();

                var bundle = ToBundle(request, resources);

                return bundle;
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }      


        private Bundle ToBundle<T>(FhirRequest request, List<T> resources) where T : Resource
        {
            var bundle = new Bundle {
                Id = Guid.NewGuid().ToString(),
                Meta = new Meta
                {
                    LastUpdated = DateTime.UtcNow
                    //VersionId = Guid.NewGuid().ToString()
                    //TODO: check if this requires http://fhir.nhs.net/StructureDefinition/nrls-searchset-bundle-1-0
                },
                Type = Bundle.BundleType.Searchset, 
                Total = resources.Count,
                Link = new List<Bundle.LinkComponent>
                {
                    new Bundle.LinkComponent
                    {
                        Relation = "_self",
                        Url = request.RequestUrl.AbsoluteUri
                    }
                }
            };

            if (!request.IsSummary)
            {
                bundle.Entry = resources.Select(r => new Bundle.EntryComponent
                {
                    Search = new Bundle.SearchComponent
                    {
                        Mode = Bundle.SearchEntryMode.Match
                    },
                    FullUrl = $"{request.RequestUrl.Scheme}://{request.RequestUrl.Host}/fhir/{request.ResourceType}/{r.Id}",
                    Resource = r
                }).ToList();
            }

            return bundle;
        }


    }
}
