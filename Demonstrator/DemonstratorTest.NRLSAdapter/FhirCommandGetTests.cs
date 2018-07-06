using Demonstrator.NRLSAdapter.Helpers;
using System;
using Xunit;

namespace DemonstratorTest.NRLSAdapter
{
    public class FhirCommandGetTests
    {
        [Fact]
        public void Valid_Returns_Success()
        {
            var validArgs = new string[] {
                "--resource",
                "DocumentReference",
                "--method",
                "GET",
                "--parameters",
                "subject|code",
                "--format",
                "JSON",
                "--setheaders",
                "fromasid|value"
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
                "GET",
                "--parameters",
                "subject|code",
                "--setheaders",
                "fromasid|value"
            };

            var fhirCommand = new FhirCommand(validArgs);

            var fhirArgs = fhirCommand.StoreValidOptions();
            Assert.True(fhirArgs.Success, fhirArgs.Message);

            var fhirParse = fhirCommand.ParseCommand();
            Assert.True(fhirParse.Success, fhirParse.Message);
        }

        [Fact]
        public void Invalid_Null_Parameters_Returns_Fail()
        {
            var validArgs = new string[] {
                "--resource",
                "DocumentReference",
                "--method",
                "GET",
                "--parameters",
                null,
                "--format",
                "JSON"
            };

            var fhirCommand = new FhirCommand(validArgs);

            var fhirArgs = fhirCommand.StoreValidOptions();

            var fhirParse = fhirCommand.ParseCommand();
            Assert.False(fhirParse.Success);
        }

        [Fact]
        public void Invalid_Resource_Returns_Fail()
        {
            var validArgs = new string[] {
                "--resource",
                "fakeresourcebla",
                "--method",
                "GET",
                "--parameters",
                "subject|code"
            };

            var fhirCommand = new FhirCommand(validArgs);

            var fhirArgs = fhirCommand.StoreValidOptions();
            Assert.False(fhirArgs.Success);
        }
    }
}
