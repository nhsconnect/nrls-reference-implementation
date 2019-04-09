using System.Collections.Generic;

namespace NRLS_API.Models.Core
{
    public class CommandResponse
    {
        public CommandResponse()
        {
            Headers = new Dictionary<string, IEnumerable<string>>();
        }

        public byte[] Content { get; set; }

        public IDictionary<string, IEnumerable<string>> Headers { get; set; }

        public int StatusCode { get; set; }

    }
}
