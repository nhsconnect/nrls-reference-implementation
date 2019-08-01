using Demonstrator.Core.Interfaces.Services.Epr;
using Demonstrator.Models.Core.Enums;
using Demonstrator.Models.ViewModels.Epr;
using Demonstrator.WebApp.Controllers;
using DemonstratorTest.Data;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DemonstratorTest.WebApp
{
    public class EprControllerTests : IDisposable
    {
        ICrisisPlanService _crisisPlanService;

        public EprControllerTests()
        {
            var model = new CrisisPlanViewModel
            {
                InvolveFamilyOrCarer  = true,
                SignsFeelingUnwell  = "SignsFeelingUnwell",
                PotentialTriggers  = "PotentialTriggers",
                WhatHelpsInCrisis = "WhatHelpsInCrisis",
                ActionForDependants = "ActionForDependants",
                EmergencyLocation = "EmergencyLocation",
                EmergencyNumber  = "EmergencyNumber",
                CrisisNumber  = "CrisisNumber",
                PatientAcceptsPlan  = true,
                PlanCreatedBy  = "PlanCreatedBy",
                PlanCreatedByJobTitle  = "PlanCreatedByJobTitle",
                Version = 1,
                RecordType = RecordType.MentalHealthCrisisPlan,
                Active = true,
                PatientNhsNumber = "PatientNhsNumber",
                Id  = "5a82f9ffcb969daa58d33377",
                OrgCode  = "Org1",
                Asid  = "1234567890"
            };

            var crisisPlanService = new Mock<ICrisisPlanService>();
            crisisPlanService.Setup(x => x.GetForPatient(It.Is<string>(y => y == "1234567890"), It.IsAny<bool>())).Returns(Task.Run(() => model));
            crisisPlanService.Setup(x => x.GetForPatient(It.Is<string>(y => y == "1234567899"), It.IsAny<bool>())).Returns(Task.Run(() => (CrisisPlanViewModel)null));
            crisisPlanService.Setup(x => x.GetById(It.Is<string>(y => y == "5a82f9ffcb969daa58d33377"))).Returns(Task.Run(() => model));
            crisisPlanService.Setup(x => x.GetById(It.Is<string>(y => y == "5a82f9ffcb969daa58d33378"))).Returns(Task.Run(() => (CrisisPlanViewModel)null));

            _crisisPlanService = crisisPlanService.Object;
        }

        public void Dispose()
        {
            _crisisPlanService = null;
        }

        [Fact]
        public async void ValidPatient_ReturnsOk()
        {
            var controller = new EprController(_crisisPlanService);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = HttpContexts.Valid_Search;

            var response = await controller.GetForPatient("1234567890");

            Assert.IsType<OkObjectResult>(response);

            var okResult = response as OkObjectResult;

            Assert.Equal(200, okResult.StatusCode);

            var responseContent = okResult.Value;

            Assert.IsType<CrisisPlanViewModel>(responseContent);
            var viewModel = responseContent as CrisisPlanViewModel;

            Assert.Equal("1234567890", viewModel.Asid);
            Assert.Equal("Org1", viewModel.OrgCode);
        }

        [Fact]
        public async void ValidPatient_ReturnsNotFound()
        {
            var controller = new EprController(_crisisPlanService);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = HttpContexts.Valid_Search;

            var response = await controller.GetForPatient("1234567899");

            Assert.IsType<OkObjectResult>(response);

            var okResult = response as OkObjectResult;

            Assert.Equal(200, okResult.StatusCode);

            var responseContent = okResult.Value;

            Assert.IsType<string>(responseContent);
            var viewModel = responseContent as string;

            Assert.Equal("null", viewModel);

        }

        [Fact]
        public async void ValidId_ReturnsOk()
        {
            var controller = new EprController(_crisisPlanService);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = HttpContexts.Valid_Search;

            var response = await controller.GetById("5a82f9ffcb969daa58d33377");

            Assert.IsType<OkObjectResult>(response);

            var okResult = response as OkObjectResult;

            Assert.Equal(200, okResult.StatusCode);

            var responseContent = okResult.Value;

            Assert.IsType<CrisisPlanViewModel>(responseContent);
            var viewModel = responseContent as CrisisPlanViewModel;

            Assert.Equal("1234567890", viewModel.Asid);
            Assert.Equal("Org1", viewModel.OrgCode);
        }

        [Fact]
        public async void ValidId_ReturnsNotFound()
        {
            var controller = new EprController(_crisisPlanService);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = HttpContexts.Valid_Search;

            var response = await controller.GetById("5a82f9ffcb969daa58d33378");

            Assert.IsType<NotFoundObjectResult>(response);

            var notFoundResult = response as NotFoundObjectResult;

            Assert.Equal(404, notFoundResult.StatusCode);

        }

    }
}
