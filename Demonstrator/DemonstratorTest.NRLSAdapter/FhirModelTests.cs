using Demonstrator.NRLSAdapter.Models;
using DemonstratorTest.Comparer;
using Xunit;

namespace DemonstratorTest.NRLSAdapter
{
    public class FhirModelTests
    {
        [Fact]
        public void CommandResponse_Set_Matches()
        {
            var validCommandResponse = new CommandResponse {
                Message = "Success",
                Success = true
            };

            var commandResponse = CommandResponse.Set(true, "Success");

            Assert.Equal(validCommandResponse, commandResponse, Comparers.ModelComparer<CommandResponse>());
        }

        [Fact]
        public void CommandResults_Set_Matches()
        {
            var validCommandResponse = new CommandResults<string>
            {
                Message = "Success",
                Results = null,
                Success = true
            };

            var commandResponse = CommandResults<string>.Set(true, "Success");

            Assert.Equal(validCommandResponse, commandResponse, Comparers.ModelComparer<CommandResults<string>>());
        }

        [Fact]
        public void CommandResult_Set_Matches()
        {
            var validCommandResponse = new CommandResult<string>
            {
                Message = "Success",
                Result = null,
                Success = true
            };

            var commandResponse = CommandResult<string>.Set(true, "Success", null);

            Assert.Equal(validCommandResponse, commandResponse, Comparers.ModelComparer<CommandResult<string>>());
        }
    }
}
