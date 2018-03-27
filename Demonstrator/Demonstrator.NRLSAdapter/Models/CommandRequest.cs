using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using System;
using System.Net;
using System.Text;

namespace Demonstrator.NRLSAdapter.Models
{
    public class CommandRequest
    {
        public Resource Resource { get; set; }

        public ResourceType ResourceType { get; set; }

        public string BaseUrl { get; set; }

        public SearchParams SearchParams { get; set; }

        public string QueryString => BuildQueryString();

        public Uri FullUrl => BuildFullUrl();

        private Uri BuildFullUrl()
        {
            var url = new Uri($"{BaseUrl}/{ResourceType}{QueryString}");

            return url;
        }

        private string BuildQueryString()
        {
            var queryString = new StringBuilder();
            if(SearchParams != null && SearchParams.Parameters != null)
            {
                queryString.Append("?");

                foreach(var param in SearchParams.Parameters)
                {
                    queryString.Append($"{param.Item1}={param.Item2}");
                }
            }

            return queryString.ToString();
        }
    }
}
