using System;
using System.Collections.Generic;

namespace NRLS_API.Models.ViewModels.Core
{
    public class SdsViewModel
    {
        public string Id { get; set; }

        public Guid PartyKey { get; set; }

        public string Fqdn { get; set; }

        public IEnumerable<Uri> EndPoints { get; set; }

        public string OdsCode { get; set; }

        public IEnumerable<string> Interactions { get; set; }

        public string Asid { get; set; }

        public string Thumbprint { get; set; }

        public bool Active { get; set; }

        public static string CacheKey = "SdsViewModel";

    }
}
