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
    public class OdsControllerTests : IDisposable
    {
        private IOdsSearch _odsSearch;

        public OdsControllerTests()
        {
            var organizationList = new List<Organization> { new Organization { Id = "ValidOrg1" }, new Organization { Id = "ValidOrg2"} };

            var odsSearch = new Mock<IOdsSearch>();
            odsSearch.Setup(x => x.Find(It.Is<FhirRequest>(y => y.RequestingAsid == null))).Returns(System.Threading.Tasks.Task.FromResult(FhirBundle.GetBundle(organizationList) as Resource));

            _odsSearch = odsSearch.Object;
        }

        public void Dispose()
        {
            _odsSearch = null;
        }

        [Fact]
        public async void Search_Valid()
        {

            var controller = new OdsController(_odsSearch);
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
