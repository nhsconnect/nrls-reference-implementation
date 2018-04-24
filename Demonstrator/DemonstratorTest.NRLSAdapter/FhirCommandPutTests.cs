using Demonstrator.NRLSAdapter.Helpers;
using System;
using Xunit;

namespace DemonstratorTest.NRLSAdapter
{
    public class FhirCommandPutTests
    {
        //[Fact]
        //public void Valid_Returns_Success()
        //{
        //    var validArgs = new string[] {
        //        "--resource",
        //        "DocumentReference",
        //        "--method",
        //        "PUT",
        //        "--body",
        //        "{'OrgCode': 'MHT01','NhsNumber': '3656987882','Url': 'http://example.org/xds/mhd/Binary/test12345.pdf','ContentType': 'application/pdf','TypeCode': '718347001','TypeDisplay': 'Mental health care plan','Creation': '2018-04-02T11:15:45+00:00'}",
        //        "--uniqueid",
        //        "dfg34r"
        //    };

        //    var fhirCommand = new FhirCommand(validArgs);

        //    var fhirArgs = fhirCommand.StoreValidOptions();
        //    Assert.True(fhirArgs.Success, fhirArgs.Message);

        //    var fhirParse = fhirCommand.ParseCommand();
        //    Assert.True(fhirParse.Success, fhirParse.Message);
        //}

        //[Fact]
        //public void Invalid_Missing_Body_Returns_Fail()
        //{
        //    var validArgs = new string[] {
        //        "--resource",
        //        "DocumentReference",
        //        "--method",
        //        "PUT",
        //        "--uniqueid",
        //        "dfg34r"
        //    };

        //    var fhirCommand = new FhirCommand(validArgs);

        //    var fhirArgs = fhirCommand.StoreValidOptions();

        //    var fhirParse = fhirCommand.ParseCommand();
        //    Assert.False(fhirParse.Success);
        //}

        //[Fact]
        //public void Invalid_Missing_UniqueId_Returns_Fail()
        //{
        //    var validArgs = new string[] {
        //        "--resource",
        //        "DocumentReference",
        //        "--method",
        //        "PUT",
        //        "--body",
        //        "{'OrgCode': 'MHT01','NhsNumber': '3656987882','Url': 'http://example.org/xds/mhd/Binary/test12345.pdf','ContentType': 'application/pdf','TypeCode': '718347001','TypeDisplay': 'Mental health care plan','Creation': '2018-04-02T11:15:45+00:00'}"
        //    };

        //    var fhirCommand = new FhirCommand(validArgs);

        //    var fhirArgs = fhirCommand.StoreValidOptions();

        //    var fhirParse = fhirCommand.ParseCommand();
        //    Assert.False(fhirParse.Success);
        //}

        [Fact]
        public void Invalid_Method_Returns_Fail()
        {
            var validArgs = new string[] {
                "--resource",
                "DocumentReference",
                "--method",
                "PUT",
                "--body",
                "{'OrgCode': 'MHT01','NhsNumber': '3656987882','Url': 'http://example.org/xds/mhd/Binary/test12345.pdf','ContentType': 'application/pdf','TypeCode': '718347001','TypeDisplay': 'Mental health care plan','Creation': '2018-04-02T11:15:45+00:00'}"
            };

            var fhirCommand = new FhirCommand(validArgs);

            var fhirArgs = fhirCommand.StoreValidOptions();
            Assert.False(fhirArgs.Success);
        }

    }
}
