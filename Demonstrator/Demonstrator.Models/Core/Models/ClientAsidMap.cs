﻿using System.Collections.Generic;

namespace Demonstrator.Models.Core.Models
{
    public class ClientAsidMap
    {
        public IDictionary<string, ClientAsid> ClientAsids { get; set; }

        public static string Key = "ClientAsidMap";
    }

    public class ClientAsid
    {
        public string Thumbprint { get; set; }

        public IList<string> Interactions { get; set; }

        public string OrgCode { get; set; }
    }
}
