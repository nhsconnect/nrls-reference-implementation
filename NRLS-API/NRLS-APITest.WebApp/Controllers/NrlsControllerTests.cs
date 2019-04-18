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
using Xunit;

namespace NRLS_APITest.WebApp.Middlewares
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
            searchMock.Setup(x => x.Find<DocumentReference>(It.Is<FhirRequest>(y => y.RequestingAsid == "000"))).Returns(System.Threading.Tasks.Task.FromResult(FhirBundle.GetBundle(pointerList) as Resource));
            searchMock.Setup(x => x.Find<DocumentReference>(It.Is<FhirRequest>(y => y.RequestingAsid == "notfound"))).Returns(System.Threading.Tasks.Task.FromResult(OperationOutcomes.NotFound as Resource));

            var maintMock = new Mock<INrlsMaintain>();
            //maintMock.Setup(x => x.Find<DocumentReference>(It.IsAny<FhirRequest>())).Returns(System.Threading.Tasks.Task.FromResult(new Bundle() as Resource));


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
        public async void Search_NotFoun()
        {

            var controller = new NrlsController(_nrlsSettings, _nrlsSearch, _nrlsMaintain);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = HttpContexts.NotFound_Search;

            var response = await controller.Search();

            Assert.IsType<NotFoundObjectResult>(response);

            var notFoundResult = response as NotFoundObjectResult;

            Assert.Equal(404, notFoundResult.StatusCode);

            var responseContent = notFoundResult.Value;

            Assert.IsType<OperationOutcome>(responseContent);
            var outcome = responseContent as OperationOutcome;

            Assert.False(outcome.Success);

        }

    }
}
