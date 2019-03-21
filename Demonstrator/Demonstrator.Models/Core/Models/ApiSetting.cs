using System.Collections.Generic;

namespace Demonstrator.Models.Core.Models
{
    public class ApiSetting
    {
        public ApiSetting()
        {
            SupportedContentTypes = new List<string>();
        }

        public IList<string> SupportedContentTypes { get; set; }

        public string BaseUrl { get; set; }

        public bool Secure { get; set; }

        public bool SecureOnly { get; set; }

        public string DefaultPort { get; set; }

        public string SecurePort { get; set; }
    }
}
