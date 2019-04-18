using NRLS_API.Models.ViewModels.Core;
using Xunit;

namespace NRLS_APITest.Models.ViewModels
{
    public class SdsViewModelTests
    {
        [Fact]
        public void SdsViewModel_CacheKey()
        {
            Assert.Equal("SdsViewModel", SdsViewModel.CacheKey);
        }

    }
}
