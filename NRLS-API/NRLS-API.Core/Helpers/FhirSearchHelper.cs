using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using MongoDB.Bson;
using MongoDB.Driver;
using NRLS_API.Models.Core;
using System.Collections.Generic;
using System.Linq;
using static Hl7.Fhir.Model.ModelInfo;

namespace NRLS_API.Core.Helpers
{
    public class FhirSearchHelper
    {
        public static FilterDefinition<BsonDocument> BuildQuery(FhirRequest request)
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

                if (criteria == null)
                {
                    continue;
                }

                var paramVal = criteria.Item2;

                if (param.Type.Equals(SearchParamType.Reference) && !string.IsNullOrEmpty(paramVal))
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

            if (filters.Count() == 0)
            {
                return FilterDefinition<BsonDocument>.Empty;
            }

            return builder.And(filters);
        }
    }
}
