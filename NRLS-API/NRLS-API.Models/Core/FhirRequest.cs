using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Http;
using NRLS_API.Models.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NRLS_API.Models.Core
{
    public class FhirRequest
    {
        public FhirRequest()
        {
            QueryParameters = new List<Tuple<string, string>>();
            AllowedParameters = new string[0];
        }

        public string Id { get; set; }

        public ResourceType ResourceType { get; set; }

        public string StrResourceType => ResourceType.ToString();

        public Resource Resource { get; set; }

        public Uri RequestUrl { get; set; }

        public IEnumerable<Tuple<string, string>> QueryParameters { get; set; }

        public string[] AllowedParameters { get; set; }

        public string RequestingAsid { get; set; }

        public string IdParameter => GetIdParameterValue();

        public bool HasIdParameter => GetIdParameter() != null;

        public string ProfileUri { get; set; }

        public static FhirRequest Create(string id, ResourceType resourceType, Resource resource, HttpRequest request, string requestingAsid)
        {
            return new FhirRequest
            {
                Id = id,
                ResourceType = resourceType,
                Resource = resource,
                RequestUrl = CreateUrl(request.Scheme, request.Host.Value, request.Path, request.QueryString.Value),
                QueryParameters = request.QueryString.Value.GetParameters(),
                AllowedParameters = resourceType.GetAllowed(),
                RequestingAsid = requestingAsid
            };
        }

        public static FhirRequest Copy(FhirRequest request, ResourceType resourceType, Resource resource, IEnumerable<Tuple<string, string>> queryParameters)
        {
            return new FhirRequest
            {
                Id = request.Id,
                ResourceType = resourceType,
                Resource = resource,
                RequestUrl = request.RequestUrl,
                QueryParameters = queryParameters,
                AllowedParameters = resourceType.GetAllowed(),
                RequestingAsid = request.RequestingAsid
            };
        }

        public static Uri CreateUrl(string scheme, string host, string path, string queryString)
        {
            return new Uri($"{scheme}://{host}{path}{queryString}");
        }

        private string GetIdParameterValue()
        {
            return GetIdParameter()?.Item2;
        }

        private Tuple<string, string> GetIdParameter()
        {
            return QueryParameters.FirstOrDefault(x => x.Item1 == "_id");
        }

    }
}
