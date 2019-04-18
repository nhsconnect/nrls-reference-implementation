using NRLS_API.Core.Resources;
using NRLS_API.Models.ViewModels.Core;
using System.Collections.Generic;

namespace NRLS_APITest.Data
{
    public class SdsViewModels
    {
        public static IEnumerable<SdsViewModel> SdsAsidAll
        {
            get
            {
                return new List<SdsViewModel>
                {
                    SdsAsid000,
                    SdsAsid002,
                    SdsAsid003,
                    SdsAsid20000000017
                };
            }
        }

        public static SdsViewModel SdsAsid000
        {
            get
            {
                return new SdsViewModel
                {
                    Asid = "000",
                    OdsCode = "TestOrgCode",
                    Interactions = new List<string> { FhirConstants.ReadInteractionId },
                    Thumbprint = "TestThumbprint",
                    Id = "5cb5fd22c892d7e5f291190a"
                };
            }
        }

        public static SdsViewModel SdsAsid002
        {
            get
            {
                return new SdsViewModel
                {
                    Asid = "002",
                    OdsCode = "TestOrgCode2",
                    Interactions = new List<string>(),
                    Thumbprint = "TestThumbprint",
                    Id = "5cb5fdbdc892d7e5f291190b"
                };
            }
        }

        public static SdsViewModel SdsAsid003
        {
            get
            {
                return new SdsViewModel
                {
                    Asid = "003",
                    OdsCode = "RV99",
                    Interactions = new List<string>(),
                    Thumbprint = "TestThumbprint",
                    Id = "5cb5fdcac892d7e5f291190c"
                };
            }
        }

        public static SdsViewModel SdsAsid20000000017
        {
            get
            {
                return new SdsViewModel
                {
                    Asid = "20000000017",
                    OdsCode = "ORG1",
                    Interactions = new List<string>(),
                    Thumbprint = "TestThumbprint",
                    Id = "5cb5fdd5c892d7e5f291190d"
                };
            }
        }
    }
}