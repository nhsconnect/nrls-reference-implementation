using Demonstrator.Core.Exceptions;
using Demonstrator.Models.ViewModels.Base;
using Demonstrator.WebApp.Controllers;
using DemonstratorTest.Data;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace DemonstratorTest.WebApp
{
    public class FhirBaseControllerStub : FhirBaseController
    {
        public RequestViewModel SetHeadersTest(RequestViewModel model)
        {
            SetHeaders<RequestViewModel>(model);

            return model;
        }
    }
    public class FhirBaseControllerTests
    {

        [Fact]
        public void Valid_NoHeaders()
        {
            var controller = new FhirBaseControllerStub();
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = HttpContexts.Valid_Search;

            var resource = new RequestViewModel
            {
                Id = "test"
            };

            var result = controller.SetHeadersTest(resource);

            Assert.Equal("test", result.Id);
            Assert.Null(result.OrgCode);
            Assert.Null(result.Asid);
        }

        [Fact]
        public void Valid_WithHeaders()
        {
            var controller = new FhirBaseControllerStub();
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = HttpContexts.Valid_CustomHeaders;

            var resource = new RequestViewModel
            {
                Id = "test"
            };

            var result = controller.SetHeadersTest(resource);

            Assert.Equal("test", result.Id);
            Assert.Equal("000", result.Asid);
            Assert.Equal("org001", result.OrgCode);
        }

        [Fact]
        public void Invalid_NullModel()
        {
            var controller = new FhirBaseControllerStub();
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = HttpContexts.Valid_CustomHeaders;

            Assert.Throws<HttpFhirException>(() => controller.SetHeadersTest(null));

        }

    }
}
