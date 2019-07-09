using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using NRLS_API.Core.Interfaces.Services;
using NRLS_API.Models.Core;
using NRLS_API.WebApp.Controllers;
using NRLS_APITest.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace NRLS_APITest.WebApp.Controllers
{
    public class NrlsControllerTests : IDisposable
    {
        private IOptionsSnapshot<NrlsApiSetting> _nrlsSettings;
        private INrlsSearch _nrlsSearch;
        private INrlsMaintain _nrlsMaintain;

        public NrlsControllerTests()
        {
            var nrlsSettingsMock = new Mock<IOptionsSnapshot<NrlsApiSetting>>();
            nrlsSettingsMock.Setup(op => op.Get(It.IsAny<string>())).Returns(AppSettings.NrlsApiSettings);

            var pointerList = new List<DocumentReference> { NrlsPointers.Valid, NrlsPointers.Valid_With_Alt_Custodian };

            var searchMock = new Mock<INrlsSearch>();
            //searchMock.Setup(x => x.Find<DocumentReference>(It.IsAny<FhirRequest>())).Returns(System.Threading.Tasks.Task.FromResult(FhirBundle.GetBundle(pointerList) as Resource));
            searchMock.Setup(x => x.Find(It.Is<FhirRequest>(y => y.RequestingAsid == "000"))).Returns(System.Threading.Tasks.Task.FromResult(FhirBundle.GetBundle(pointerList) as Resource));
            searchMock.Setup(x => x.Get(It.Is<FhirRequest>(y => y.RequestingAsid == "000"))).Returns(System.Threading.Tasks.Task.FromResult(NrlsPointers.Valid as Resource));
            searchMock.Setup(x => x.Get(It.Is<FhirRequest>(y => y.RequestingAsid == "notfound"))).Returns(System.Threading.Tasks.Task.FromResult(null as Resource));
            searchMock.Setup(x => x.Get(It.Is<FhirRequest>(y => y.RequestingAsid == "inactive"))).Returns(System.Threading.Tasks.Task.FromResult(NrlsPointers.Valid_Superseded as Resource)); 

            var maintMock = new Mock<INrlsMaintain>();
            //maintMock.Setup(x => x.Find<DocumentReference>(It.IsAny<FhirRequest>())).Returns(System.Threading.Tasks.Task.FromResult(new Bundle() as Resource));
            maintMock.Setup(x => x.Delete(It.Is<FhirRequest>(y => y.RequestingAsid == "fromASID"))).Returns(System.Threading.Tasks.Task.FromResult(OperationOutcomes.Deleted));
            maintMock.Setup(x => x.Delete(It.Is<FhirRequest>(y => y.RequestingAsid == "badrequest"))).Returns(System.Threading.Tasks.Task.FromResult(OperationOutcomes.Invalid));
            maintMock.Setup(x => x.Delete(It.Is<FhirRequest>(y => y.RequestingAsid == "notfound"))).Returns(System.Threading.Tasks.Task.FromResult(OperationOutcomes.NotFound));

            _nrlsSettings = nrlsSettingsMock.Object;
            _nrlsSearch = searchMock.Object;
            _nrlsMaintain = maintMock.Object;
        }

        public void Dispose()
        {
            _nrlsSettings = null;
            _nrlsSearch = null;
            _nrlsMaintain = null;
        }

        [Fact]
        public async void Search_Valid()
        {

            var controller = new NrlsController(_nrlsSettings, _nrlsSearch, _nrlsMaintain);
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

        [Fact]
        public async void Read_Found()
        {

            var controller = new NrlsController(_nrlsSettings, _nrlsSearch, _nrlsMaintain);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = HttpContexts.Valid_Search;

            var response = await controller.Read("logicalId");

            Assert.IsType<OkObjectResult>(response);

            var okResult = response as OkObjectResult;

            Assert.Equal(200, okResult.StatusCode);

            var responseContent = okResult.Value;

            Assert.IsType<DocumentReference>(responseContent);
            var outcome = responseContent as DocumentReference;

            Assert.Equal(ResourceType.DocumentReference, outcome.ResourceType);
            Assert.Equal("https://directory.spineservices.nhs.uk/STU3/Organization/1XR", outcome.Custodian.Reference);
            Assert.Equal("https://demographics.spineservices.nhs.uk/STU3/Patient/2686033207", outcome.Subject.Reference);
        }

        [Fact]
        public async void Read_NotFound()
        {

            var controller = new NrlsController(_nrlsSettings, _nrlsSearch, _nrlsMaintain);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = HttpContexts.NotFound_Search;

            var response = await controller.Read("logicalId");

            Assert.IsType<NotFoundObjectResult>(response);

            var notFoundResult = response as NotFoundObjectResult;

            Assert.Equal(404, notFoundResult.StatusCode);

            var responseContent = notFoundResult.Value;

            Assert.IsType<OperationOutcome>(responseContent);
            var outcome = responseContent as OperationOutcome;

            Assert.False(outcome.Success);

        }

        [Fact]
        public async void Read_Inactive()
        {

            var controller = new NrlsController(_nrlsSettings, _nrlsSearch, _nrlsMaintain);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = HttpContexts.Inactive_Search;

            var response = await controller.Read("logicalId");

            Assert.IsType<BadRequestObjectResult>(response);

            var result = response as BadRequestObjectResult;

            Assert.Equal(400, result.StatusCode);

            var responseContent = result.Value;

            Assert.IsType<OperationOutcome>(responseContent);
            var outcome = responseContent as OperationOutcome;

            Assert.Equal(1, outcome.Warnings);

        }

        [Fact]
        public async void Delete_Valid()
        {

            var controller = new NrlsController(_nrlsSettings, _nrlsSearch, _nrlsMaintain);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = HttpContexts.Valid_Delete_Pointer;

            var response = await controller.Delete();

            Assert.IsType<OkObjectResult>(response);

            var okResult = response as OkObjectResult;

            Assert.Equal(200, okResult.StatusCode);

            var responseContent = okResult.Value;

            Assert.IsType<OperationOutcome>(responseContent);
            var operationOutcome = responseContent as OperationOutcome;

            Assert.True(operationOutcome.Success);

        }

        [Fact]
        public async void Delete_Invalid()
        {

            var controller = new NrlsController(_nrlsSettings, _nrlsSearch, _nrlsMaintain);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = HttpContexts.Invalid_Delete_Pointer_BadRequest;

            var response = await controller.Delete();

            Assert.IsType<BadRequestObjectResult>(response);

            var badResult = response as BadRequestObjectResult;

            Assert.Equal(400, badResult.StatusCode);

            var responseContent = badResult.Value;

            Assert.IsType<OperationOutcome>(responseContent);
            var operationOutcome = responseContent as OperationOutcome;

            Assert.False(operationOutcome.Success);

            Assert.NotNull(operationOutcome.Issue.FirstOrDefault(x => x.Details.Coding.FirstOrDefault(y => y.Code == "INVALID_RESOURCE") != null));

        }

        [Fact]
        public async void Delete_NotFound()
        {

            var controller = new NrlsController(_nrlsSettings, _nrlsSearch, _nrlsMaintain);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = HttpContexts.Invalid_Delete_Pointer_NotFound;

            var response = await controller.Delete();

            Assert.IsType<NotFoundObjectResult>(response);

            var notfoundResult = response as NotFoundObjectResult;

            Assert.Equal(404, notfoundResult.StatusCode);

            var responseContent = notfoundResult.Value;

            Assert.IsType<OperationOutcome>(responseContent);
            var operationOutcome = responseContent as OperationOutcome;

            Assert.False(operationOutcome.Success);

            Assert.NotNull(operationOutcome.Issue.FirstOrDefault(x => x.Details.Coding.FirstOrDefault(y => y.Code == "NO_RECORD_FOUND") != null));

        }

    }
}
