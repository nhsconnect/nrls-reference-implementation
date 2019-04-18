using NRLS_API.Models.Core;
using Xunit;

namespace NRLS_APITest.Models
{
    public class CommandResponseTests
    {
        [Fact]
        public void CommandResponse_Default()
        {
            var model = new CommandResponse();

            Assert.NotNull(model.Headers);
        }

    }
}
