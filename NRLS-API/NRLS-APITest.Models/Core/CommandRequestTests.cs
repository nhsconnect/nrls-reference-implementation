using NRLS_API.Models.Core;
using Xunit;

namespace NRLS_APITest.Models
{
    public class CommandRequestTests
    {
        [Fact]
        public void CommandRequest_Default()
        {
            var model = new CommandRequest();

            Assert.NotNull(model.Headers);
        }

    }
}
