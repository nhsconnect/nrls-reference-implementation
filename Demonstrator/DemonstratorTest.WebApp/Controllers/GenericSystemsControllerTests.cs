using Demonstrator.Core.Interfaces.Services.Flows;
using Demonstrator.Models.Core.Enums;
using Demonstrator.Models.Core.Models;
using Demonstrator.Models.ViewModels.Flows;
using Demonstrator.WebApp.Controllers;
using DemonstratorTest.Data;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace DemonstratorTest.WebApp
{
    public class GenericSystemsControllerTests : IDisposable
    {
        IGenericSystemService _genericSystemService;
        IPersonnelService _personnelService;

        public GenericSystemsControllerTests()
        {
            var systemModel = new GenericSystemViewModel
            {
                Id = "5a8417338317338c8e0809e5",
                Name = "Ambulance Service Call Handler",
                FModule = "Ambulance_Service_Call_Handler",
                Asid = "200000000115",
                Context = "Some context...",
                ActionTypes = new List<ActorType> { ActorType.Consumer }
            };

            var personelModel = new PersonnelViewModel
            {
                Id = "5a8417f68317338c8e080a62",
                Name = "999 Call Handler",
                ImageUrl = "....",
                Context = new List<ContentView>()
                {
                    new ContentView
                    {
                        Title = "Title Text",
                        Content = new List<string> { "Content Text" },
                        CssClass = "CssClass Text",
                        Order = 1
                    }
                },
                UsesNrls = true,
                ActorOrganisationId = "5a82f9ffcb969daa58d33377",
                CModule = "CModule-Type",
                SystemIds = new List<string> { "5a8417338317338c8e0809e5" },
                Benefits = new List<string> { "benefitid" }
            };

            var genericSystemService = new Mock<IGenericSystemService>();
            genericSystemService.Setup(x => x.GetById(It.Is<string>(y => y == "5a8417338317338c8e0809e5"))).Returns(Task.Run(() => systemModel));
            genericSystemService.Setup(x => x.GetById(It.Is<string>(y => y == "5a8417338317338c8e0809e6"))).Returns(Task.Run(() => (GenericSystemViewModel)null));

            var personnelService = new Mock<IPersonnelService>();
            personnelService.Setup(x => x.GetModelBySystemId(It.Is<string>(y => y == "5a8417f68317338c8e080a62"))).Returns(Task.Run(() => personelModel));
            personnelService.Setup(x => x.GetModelBySystemId(It.Is<string>(y => y == "5a8417f68317338c8e080a63"))).Returns(Task.Run(() => (PersonnelViewModel)null));

            _genericSystemService = genericSystemService.Object;
            _personnelService = personnelService.Object;
        }

        public void Dispose()
        {
            _genericSystemService = null;
            _personnelService = null;
        }

        [Fact]
        public async void ValidSystem_ReturnsOk()
        {
            var controller = new GenericSystemsController(_genericSystemService, _personnelService);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = HttpContexts.Valid_Search;

            var response = await controller.Get("5a8417338317338c8e0809e5");

            Assert.IsType<OkObjectResult>(response);

            var okResult = response as OkObjectResult;

            Assert.Equal(200, okResult.StatusCode);

            var responseContent = okResult.Value;

            Assert.IsType<GenericSystemViewModel>(responseContent);
            var viewModel = responseContent as GenericSystemViewModel;

            Assert.Equal("Ambulance Service Call Handler", viewModel.Name);
            Assert.Equal("200000000115", viewModel.Asid);
        }

        [Fact]
        public async void ValidSystem_ReturnsNotFound()
        {
            var controller = new GenericSystemsController(_genericSystemService, _personnelService);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = HttpContexts.Valid_Search;

            var response = await controller.Get("5a8417338317338c8e0809e6");

            Assert.IsType<NotFoundObjectResult>(response);

            var notFoundResult = response as NotFoundObjectResult;

            Assert.Equal(404, notFoundResult.StatusCode);

        }

        [Fact]
        public async void ValidPersonel_ReturnsOk()
        {
            var controller = new GenericSystemsController(_genericSystemService, _personnelService);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = HttpContexts.Valid_Search;

            var response = await controller.GetPersonnel("5a8417f68317338c8e080a62");

            Assert.IsType<OkObjectResult>(response);

            var okResult = response as OkObjectResult;

            Assert.Equal(200, okResult.StatusCode);

            var responseContent = okResult.Value;

            Assert.IsType<PersonnelViewModel>(responseContent);
            var viewModel = responseContent as PersonnelViewModel;

            Assert.Equal("999 Call Handler", viewModel.Name);
            Assert.Equal("CModule-Type", viewModel.CModule);
        }

        [Fact]
        public async void ValidPersonel_ReturnsNotFound()
        {
            var controller = new GenericSystemsController(_genericSystemService, _personnelService);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = HttpContexts.Valid_Search;

            var response = await controller.GetPersonnel("5a8417f68317338c8e080a63");

            Assert.IsType<NotFoundObjectResult>(response);

            var notFoundResult = response as NotFoundObjectResult;

            Assert.Equal(404, notFoundResult.StatusCode);

        }
    }
}
