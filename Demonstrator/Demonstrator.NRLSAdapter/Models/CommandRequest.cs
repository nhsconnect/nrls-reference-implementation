using Demonstrator.Models.Core.Models;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Demonstrator.NRLSAdapter.Models
{
    public class CommandRequest : Request
    {
        public CommandRequest()
        {
            Headers = new Dictionary<string, string>();
            RegenerateUrl = true;
        }

        public Resource Resource { get; set; }

        public ResourceType ResourceType { get; set; }

        public HttpMethod Method { get; set; }

        public HttpContent Content { get; set; }

        public string BaseUrl { get; set; }

        public string ClientThumbprint { get; set; }

        public string ServerThumbprint { get; set; }

        public bool UseSecure { get; set; }

        public bool RegenerateUrl { get; set; }

        public string ResourceId { get; set; }

        public IDictionary<string, string> Headers { get; set; }

        public SearchParams SearchParams { get; set; }

        public string QueryString => BuildQueryString();

        public Uri FullUrl => BuildFullUrl();

        private Uri BuildFullUrl()
        {
            //var url = new Uri($"{BaseUrl}/{ResourceType}{("/" + ResourceId ?? "")}{QueryString}"); // currently no endpoints contain a resource id

            var urlTemplate = RegenerateUrl ? $"{BaseUrl}/{ResourceType}{QueryString}" : $"{BaseUrl}" ;
            var url = new Uri(urlTemplate);

            return url;
        }

        private string BuildQueryString()
        {
            var queryString = new StringBuilder();
            if(SearchParams != null && SearchParams.Parameters != null && SearchParams.Parameters.Count > 0)
            {
                queryString.Append("?");

                foreach(var param in SearchParams.Parameters)
                {
                    var prefix = queryString.Length > 1 ? "&" : "";
                    queryString.Append($"{prefix}{param.Item1}={param.Item2}");
                }
            }

            return queryString.ToString();
        }
    }
}
