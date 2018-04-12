using System;

namespace Demonstrator.NRLSAdapter.Models
{
    public class NrlsPointerBody
    {
        public string Id { get; set; }

        public string OrgCode { get; set; }

        public string NhsNumber { get; set; }

        public string Url { get; set; }

        public string ContentType { get; set; }

        public string TypeCode { get; set; }

        public string TypeDisplay { get; set; }

        public DateTime Creation { get; set; }

    }
}
