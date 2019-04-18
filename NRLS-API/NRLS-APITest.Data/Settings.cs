using NRLS_API.Models.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRLS_APITest.Data
{
    public class AppSettings
    {
        public static NrlsApiSetting NrlsApiSettings
        {
            get
            {
                return new NrlsApiSetting
                {
                    BaseUrl = "https://baseurl.com/",
                    DefaultPort = "80",
                    ProfileUrl = "https://profileurl.com",
                    Secure = false,
                    SecurePort = "443",
                    SupportedContentTypes = new List<string> { "application/fhir+json" },
                    SupportedResources = new List<string> { "DocumentReference" }
                };
            }
        }

        public static ApiSetting SspApiSettings
        {
            get
            {
                return new ApiSetting
                {
                    BaseUrl = "://localhost.com",
                    BasePath = "/nrls-ri/SSP",
                    Secure = false,
                    SecureOnly = false,
                    DefaultPort = "51913",
                    SecurePort = "44313"
                };
            }
        }
    }
}