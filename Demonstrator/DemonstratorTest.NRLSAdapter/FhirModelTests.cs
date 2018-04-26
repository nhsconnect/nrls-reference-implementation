using Demonstrator.NRLSAdapter.Models;
using DemonstratorTest.Comparer;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using System;
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

        [Fact]
        public void CommandRequest_Set_BuildsUrl()
        {

            var testBaseUrl = "https://testurl.com";
            var testResourceType = ResourceType.DocumentReference;

            var testSearchParams = new SearchParams();
            testSearchParams.Add("paramA", "valueA");
            testSearchParams.Add("paramB", "valueB");

            var validCommandRequest = new CommandRequest
            {
                BaseUrl = testBaseUrl,
                ResourceType = testResourceType,
                SearchParams = testSearchParams
            };

            var expectedQueryString = "?paramA=valueA&paramB=valueB";

            var expectedFullUrl = new Uri($"{testBaseUrl}/{testResourceType}{expectedQueryString}");

            Assert.Equal(validCommandRequest.QueryString, expectedQueryString);

            Assert.Equal(validCommandRequest.FullUrl.AbsoluteUri, expectedFullUrl.AbsoluteUri);
        }
    }
}
