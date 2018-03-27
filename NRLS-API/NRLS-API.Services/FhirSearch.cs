using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using NRLS_API.Core.Exceptions;
using NRLS_API.Core.Factories;
using NRLS_API.Core.Interfaces.Database;
using NRLS_API.Core.Interfaces.Services;
using NRLS_API.Models.Core;
using NRLS_API.Services.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using static Hl7.Fhir.Model.ModelInfo;

namespace NRLS_API.Services
{
    public class FhirSearch : IFhirSearch
    {

        private readonly INRLSMongoDBContext _context;
        private readonly IList<string> _supportedResources;

        public FhirSearch(IOptions<NrlsApiSetting> nrlsApiSetting, INRLSMongoDBContext context)
        {
            _context = context;
            _supportedResources = nrlsApiSetting.Value.SupportedResources;
        }

        public async Task<Resource> Get<T>(FhirRequest request) where T : Resource
        {
            ValidateResource(request.StrResourceType);

            try
            {
                var builder = Builders<BsonDocument>.Filter;
                var filters = new List<FilterDefinition<BsonDocument>>();
                filters.Add(builder.Eq("_id", request.Id));

                //var options = new FindOptions<BsonDocument, BsonDocument>();
                //options.Sort = Builders<Personnel>.Sort.Ascending(x => x.Name);

                var resource = await _context.Resource(request.StrResourceType).FindSync<BsonDocument>(builder.And(filters)).FirstOrDefault().ToFhirAsync<T>();

                return resource;
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

            try
            {
                var query = BuildQuery(request);

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

        private FilterDefinition<BsonDocument> BuildQuery(FhirRequest request)
        {
            var builder = Builders<BsonDocument>.Filter;
            var filters = new List<FilterDefinition<BsonDocument>>();

            if (request.QueryParameters == null || request.QueryParameters.Count() == 0)
            {
                return FilterDefinition<BsonDocument>.Empty;
            }

            var searchQuery = SearchParams.FromUriParamList(request.QueryParameters);

            var modelParams = SearchParameters.Where(s => s.Resource.Equals(request.ResourceType.ToString()) && request.AllowedParameters.Contains(s.Name));

            foreach (var param in modelParams)
            {
                var criteria = searchQuery.Parameters.FirstOrDefault(x => x.Item1.Equals(param.Name));

                if(criteria == null)
                {
                    continue;
                }

                var paramVal = criteria.Item2;

                if(param.Type.Equals(SearchParamType.Reference) && !string.IsNullOrEmpty(paramVal))
                {
                    //NRLS Hack
                    var paramName = (param.Name?.Equals("patient") == true) ? "subject" : param.Name;

                    filters.Add(builder.Eq($"{paramName}.reference", paramVal));
                }

                if (param.Type.Equals(SearchParamType.Token) && !string.IsNullOrEmpty(paramVal))
                {
                    var isCodeOnly = !paramVal.Contains("|");
                    var sysVal = paramVal.Split('|');

                    //expand these to allow other token types
                    var valType = "value";
                    var sysType = "system";

                    if (sysVal.Length == 1 && isCodeOnly)
                    {
                        filters.Add(builder.Eq($"{param.Name}.{valType}", sysVal.ElementAt(0)));
                    }
                    else if (sysVal.Length == 2)
                    {
                        if (!string.IsNullOrEmpty(sysVal.ElementAt(0)))
                        {
                            filters.Add(builder.Eq($"{param.Name}.{sysType}", sysVal.ElementAt(0)));
                        }

                        if (!string.IsNullOrEmpty(sysVal.ElementAt(1)))
                        {
                            filters.Add(builder.Eq($"{param.Name}.{valType}", sysVal.ElementAt(1)));
                        }
                    }
                }
            }

            if(filters.Count() == 0)
            {
                return FilterDefinition<BsonDocument>.Empty;
            }

            return builder.And(filters);
        }


        private Bundle ToBundle<T>(FhirRequest request, List<T> resources) where T : Resource
        {
            var bundle = new Bundle {
                Id = Guid.NewGuid().ToString(),
                Meta = new Meta
                {
                    LastUpdated = DateTime.UtcNow,
                    VersionId = Guid.NewGuid().ToString()
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
                },
                Entry = resources.Select(r => new Bundle.EntryComponent
                        {
                            Search = new Bundle.SearchComponent
                            {
                                Mode = Bundle.SearchEntryMode.Match
                            },
                            FullUrl = $"{request.RequestUrl.Scheme}://{request.RequestUrl.Host}/fhir/{request.ResourceType}/{r.Id}",
                            Resource = r
                        }).ToList()

            };

            return bundle;
        }

        private void ValidateResource(string resourceType)
        {
            if (_supportedResources.Any() && !_supportedResources.Contains(resourceType))
            {
                throw new HttpFhirException("Bad Request", OperationOutcomeFactory.CreateInvalidResource(resourceType), HttpStatusCode.BadRequest);
            }
        }
    }
}
