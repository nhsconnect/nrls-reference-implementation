using System;
using System.Collections.Generic;
using System.Text;

namespace NRLS_API.Models.Core
{
    public class Forwarded
    {
        public string By { get; set; }

        public string For { get; set; }

        public string Host { get; set; }

        public string Protocol { get; set; }
    }
}
