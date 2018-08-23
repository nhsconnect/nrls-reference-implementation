using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using MongoDB.Bson;
using MongoDB.Driver;
using NRLS_API.Core.Exceptions;
using NRLS_API.Core.Factories;
using NRLS_API.Core.Interfaces.Helpers;
using NRLS_API.Models.Core;
using System.Collections.Generic;
using System.Linq;
using static Hl7.Fhir.Model.ModelInfo;

namespace NRLS_API.Core.Helpers
{
    public class FhirSearchHelper : IFhirSearchHelper
    {
        private readonly IFhirCacheHelper _fhirCacheHelper;

        public FhirSearchHelper(IFhirCacheHelper fhirCacheHelper)
        {
            _fhirCacheHelper = fhirCacheHelper;
        }

        public Resource GetResourceProfile(string profileUrl)
        {
            return _fhirCacheHelper.GetResourceProfile(profileUrl);
        }

        public FilterDefinition<BsonDocument> BuildIdQuery(string _id)
        {
            //validate request
            ObjectId id;
            if (!ObjectId.TryParse(_id, out id))
            {
                throw new HttpFhirException("Invalid _id parameter");
            }

            var builder = Builders<BsonDocument>.Filter;
            var filters = new List<FilterDefinition<BsonDocument>>();
            filters.Add(builder.Eq("_id", id));

            return builder.And(filters);
        }

        public FilterDefinition<BsonDocument> BuildQuery(FhirRequest request)
        {
            var builder = Builders<BsonDocument>.Filter;
            var filters = new List<FilterDefinition<BsonDocument>>();

            if (request.QueryParameters == null || request.QueryParameters.Count() == 0)
            {
                return FilterDefinition<BsonDocument>.Empty;
            }

            var searchQuery = SearchParams.FromUriParamList(request.QueryParameters);

            var modelParams = SearchParameters.Where(s => s.Resource.Equals(request.StrResourceType) && request.AllowedParameters.Contains(s.Name));

            StructureDefinition profile = null;

            if (!string.IsNullOrEmpty(request.ProfileUri))
            {
                try
                {
                    profile = (StructureDefinition)GetResourceProfile(request.ProfileUri);
                }
                catch
                {
                    throw new HttpFhirException("Failed to parse fhir type for search query.", OperationOutcomeFactory.CreateInternalError($"Failed to parse fhir type of {request.StrResourceType} for search query."));
                }
            }



            foreach (var param in modelParams)
            {
                var paramName = param.Name;

                var criteria = searchQuery.Parameters.FirstOrDefault(x => (!x.Item1.Contains(".") && x.Item1.Equals(paramName)) || (x.Item1.Contains(".") && x.Item1.StartsWith(paramName)));

                if (criteria == null)
                {
                    continue;
                }

                var paramDef = profile?.Snapshot?.Element.FirstOrDefault(e => e.Path.Equals($"{request.StrResourceType}.{paramName}"));

                var paramVal = criteria.Item2;

                if (param.Type.Equals(SearchParamType.Reference) && !string.IsNullOrEmpty(paramVal))
                {
                    filters.Add(builder.Eq($"{paramName}.reference", paramVal));
                    continue;
                }

                if (param.Type.Equals(SearchParamType.Token) && !string.IsNullOrEmpty(paramVal))
                {
                    var isCodeOnly = !paramVal.Contains("|");
                    var sysVal = paramVal.Split('|');

                    //expand these to allow other token types
                    var valType = "value";
                    var sysType = "system";
                    var arrayPath = "";

                    //extend for other types
                    if(paramDef != null && paramDef.Type.FirstOrDefault(t => t.Code.Equals(FHIRAllTypes.CodeableConcept.ToString())) != null)
                    {
                        valType = "code";

                        var tokenCodingDef = profile.Snapshot?.Element.FirstOrDefault(e => e.Path.Equals($"{request.StrResourceType}.{paramName}.coding"));

                        if (tokenCodingDef == null)
                        {
                            continue;
                        }

                        int? minVal = tokenCodingDef.Min;
                        string maxVal = tokenCodingDef.Max;

                        if (tokenCodingDef.Base != null)
                        {
                            minVal = tokenCodingDef.Base.Min;
                            maxVal = tokenCodingDef.Base.Max;
                        }

                        int max = 0;
                        var isMaxint = !string.IsNullOrWhiteSpace(maxVal) && int.TryParse(maxVal, out max);

                        //Assuming tokenCodingDef is a CodeableConcept.coding element
                        if (minVal.HasValue && minVal.Value >= 0 && (maxVal.Equals("*") || isMaxint && max > 0))
                        {
                            arrayPath = ".coding";
                        }
                    }

                    // NRLS Hack - NRLS allows masterIdentifier but not identifier document element
                    // Could extend this to create an OR for masterIdentifier || identifier
                    if (request.StrResourceType.Equals(ResourceType.DocumentReference.ToString()) && paramName.Equals("identifier"))
                    {
                        paramName = "masterIdentifier";
                    }


                    if (sysVal.Length == 1 && isCodeOnly)
                    {
                        if (string.IsNullOrEmpty(arrayPath))
                        {
                            filters.Add(builder.Eq($"{paramName}.{valType}", sysVal.ElementAt(0)));
                        }
                        else
                        {
                            filters.Add(builder.ElemMatch($"{paramName}{arrayPath}", builder.Eq(valType, sysVal.ElementAt(0))));
                        }
                        continue;
                    }
                    else if (sysVal.Length == 2)
                    {

                        FilterDefinition<BsonDocument> sysValFilter = null;

                        if (!string.IsNullOrEmpty(sysVal.ElementAt(0)))
                        {

                            if (string.IsNullOrEmpty(arrayPath))
                            {
                                filters.Add(builder.Eq($"{paramName}.{sysType}", sysVal.ElementAt(0)));
                            }
                            else
                            {
                                sysValFilter = builder.Eq(sysType, sysVal.ElementAt(0));

                            }
                        }

                        if (!string.IsNullOrEmpty(sysVal.ElementAt(1)))
                        {
                            if (string.IsNullOrEmpty(arrayPath))
                            {
                                filters.Add(builder.Eq($"{paramName}.{valType}", sysVal.ElementAt(1)));
                            }
                            else
                            {
                                sysValFilter = sysValFilter & builder.Eq(valType, sysVal.ElementAt(1));
                            }
                        }

                        if(!string.IsNullOrEmpty(arrayPath) && sysValFilter != null)
                        {
                            filters.Add(builder.ElemMatch($"{paramName}{arrayPath}", sysValFilter));
                        }

                        continue;
                    }
                }
            }

            if (filters.Count() == 0)
            {
                return FilterDefinition<BsonDocument>.Empty;
            }

            return builder.And(filters);
        }
    }
}
