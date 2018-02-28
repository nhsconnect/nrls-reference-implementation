using Demonstrator.NRLSAdapter.Helpers;
using System;
using Xunit;

namespace DemonstratorTest.NRLSAdapter
{
    public class FhirCommandPutTests
    {
        [Fact]
        public void Valid_Returns_Success()
        {
            var validArgs = new string[] {
                "--resource",
                "DocumentReference",
                "--method",
                "PUT",
                "--body",
                "{somefakejsonbody}",
                "--uniqueid",
                "dfg34r"
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
                "PUT",
                "--uniqueid",
                "dfg34r"
            };

            var fhirCommand = new FhirCommand(validArgs);

            var fhirArgs = fhirCommand.StoreValidOptions();

            var fhirParse = fhirCommand.ParseCommand();
            Assert.False(fhirParse.Success);
        }

        [Fact]
        public void Invalid_Missing_UniqueId_Returns_Fail()
        {
            var validArgs = new string[] {
                "--resource",
                "DocumentReference",
                "--method",
                "PUT",
                "--body",
                "{somefakejsonbody}"
            };

            var fhirCommand = new FhirCommand(validArgs);

            var fhirArgs = fhirCommand.StoreValidOptions();

            var fhirParse = fhirCommand.ParseCommand();
            Assert.False(fhirParse.Success);
        }

        [Fact]
        public void Invalid_Method_Returns_Fail()
        {
            var validArgs = new string[] {
                "--resource",
                "DocumentReference",
                "--method",
                "DELETE",
                "--body",
                "{somefakejsonbody}"
            };

            var fhirCommand = new FhirCommand(validArgs);

            var fhirArgs = fhirCommand.StoreValidOptions();
            Assert.False(fhirArgs.Success);
        }

    }
}
