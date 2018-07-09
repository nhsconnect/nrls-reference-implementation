using Hl7.Fhir.Model;
using Microsoft.Extensions.Options;
using Moq;
using NRLS_API.Core.Interfaces.Services;
using NRLS_API.Models.Core;
using NRLS_API.Services;
using NRLS_APITest.Data;
using System;
using System.Collections.Generic;
using SystemTasks = System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Caching.Memory;
using NRLS_API.Core.Exceptions;
using NRLS_APITest.StubClasses;

namespace NRLS_APITest.Services
{
    public class NrlsMaintainTests : IDisposable
    {
        IOptionsSnapshot<NrlsApiSetting> _nrlsApiSettings;
        IFhirMaintain _fhirMaintain;
        IFhirSearch _fhirSearch;
        IMemoryCache _cache;
        IFhirValidation _fhirValidation;

        public NrlsMaintainTests()
        {
            var opts = AppSettings.NrlsApiSettings;
            var settingsMock = new Mock<IOptionsSnapshot<NrlsApiSetting>>();
            settingsMock.Setup(op => op.Get(It.IsAny<string>())).Returns(opts);

            var searchOrgList = new List<Organization> { FhirOrganizations.Valid_Organization };
            var searchOrgBundle = FhirBundle.GetBundle<Organization>(searchOrgList);
            var emptySearchBundle = FhirBundle.GetBundle<Organization>(new List<Organization>());

            var searchDocList = new List<DocumentReference> { NrlsPointers.Valid };
            var searchDocBundle = FhirBundle.GetBundle<DocumentReference>(searchDocList);

            var searchMock = new Mock<IFhirSearch>();
            searchMock.Setup(op => op.Find<Organization>(It.Is<FhirRequest>(request => request.RequestingAsid == "000"))).Returns(SystemTasks.Task.Run(() => searchOrgBundle as Resource));
            searchMock.Setup(op => op.Find<Organization>(It.Is<FhirRequest>(request => request.RequestingAsid == "001"))).Returns(SystemTasks.Task.Run(() => searchOrgBundle as Resource));
            searchMock.Setup(op => op.Find<Organization>(It.Is<FhirRequest>(request => request.RequestingAsid == "002"))).Returns(SystemTasks.Task.Run(() => emptySearchBundle as Resource));
            searchMock.Setup(op => op.Get<DocumentReference>(It.IsAny<FhirRequest>())).Returns(SystemTasks.Task.Run(() => searchDocBundle as Resource));

            var maintMock = new Mock<IFhirMaintain>();
            maintMock.Setup(op => op.Create<DocumentReference>(It.IsAny<FhirRequest>())).Returns(SystemTasks.Task.Run(() => NrlsPointers.Valid as Resource));
            maintMock.Setup(op => op.Delete<DocumentReference>(It.Is<FhirRequest>(request => request.RequestingAsid == "000"))).Returns(SystemTasks.Task.Run(() => OperationOutcomes.Ok));
            maintMock.Setup(op => op.Delete<DocumentReference>(It.Is<FhirRequest>(request => request.RequestingAsid == "001"))).Returns(SystemTasks.Task.Run(() => OperationOutcomes.Error));

            var validationMock = new Mock<IFhirValidation>();
            validationMock.Setup(op => op.ValidPointer(It.Is<DocumentReference>(pointer => pointer.Id == "5ab13f41957d0ad5d93a1339"))).Returns(OperationOutcomes.Ok);
            validationMock.Setup(op => op.ValidPointer(It.Is<DocumentReference>(pointer => pointer.Id == "5affc72bcae33eb8690e5881"))).Returns(OperationOutcomes.Error);
            validationMock.Setup(op => op.GetOrganizationReferenceId(It.Is<ResourceReference>(reference => reference.Reference == "https://directory.spineservices.nhs.uk/STU3/Organization/1XR"))).Returns("TestOrgCode");
            validationMock.Setup(op => op.GetOrganizationReferenceId(It.Is<ResourceReference>(reference => reference.Reference == "https://directory.spineservices.nhs.uk/STU3/Organization/error"))).Returns("TestOrgCode2");
            validationMock.Setup(op => op.GetOrganizationReferenceId(It.IsAny<ResourceReference>())).Returns("TestOrgCode");

            var clientMapCache = new ClientAsidMap
            {
                ClientAsids = new Dictionary<string, ClientAsid>()
                {
                    { "000", new ClientAsid { Interactions = new List<string>(), OrgCode = "TestOrgCode", Thumbprint = "TestThumbprint" } },
                    { "002", new ClientAsid { Interactions = new List<string>(), OrgCode = "TestOrgCode2", Thumbprint = "TestThumbprint" } }

                }
            };

            var cacheMock = MemoryCacheStub.MockMemoryCacheService.GetMemoryCache(clientMapCache);

            _nrlsApiSettings = settingsMock.Object;
            _fhirMaintain = maintMock.Object;
            _fhirSearch = searchMock.Object;
            _fhirValidation = validationMock.Object;
            _cache = cacheMock;
        }

