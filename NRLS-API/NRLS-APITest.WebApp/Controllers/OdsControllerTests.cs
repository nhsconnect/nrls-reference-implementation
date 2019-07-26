using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NRLS_API.Core.Interfaces.Services;
using NRLS_API.Core.Resources;
using NRLS_API.Models.Core;
using NRLS_API.WebApp.Controllers;
using NRLS_APITest.Data;
using System;
using System.Collections.Generic;
using System.Linq;
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
            odsSearch.Setup(x => x.Find(It.Is<FhirRequest>(y => y.RequestingAsid == null))).Returns(System.Threading.Tasks.Task.FromResult(FhirBundle.GetBundle(organizationList)));
            odsSearch.Setup(x => x.GetByQuery(It.IsAny<FhirRequest>())).Returns(System.Threading.Tasks.Task.FromResult(FhirOrganizations.Valid_Organization));
            odsSearch.Setup(x => x.GetByQuery(It.Is<FhirRequest>(y => y.IdentifierParameter == $"{FhirConstants.SystemOrgCode}|NOTFOUND"))).Returns(System.Threading.Tasks.Task.FromResult(null as Organization));
            
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

            var result = response as OkObjectResult;

            Assert.Equal(200, result.StatusCode);

            var responseContent = result.Value;

            Assert.IsType<Bundle>(responseContent);

            var bundle = responseContent as Bundle;

            Assert.Equal(2, bundle.Total);
        }

        [Fact]
        public async void Read_Valid()
        {

            var controller = new OdsController(_odsSearch);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = HttpContexts.Valid_Search;

            var response = await controller.Read("odscode");

            Assert.IsType<OkObjectResult>(response);

            var okResult = response as OkObjectResult;

            Assert.Equal(200, okResult.StatusCode);

            var responseContent = okResult.Value;

            Assert.IsType<Organization>(responseContent);

            var org = responseContent as Organization;

            Assert.Equal("5ab13695957d0ad5d93a1330", org.Id);

        }

        [Fact]
        public async void Read_Invalid()
        {

            var controller = new OdsController(_odsSearch);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = HttpContexts.NotFound_Search;

            var response = await controller.Read("notfound");

            Assert.IsType<NotFoundObjectResult>(response);

            var result = response as NotFoundObjectResult;

            Assert.Equal(404, result.StatusCode);

            var responseContent = result.Value;

            Assert.IsType<OperationOutcome>(responseContent);

            var outcome = responseContent as OperationOutcome;

            Assert.Single(outcome.Issue);

            Assert.Equal("NotFound", outcome.Issue.First().Code.Value.ToString());

        }

    }
}
