using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Http;
using NRLS_API.Models.Extensions;
using NRLS_API.Models.Helpers;
using System;
using System.Collections.Generic;

namespace NRLS_API.Models.Core
{
    public class FhirRequest
    {
        public string Id { get; set; }

        public ResourceType ResourceType { get; set; }

        public string StrResourceType => ResourceType.ToString();

        public Resource Resource { get; set; }

        public Uri RequestUrl { get; set; }

        public IEnumerable<Tuple<string, string>> QueryParameters { get; set; }

        public string[] AllowedParameters { get; set; }

        public static FhirRequest Create(string id, ResourceType resourceType, Resource resource, HttpRequest request)
        {
            return new FhirRequest
            {
                Id = id,
                ResourceType = resourceType,
                Resource = resource,
                RequestUrl = CreateUrl(request.Scheme, request.Host.Value, request.Path, request.QueryString.Value),
                QueryParameters = request.QueryString.Value.GetParameters(),
                AllowedParameters = resourceType.GetAllowed()
            };
        }

        public static Uri CreateUrl(string scheme, string host, string path, string queryString)
        {
            return new Uri($"{scheme}://{host}{path}{queryString}");
        }

    }
}
