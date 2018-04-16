using System.Collections.Generic;

namespace NRLS_API.Models.Core
{
    public class NrlsApiSetting
    {
        public List<string> SupportedResources { get; set; }

        public List<string> SupportedContentTypes { get; set; }

        public string ProfileUrl { get; set; }

        public string BaseUrl { get; set; }
    }
}
