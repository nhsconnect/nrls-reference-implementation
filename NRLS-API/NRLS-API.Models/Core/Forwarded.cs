using System;
using System.Collections.Generic;
using System.Text;

namespace NRLS_API.Models.Core
{
    public class Forwarded
    {
        //IP Address
        public string By { get; set; }

        //IP Address
        public string For { get; set; }

        //FQDN{:PORT}
        public string Host { get; set; }

        //HTTP|HTTPS
        public string Protocol { get; set; }
    }
}