        public void Dispose()
        {
            _nrlsApiSettings = null;
            _fhirMaintain = null;
            _fhirSearch = null;
            _fhirValidation = null;
            _cache = null;
        }

        [Fact]
        public async void Create_Valid()
        {
            var service = new NrlsMaintain(_nrlsApiSettings, _fhirMaintain, _fhirSearch, _cache, _fhirValidation);

            var response = await service.Create<DocumentReference>(FhirRequests.Valid_Create);

            Assert.IsType<DocumentReference>(response);

        }

        [Fact]
        public void Create_Invalid_Pointer()
        {
            var service = new NrlsMaintain(_nrlsApiSettings, _fhirMaintain, _fhirSearch, _cache, _fhirValidation);


            Assert.ThrowsAsync<HttpFhirException>(async delegate
            {
                await SystemTasks.Task.Run(async () => { var response = await service.Create<DocumentReference>(FhirRequests.Invalid_Create); });

            });
        }

        [Fact]
        public async void Create_Invalid_Asid()
        {
            var service = new NrlsMaintain(_nrlsApiSettings, _fhirMaintain, _fhirSearch, _cache, _fhirValidation);


             var response = await service.Create<DocumentReference>(FhirRequests.Valid_Create_Alt);

            Assert.IsType<OperationOutcome>(response);
        }

        [Fact]
        public async void Create_Invalid_Custodian()
        {
            var service = new NrlsMaintain(_nrlsApiSettings, _fhirMaintain, _fhirSearch, _cache, _fhirValidation);


            var response = await service.Create<DocumentReference>(FhirRequests.Invalid_Custodian);

            Assert.IsType<OperationOutcome>(response);
        }

        [Fact]
        public async void Delete_Valid()
        {
            var service = new NrlsMaintain(_nrlsApiSettings, _fhirMaintain, _fhirSearch, _cache, _fhirValidation);

            var response = await service.Delete<DocumentReference>(FhirRequests.Valid_Delete);

            Assert.IsType<OperationOutcome>(response);

            Assert.True(response.Success);
        }

        [Fact]
        public void Delete_Invalid_Id()
        {
            var service = new NrlsMaintain(_nrlsApiSettings, _fhirMaintain, _fhirSearch, _cache, _fhirValidation);


            Assert.ThrowsAsync<HttpFhirException>(async delegate
            {
                await SystemTasks.Task.Run(async () => {
                    var response = await service.Delete<DocumentReference>(FhirRequests.Invalid_Delete);
                });

            });
        }

        [Fact]
        public async void Delete_Invalid_Asid()
        {
            var service = new NrlsMaintain(_nrlsApiSettings, _fhirMaintain, _fhirSearch, _cache, _fhirValidation);

            var response = await service.Delete<DocumentReference>(FhirRequests.Valid_Delete_Alt);

            Assert.IsType<OperationOutcome>(response);

            Assert.False(response.Success);
        }
    }
}
