using NRLS_API.Models.Core;
using Xunit;

namespace NRLS_APITest.Models
{
    public class FhirRequestExtensionsTests
    {
        [Fact]
        public void ApiSetting_Default()
        {
            var apiSetting = new ApiSetting();

            Assert.NotNull(apiSetting.SupportedContentTypes);
            Assert.NotNull(apiSetting.SupportedResources);
        }

    }
}
