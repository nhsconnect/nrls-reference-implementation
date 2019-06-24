using Demonstrator.Core.Interfaces.Services.Nrls;
using Demonstrator.Models.ViewModels.Base;
using Demonstrator.Models.ViewModels.Nrls;
using Demonstrator.WebApp.Controllers;
using DemonstratorTest.Data;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Text;
using Xunit;

namespace DemonstratorTest.WebApp
{
    public class NrlsControllerTests : IDisposable
    {
        IPointerService _pointerService;

        public NrlsControllerTests()
        {
            var pointerService = new Mock<IPointerService>();
            pointerService.Setup(x => x.GetCachedPointer(It.Is<string>(y => y == "0000000000"), It.IsAny<string>())).Returns(PointerViewModels.ValidFor0000000000);
            pointerService.Setup(x => x.GetCachedPointer(It.Is<string>(y => y == "0000000001"), It.IsAny<string>())).Returns((PointerViewModel)null);
            pointerService.Setup(x => x.GetPointerDocument(It.IsAny<RequestViewModel>(), It.IsAny<PointerViewModel>())).Returns(System.Threading.Tasks.Task.Run(() => FhirBinaries.Html));

            _pointerService = pointerService.Object;
        }

        public void Dispose()
        {
            _pointerService = null;
        }

        [Fact]
        public async void Valid_ReturnsOk()
        {
            var controller = new NrlsController(_pointerService);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = HttpContexts.Valid_Search;

            var response = await controller.Document("anything", "0000000000");

            Assert.IsType<OkObjectResult>(response);

            var okResult = response as OkObjectResult;

            Assert.Equal(200, okResult.StatusCode);

            var responseContent = okResult.Value;

            Assert.IsType<Binary>(responseContent);
            var binary = responseContent as Binary;

            Assert.Equal("text/html", binary.ContentType);
            Assert.Equal(Encoding.UTF8.GetBytes("<p>Hello</p>"), binary.Content);
        }

        [Fact]
        public async void Valid_ReturnsNotFound()
        {
            var controller = new NrlsController(_pointerService);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = HttpContexts.Valid_Search;

            var response = await controller.Document("anything", "0000000001");

            Assert.IsType<NotFoundResult>(response);

            var notFoundResult = response as NotFoundResult;

            Assert.Equal(404, notFoundResult.StatusCode);

        }
    }
}
