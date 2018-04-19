using System;

namespace Demonstrator.Models.Core.Models
{
    public class ExternalApiSetting
    {
        public Uri NrlsServerUrl { get; set; }

        public Uri NrlsSecureServerUrl { get; set; }

        public bool NrlsUseSecure { get; set; }

        public string NrlsDefaultprofile { get; set; }

        public Uri PdsServerUrl { get; set; }

        public Uri PdsSecureServerUrl { get; set; }

        public bool PdsUseSecure { get; set; }

        public string PdsDefaultprofile { get; set; }

        public Uri OdsServerUrl { get; set; }

        public Uri OdsSecureServerUrl { get; set; }

        public bool OdsUseSecure { get; set; }

        public string OdsDefaultprofile { get; set; }

        public string SpineAsid { get; set; }

        public string SpineThumbprint { get; set; }

        public string ClientAsidMapFile { get; set; }
    }
}
