using Demonstrator.NRLSAdapter.Helpers;
using System;
using Xunit;

namespace DemonstratorTest.NRLSAdapter
{
    public class FhirCommandPostTests
    {
        [Fact]
        public void Valid_Returns_Success()
        {
            var validArgs = new string[] {
                "--resource",
                "DocumentReference",
                "--method",
                "POST",
                "--body",
                "{somefakejsonbody}",
                "--format",
                "JSON"
            };

            var fhirCommand = new FhirCommand(validArgs);

            var fhirArgs = fhirCommand.StoreValidOptions();
            Assert.True(fhirArgs.Success, fhirArgs.Message);

            var fhirParse = fhirCommand.ParseCommand();
            Assert.True(fhirParse.Success, fhirParse.Message);
        }

        [Fact]
        public void Valid_Without_Format_Returns_Success()
        {
            var validArgs = new string[] {
                "--resource",
                "DocumentReference",
                "--method",
                "POST",
                "--body",
                "{somefakejsonbody}"
            };

            var fhirCommand = new FhirCommand(validArgs);

            var fhirArgs = fhirCommand.StoreValidOptions();
            Assert.True(fhirArgs.Success, fhirArgs.Message);

            var fhirParse = fhirCommand.ParseCommand();
            Assert.True(fhirParse.Success, fhirParse.Message);
        }

        [Fact]
        public void Invalid_Missing_Body_Returns_Fail()
        {
            var validArgs = new string[] {
                "--resource",
                "DocumentReference",
                "--method",
                "POST"
            };

            var fhirCommand = new FhirCommand(validArgs);

            var fhirArgs = fhirCommand.StoreValidOptions();

            var fhirParse = fhirCommand.ParseCommand();
            Assert.False(fhirParse.Success);
        }

    }
}
