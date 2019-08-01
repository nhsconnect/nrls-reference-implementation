using Demonstrator.Core.Exceptions;
using Demonstrator.Core.Interfaces.Services.Nrls;
using Demonstrator.Models.Core.Models;
using Demonstrator.WebApp.Controllers;
using DemonstratorTest.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.NodeServices;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DemonstratorTest.WebApp
{
    public class CCBinaryControllerTests : IDisposable
    {
        IPointerService _pointerService;
        IOptions<ApiSetting> _apiSettings;

        public CCBinaryControllerTests()
        {
            var pointerService = new Mock<IPointerService>();
            _pointerService = pointerService.Object;

            var apiSettings = new Mock<IOptions<ApiSetting>>();
            _apiSettings = apiSettings.Object;
        }

        public void Dispose()
        {
            _pointerService = null;
            _apiSettings = null;
        }

        [Fact]
        public async void Get_OkFhir()
        {
            var controller = new BinaryController(_pointerService, _apiSettings);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = HttpContexts.Valid_DocumentFhir;

            var nodeServices = new Mock<INodeServices>();
            nodeServices.Setup(x => x.InvokeAsync<byte[]>(It.IsAny<string>(), It.IsAny<object[]>())).Returns(Task.Run(() => Encoding.UTF8.GetBytes("I'm a PDF!")));

            var response = await controller.Get(nodeServices.Object, "5a82f9ffcb969daa58d33377", "mhcp");

            Assert.IsType<FileContentResult>(response);

            var responseContent = response as FileContentResult;

            Assert.Equal(Encoding.UTF8.GetBytes("{\"resourceType\":\"Binary\",\"contentType\":\"application/pdf\",\"content\":\"SSdtIGEgUERGIQ==\"}"), responseContent.FileContents);
            Assert.Equal("application/xml+fhir; charset=utf-8", responseContent.ContentType);
        }

        [Fact]
        public async void Get_OkPdf()
        {
            var controller = new BinaryController(_pointerService, _apiSettings);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = HttpContexts.Valid_Search;

            var nodeServices = new Mock<INodeServices>();
            nodeServices.Setup(x => x.InvokeAsync<byte[]>(It.IsAny<string>(), It.IsAny<object[]>())).Returns(Task.Run(() => Encoding.UTF8.GetBytes("I'm a PDF!")));

            var response = await controller.Get(nodeServices.Object, "5a82f9ffcb969daa58d33377", "mhcp");

            Assert.IsType<FileContentResult>(response);

            var responseContent = response as FileContentResult;

            Assert.Equal(Encoding.UTF8.GetBytes("I'm a PDF!"), responseContent.FileContents);
            Assert.Equal("application/pdf", responseContent.ContentType);
        }

        [Fact]
        public void Get_InvalidEmptyId()
        {
            var controller = new BinaryController(_pointerService, _apiSettings);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = HttpContexts.Valid_Search;

            var nodeServices = new Mock<INodeServices>();
            nodeServices.Setup(x => x.InvokeAsync<byte[]>(It.IsAny<string>(), It.IsAny<object[]>())).Returns(Task.Run(() => Encoding.UTF8.GetBytes("I'm a PDF!")));

            Assert.ThrowsAsync<HttpFhirException>(async () => await controller.Get(nodeServices.Object, " ", "mhcp"));

        }

        [Fact]
        public void Get_InvalidNullId()
        {
            var controller = new BinaryController(_pointerService, _apiSettings);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = HttpContexts.Valid_Search;

            var nodeServices = new Mock<INodeServices>();
            nodeServices.Setup(x => x.InvokeAsync<byte[]>(It.IsAny<string>(), It.IsAny<object[]>())).Returns(Task.Run(() => Encoding.UTF8.GetBytes("I'm a PDF!")));

            Assert.ThrowsAsync<HttpFhirException>(async () => await controller.Get(nodeServices.Object, null, "mhcp"));

        }

        [Fact]
        public void Get_InvalidEmptyType()
        {
            var controller = new BinaryController(_pointerService, _apiSettings);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = HttpContexts.Valid_Search;

            var nodeServices = new Mock<INodeServices>();
            nodeServices.Setup(x => x.InvokeAsync<byte[]>(It.IsAny<string>(), It.IsAny<object[]>())).Returns(Task.Run(() => Encoding.UTF8.GetBytes("I'm a PDF!")));

            Assert.ThrowsAsync<HttpFhirException>(async () => await controller.Get(nodeServices.Object, "5a82f9ffcb969daa58d33377", " "));

        }

        [Fact]
        public void Get_InvalidNullType()
        {
            var controller = new BinaryController(_pointerService, _apiSettings);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = HttpContexts.Valid_Search;

            var nodeServices = new Mock<INodeServices>();
            nodeServices.Setup(x => x.InvokeAsync<byte[]>(It.IsAny<string>(), It.IsAny<object[]>())).Returns(Task.Run(() => Encoding.UTF8.GetBytes("I'm a PDF!")));

            Assert.ThrowsAsync<HttpFhirException>(async () => await controller.Get(nodeServices.Object, "5a82f9ffcb969daa58d33377", null));

        }


    }
}
