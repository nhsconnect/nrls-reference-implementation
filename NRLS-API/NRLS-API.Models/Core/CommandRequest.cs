using System;
using System.Collections.Generic;
using System.Net.Http;

namespace NRLS_API.Models.Core
{
    public class CommandRequest
    {
        public CommandRequest()
        {
            Headers = new List<KeyValuePair<string, string>>();
        }

        public HttpMethod Method { get; set; }

        //public HttpContent Content { get; set; }

        public Uri ForwardUrl { get; set; }

        public Forwarded Forwarded { get; set; }

        public string ClientThumbprint { get; set; }

        public string ServerThumbprint { get; set; }

        public bool UseSecure { get; set; }

        public IEnumerable<KeyValuePair<string, string>> Headers { get; set; }

    }
}
