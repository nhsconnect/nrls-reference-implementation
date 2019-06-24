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
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DemonstratorTest.WebApp
{
    public class ActorOrganisationsControllerTests : IDisposable
    {
        IActorOrganisationService _actorOrganisationService;

        public ActorOrganisationsControllerTests()
        {
            var model = new ActorOrganisationViewModel
            {
                Id = "5a82f9ffcb969daa58d33377",
                Type = ActorType.Consumer,
                Name = "Ambulance Service",
                ImageUrl = "....",
                Context = new List<ContentView>()
                        {
                            new ContentView
                            {
                                Title = "Title Text",
                                Content = new List<string>{"Content Text" },
                                CssClass = "CssClass Text",
                                Order = 1
                            }
                        },
                OrgCode = "AMSR01",
                Benefits = new List<string> { "benefitid" },
                PersonnelLinkId = "testlink"
            };

            var actorOrganisationService = new Mock<IActorOrganisationService>();
            actorOrganisationService.Setup(x => x.GetById(It.Is<string>(y => y == "5a82f9ffcb969daa58d33377"))).Returns(Task.Run(() => model));
            actorOrganisationService.Setup(x => x.GetById(It.Is<string>(y => y == "5a82f9ffcb969daa58d33378"))).Returns(Task.Run(() => (ActorOrganisationViewModel)null));
            actorOrganisationService.Setup(x => x.GetPersonnel(It.Is<string>(y => y == "5a82f9ffcb969daa58d33377"))).Returns(Task.Run(() => (IEnumerable<PersonnelViewModel>) new List<PersonnelViewModel>()));

            _actorOrganisationService = actorOrganisationService.Object;
        }

        public void Dispose()
        {
            _actorOrganisationService = null;
        }

        [Fact]
        public async void ValidOrg_ReturnsOk()
        {
            var controller = new ActorOrganisationsController(_actorOrganisationService);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = HttpContexts.Valid_Search;

            var response = await controller.GetOne("5a82f9ffcb969daa58d33377");

            Assert.IsType<OkObjectResult>(response);

            var okResult = response as OkObjectResult;

            Assert.Equal(200, okResult.StatusCode);

            var responseContent = okResult.Value;

            Assert.IsType<ActorOrganisationViewModel>(responseContent);
            var viewModel = responseContent as ActorOrganisationViewModel;

            Assert.Equal("Ambulance Service", viewModel.Name);
            Assert.Equal("AMSR01", viewModel.OrgCode);
        }

        [Fact]
        public async void ValidOrg_ReturnsNotFound()
        {
            var controller = new ActorOrganisationsController(_actorOrganisationService);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = HttpContexts.Valid_Search;

            var response = await controller.GetOne("5a82f9ffcb969daa58d33378");

            Assert.IsType<NotFoundObjectResult>(response);

            var notFoundResult = response as NotFoundObjectResult;

            Assert.Equal(404, notFoundResult.StatusCode);

        }

        [Fact]
        public async void ValidPerson_ReturnsOk()
        {
            var controller = new ActorOrganisationsController(_actorOrganisationService);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = HttpContexts.Valid_Search;

            var response = await controller.GetPersonnel("5a82f9ffcb969daa58d33377");

            Assert.IsType<OkObjectResult>(response);

            var okResult = response as OkObjectResult;

            Assert.Equal(200, okResult.StatusCode);

            var responseContent = okResult.Value;

            Assert.IsType<List<PersonnelViewModel>>(responseContent);
            var viewModel = responseContent as List<PersonnelViewModel>;
        }

        [Fact]
        public async void ValidPerson_ReturnsNotFound()
        {
            var controller = new ActorOrganisationsController(_actorOrganisationService);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = HttpContexts.Valid_Search;

            var response = await controller.GetPersonnel("5a82f9ffcb969daa58d33378");

            Assert.IsType<NotFoundObjectResult>(response);

            var notFoundResult = response as NotFoundObjectResult;

            Assert.Equal(404, notFoundResult.StatusCode);

        }

    }
}
