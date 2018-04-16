using System;

namespace Demonstrator.Models.Core.Models
{
    public class ExternalApiSetting
    {
        public Uri NrlsServerUrl { get; set; }

        public Uri PdsServerUrl { get; set; }

        public Uri OdsServerUrl { get; set; }

        public string SpineAsid { get; set; }

        public string SpineThumbprint { get; set; }

        public string ClientAsidMapFile { get; set; }
    }
}
