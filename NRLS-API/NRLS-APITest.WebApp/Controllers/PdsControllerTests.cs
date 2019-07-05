using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NRLS_API.Core.Interfaces.Services;
using NRLS_API.Models.Core;
using NRLS_API.WebApp.Controllers;
using NRLS_APITest.Data;
using System;
using System.Collections.Generic;
using Xunit;

namespace NRLS_APITest.WebApp.Controllers
{
    public class PdsControllerTests : IDisposable
    {
        private IPdsSearch _pdsSearch;

        public PdsControllerTests()
        {
            var patientList = new List<Patient> { new Patient { Id = "ValidPatient1" }, new Patient { Id = "ValidPatient2" } };

            var pdsSearch = new Mock<IPdsSearch>();
            pdsSearch.Setup(x => x.Find(It.Is<FhirRequest>(y => y.RequestingAsid == null))).Returns(System.Threading.Tasks.Task.FromResult(FhirBundle.GetBundle(patientList) as Resource));

            _pdsSearch = pdsSearch.Object;
        }

        public void Dispose()
        {
            _pdsSearch = null;
        }

        [Fact]
        public async void Search_Valid()
        {

            var controller = new PdsController(_pdsSearch);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = HttpContexts.Valid_Search;

            var response = await controller.Search();

            Assert.IsType<OkObjectResult>(response);

            var okResult = response as OkObjectResult;

            Assert.Equal(200, okResult.StatusCode);

            var responseContent = okResult.Value;

            Assert.IsType<Bundle>(responseContent);

            var bundle = responseContent as Bundle;

            Assert.Equal(2, bundle.Total);

        }

    }
}
