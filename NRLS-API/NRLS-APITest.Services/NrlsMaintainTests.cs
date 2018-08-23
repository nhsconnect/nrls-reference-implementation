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
using System.Linq;
using MongoDB.Driver;
using MongoDB.Bson;

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

            var searchDocBundle = FhirBundle.GetBundle<DocumentReference>(new List<DocumentReference> { NrlsPointers.Valid_With_Alt_Custodian });
            var searchDocAltBundle = FhirBundle.GetBundle<DocumentReference>(new List<DocumentReference> { NrlsPointers.Valid_With_Alt2_Custodian });
            var searchDocAltInvalidBundle = FhirBundle.GetBundle<DocumentReference>(new List<DocumentReference> { NrlsPointers.Valid_AltCust_With_MasterId_and_RelatesTo_BadStatus });        
            var searchDocEmptyBundle = FhirBundle.GetBundle<DocumentReference>(new List<DocumentReference>());

            var searchMock = new Mock<IFhirSearch>();
            searchMock.Setup(op => op.Find<Organization>(It.Is<FhirRequest>(request => request.RequestingAsid == "000"))).Returns(SystemTasks.Task.Run(() => searchOrgBundle as Resource));
            searchMock.Setup(op => op.Find<Organization>(It.Is<FhirRequest>(request => request.RequestingAsid == "001"))).Returns(SystemTasks.Task.Run(() => searchOrgBundle as Resource));
            searchMock.Setup(op => op.Find<Organization>(It.Is<FhirRequest>(request => request.RequestingAsid == "002"))).Returns(SystemTasks.Task.Run(() => emptySearchBundle as Resource));

            searchMock.Setup(op => op.Find<DocumentReference>(It.Is<FhirRequest>(request => request.RequestingAsid == "000"))).Returns(SystemTasks.Task.Run(() => searchDocBundle as Resource));
            searchMock.Setup(op => op.Find<DocumentReference>(It.Is<FhirRequest>(request => request.RequestingAsid == "001"))).Returns(SystemTasks.Task.Run(() => searchDocAltBundle as Resource));
            searchMock.Setup(op => op.Find<DocumentReference>(It.Is<FhirRequest>(request => request.RequestingAsid == "002"))).Returns(SystemTasks.Task.Run(() => searchDocEmptyBundle as Resource));
            searchMock.Setup(op => op.Find<DocumentReference>(It.Is<FhirRequest>(request => request.RequestingAsid == "003"))).Returns(SystemTasks.Task.Run(() => searchDocAltInvalidBundle as Resource));

            searchMock.Setup(op => op.Get<DocumentReference>(It.IsAny<FhirRequest>())).Returns(SystemTasks.Task.Run(() => searchDocBundle as Resource));
            //searchMock.Setup(op => op.GetByMasterId<DocumentReference>(It.Is<FhirRequest>(request => (request.Resource as DocumentReference).MasterIdentifier.Value == "testValueForMaintTest"))).Returns(SystemTasks.Task.Run(() => searchDocBundle as Resource));
            searchMock.Setup(op => op.GetByMasterId<DocumentReference>(It.IsAny<FhirRequest>())).Returns(SystemTasks.Task.Run(() => searchDocBundle as Resource));

            var maintMock = new Mock<IFhirMaintain>();
            maintMock.Setup(op => op.Create<DocumentReference>(It.Is<FhirRequest>(request => request.RequestingAsid == "000"))).Returns(SystemTasks.Task.Run(() => NrlsPointers.Valid as Resource));
            maintMock.Setup(op => op.Create<DocumentReference>(It.Is<FhirRequest>(request => request.RequestingAsid == "002"))).Returns(SystemTasks.Task.Run(() => null as Resource));

            maintMock.Setup(op => op.CreateWithUpdate<DocumentReference>(It.IsAny<FhirRequest>(), It.Is<FhirRequest>(request => request.Id == "5ab13f41957d0ad5d93a1339"), It.IsAny<UpdateDefinition<BsonDocument>>())).Returns(SystemTasks.Task.Run(() => (created: NrlsPointers.Valid as Resource, updated: true)));
            maintMock.Setup(op => op.CreateWithUpdate<DocumentReference>(It.IsAny<FhirRequest>(), It.Is<FhirRequest>(request => request.Id == "5ab13f41957d0ad5d93a1338"), It.IsAny<UpdateDefinition<BsonDocument>>())).Returns(SystemTasks.Task.Run(() => (created: null as Resource, updated: true)));
            maintMock.Setup(op => op.CreateWithUpdate<DocumentReference>(It.IsAny<FhirRequest>(), It.Is<FhirRequest>(request => request.Id == "5ab13f41957d0ad5d93a1337"), It.IsAny<UpdateDefinition<BsonDocument>>())).Returns(SystemTasks.Task.Run(() => (created: NrlsPointers.Valid as Resource, updated: false)));

            maintMock.Setup(op => op.CreateWithUpdate<DocumentReference>(It.IsAny<FhirRequest>(), It.Is<FhirRequest>(request => request.Id == "5ab13f41957d0ad5d93a1336"), It.IsAny<UpdateDefinition<BsonDocument>>())).ThrowsAsync(new HttpFhirException());


            maintMock.Setup(op => op.Delete<DocumentReference>(It.Is<FhirRequest>(request => request.RequestingAsid == "003"))).Returns(SystemTasks.Task.Run(() => true));
            maintMock.Setup(op => op.Delete<DocumentReference>(It.Is<FhirRequest>(request => request.RequestingAsid == "001"))).Returns(SystemTasks.Task.Run(() => false));
            maintMock.Setup(op => op.DeleteConditional<DocumentReference>(It.Is<FhirRequest>(request => request.RequestingAsid == "003"))).Returns(SystemTasks.Task.Run(() => true));
            maintMock.Setup(op => op.DeleteConditional<DocumentReference>(It.Is<FhirRequest>(request => request.RequestingAsid == "001"))).Returns(SystemTasks.Task.Run(() => false));

            var validationMock = new Mock<IFhirValidation>();
            validationMock.Setup(op => op.ValidPointer(It.Is<DocumentReference>(pointer => pointer.Id == "5ab13f41957d0ad5d93a1339"))).Returns(OperationOutcomes.Ok);
            validationMock.Setup(op => op.ValidPointer(It.Is<DocumentReference>(pointer => pointer.Id == "5affc72bcae33eb8690e5881"))).Returns(OperationOutcomes.Error);

            validationMock.Setup(op => op.GetOrganizationReferenceId(It.IsAny<ResourceReference>())).Returns("TestOrgCode");
            validationMock.Setup(op => op.GetOrganizationReferenceId(It.Is<ResourceReference>(reference => reference.Reference == "https://directory.spineservices.nhs.uk/STU3/Organization/1XR"))).Returns("TestOrgCode");
            validationMock.Setup(op => op.GetOrganizationReferenceId(It.Is<ResourceReference>(reference => reference.Reference == "https://directory.spineservices.nhs.uk/STU3/Organization/error"))).Returns("TestOrgCode2");
            validationMock.Setup(op => op.GetOrganizationReferenceId(It.Is<ResourceReference>(reference => reference.Reference == "https://directory.spineservices.nhs.uk/STU3/Organization/RV99"))).Returns("RV99");
            validationMock.Setup(op => op.GetOrganizationReferenceId(It.Is<ResourceReference>(reference => reference.Reference == "https://directory.spineservices.nhs.uk/STU3/Organization/RV88"))).Returns("RV88");

            validationMock.Setup(op => op.ValidateIdentifierParameter(It.IsAny<string>(), It.IsAny<string>())).Returns(OperationOutcomes.Error);
            validationMock.Setup(op => op.ValidateIdentifierParameter(It.IsAny<string>(), It.Is<string>(p => p == "testsystem|testvalue"))).Returns(delegate { return null; });

            validationMock.Setup(op => op.ValidatePatientParameter(It.IsAny<string>())).Returns(OperationOutcomes.Error);
            validationMock.Setup(op => op.ValidatePatientParameter(It.Is<string>(p => p == "https://demographics.spineservices.nhs.uk/STU3/Patient/2686033207"))).Returns(delegate { return null; });

            validationMock.Setup(op => op.GetValidRelatesTo(It.Is<IList<DocumentReference.RelatesToComponent>>(r => r.FirstOrDefault(w => w.Target != null && w.Target.Identifier != null && w.Target.Identifier.Value == "urn:tag:humber.nhs.uk,2004:cdc:600009612669​") != null))).Returns(FhirResources.Valid_Single_RelatesTo);

            validationMock.Setup(op => op.GetSubjectReferenceId(It.IsAny<ResourceReference>())).Returns("2686033207");

            var clientMapCache = new ClientAsidMap
            {
                ClientAsids = new Dictionary<string, ClientAsid>()
                {
                    { "000", new ClientAsid { Interactions = new List<string>(), OrgCode = "TestOrgCode", Thumbprint = "TestThumbprint" } },
                    { "002", new ClientAsid { Interactions = new List<string>(), OrgCode = "TestOrgCode2", Thumbprint = "TestThumbprint" } },
                    { "003", new ClientAsid { Interactions = new List<string>(), OrgCode = "RV99", Thumbprint = "TestThumbprint" } }

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
        public void BuildCreate_Valid()
        {
            var service = new NrlsMaintain(_nrlsApiSettings, _fhirMaintain, _fhirSearch, _cache, _fhirValidation);

            var response = service.SetMetaValues(FhirRequests.Valid_Create, null);

            Assert.IsType<FhirRequest>(response);

            Assert.NotNull(response.Resource);
            Assert.NotNull(response.Resource.Meta);

            Assert.Equal("1", response.Resource.Meta.VersionId);
        }

        [Fact]
        public void BuildCreate_ValidVersion()
        {
            var service = new NrlsMaintain(_nrlsApiSettings, _fhirMaintain, _fhirSearch, _cache, _fhirValidation);

            var response = service.SetMetaValues(FhirRequests.Valid_Create, "2");

            Assert.IsType<FhirRequest>(response);

            Assert.NotNull(response.Resource);
            Assert.NotNull(response.Resource.Meta);

            Assert.Equal("3", response.Resource.Meta.VersionId);
        }

        [Fact]
        public void BuildCreate_InvalidVersion()
        {
            var service = new NrlsMaintain(_nrlsApiSettings, _fhirMaintain, _fhirSearch, _cache, _fhirValidation);

            Assert.Throws<HttpFhirException>(delegate
            {
                var response = service.SetMetaValues(FhirRequests.Valid_Create, "bad-number");

            });
        }

        [Fact]
        public async void Create_Valid()
        {
            var service = new NrlsMaintain(_nrlsApiSettings, _fhirMaintain, _fhirSearch, _cache, _fhirValidation);

            var response = await service.CreateWithoutValidation<DocumentReference>(FhirRequests.Valid_Create);

            Assert.IsType<DocumentReference>(response);
        }

        [Fact]
        public async void Create_Invalid()
        {
            var service = new NrlsMaintain(_nrlsApiSettings, _fhirMaintain, _fhirSearch, _cache, _fhirValidation);

            var response = await service.CreateWithoutValidation<DocumentReference>(FhirRequests.Invalid_Custodian);

            Assert.IsType<OperationOutcome>(response);

            var outcome = response as OperationOutcome;

            Assert.NotNull(outcome.Issue);
            Assert.NotEmpty(outcome.Issue);

            var details = outcome.Issue.FirstOrDefault().Details;

            Assert.NotNull(details);
            Assert.NotNull(details.Coding);
            Assert.NotEmpty(details.Coding);

            var errorDetail = details.Coding.FirstOrDefault();

            Assert.Equal("INVALID_RESOURCE", errorDetail.Code);

        }

        [Fact]
        public async void ValidateCreate_Valid()
        {
            var service = new NrlsMaintain(_nrlsApiSettings, _fhirMaintain, _fhirSearch, _cache, _fhirValidation);

            var validation = await service.ValidateCreate<DocumentReference>(FhirRequests.Valid_Create);

            Assert.Null(validation);

        }

        [Fact]
        public void ValidateCreate_Invalid_Pointer()
        {
            var service = new NrlsMaintain(_nrlsApiSettings, _fhirMaintain, _fhirSearch, _cache, _fhirValidation);


            Assert.ThrowsAsync<HttpFhirException>(async delegate
            {
                await SystemTasks.Task.Run(async () => 
                {
                    var validation = await service.ValidateCreate<DocumentReference>(FhirRequests.Invalid_Create);
                });

            });
        }

        [Fact]
        public async void ValidateCreate_Invalid_Asid()
        {
            var service = new NrlsMaintain(_nrlsApiSettings, _fhirMaintain, _fhirSearch, _cache, _fhirValidation);

            var validation = await service.ValidateCreate<DocumentReference>(FhirRequests.Valid_Create_Alt);

            Assert.IsType<OperationOutcome>(validation);
        }

        [Fact]
        public async void ValidateCreate_Invalid_Custodian()
        {
            var service = new NrlsMaintain(_nrlsApiSettings, _fhirMaintain, _fhirSearch, _cache, _fhirValidation);

            var validation = await service.ValidateCreate<DocumentReference>(FhirRequests.Invalid_Custodian);

            Assert.IsType<OperationOutcome>(validation);
        }

        [Fact]
        public async void ValidateCreate_Invalid_MasterIdentifier()
        {
            var service = new NrlsMaintain(_nrlsApiSettings, _fhirMaintain, _fhirSearch, _cache, _fhirValidation);

            var validation = await service.ValidateCreate<DocumentReference>(FhirRequests.Valid_Create_MasterId);

            Assert.IsType<OperationOutcome>(validation);
        }

        [Fact]
        public async void ValidateConditionalUpdate_Valid()
        {
            var service = new NrlsMaintain(_nrlsApiSettings, _fhirMaintain, _fhirSearch, _cache, _fhirValidation);

            var response = await service.ValidateConditionalUpdate(FhirRequests.Valid_Update);

            Assert.IsType<DocumentReference>(response);
        }

        [Fact]
        public async void ValidateConditionalUpdate_Valid_NoRelatesTo()
        {
            var service = new NrlsMaintain(_nrlsApiSettings, _fhirMaintain, _fhirSearch, _cache, _fhirValidation);

            var response = await service.ValidateConditionalUpdate(FhirRequests.Valid_Create);

            Assert.Null(response);
        }

        [Fact]
        public async void ValidateConditionalUpdate_Invalid_BadResourceType()
        {
            var service = new NrlsMaintain(_nrlsApiSettings, _fhirMaintain, _fhirSearch, _cache, _fhirValidation);

            var request = FhirRequests.Valid_Update;
            request.Resource = new Patient();

            var response = await service.ValidateConditionalUpdate(request);

            Assert.IsType<OperationOutcome>(response);

            var outcome = response as OperationOutcome;

            Assert.NotNull(outcome.Issue);
            Assert.NotEmpty(outcome.Issue);

            var details = outcome.Issue.FirstOrDefault().Details;

            Assert.NotNull(details);
            Assert.NotNull(details.Coding);
            Assert.NotEmpty(details.Coding);

            var errorDetail = details.Coding.FirstOrDefault();

            Assert.Equal("INVALID_RESOURCE", errorDetail.Code);
        }

        [Fact]
        public async void ValidateConditionalUpdate_Invalid_RelatesTo()
        {
            var service = new NrlsMaintain(_nrlsApiSettings, _fhirMaintain, _fhirSearch, _cache, _fhirValidation);

            var response = await service.ValidateConditionalUpdate(FhirRequests.Invalid_Update_Bad_RelatesTo);

            Assert.IsType<OperationOutcome>(response);

            var outcome = response as OperationOutcome;

            Assert.NotNull(outcome.Issue);
            Assert.NotEmpty(outcome.Issue);

            var details = outcome.Issue.FirstOrDefault().Details;

            Assert.NotNull(details);
            Assert.NotNull(details.Coding);
            Assert.NotEmpty(details.Coding);

            var errorDetail = details.Coding.FirstOrDefault();

            Assert.Equal("INVALID_RESOURCE", errorDetail.Code);
        }

        [Fact]
        public async void ValidateConditionalUpdate_Invalid_RelatesTo_NotFound()
        {
            var service = new NrlsMaintain(_nrlsApiSettings, _fhirMaintain, _fhirSearch, _cache, _fhirValidation);

            var response = await service.ValidateConditionalUpdate(FhirRequests.Valid_Update_Alt2);

            Assert.IsType<OperationOutcome>(response);

            var outcome = response as OperationOutcome;

            Assert.NotNull(outcome.Issue);
            Assert.NotEmpty(outcome.Issue);

            var details = outcome.Issue.FirstOrDefault().Details;

            Assert.NotNull(details);
            Assert.NotNull(details.Coding);
            Assert.NotEmpty(details.Coding);

            var errorDetail = details.Coding.FirstOrDefault();

            Assert.Equal("INVALID_RESOURCE", errorDetail.Code);
        }

        [Fact]
        public async void ValidateConditionalUpdate_Invalid_Incorrect_Custodian()
        {
            var service = new NrlsMaintain(_nrlsApiSettings, _fhirMaintain, _fhirSearch, _cache, _fhirValidation);

            var response = await service.ValidateConditionalUpdate(FhirRequests.Valid_Update_Alt);

            Assert.IsType<OperationOutcome>(response);

            var outcome = response as OperationOutcome;

            Assert.NotNull(outcome.Issue);
            Assert.NotEmpty(outcome.Issue);

            var details = outcome.Issue.FirstOrDefault().Details;

            Assert.NotNull(details);
            Assert.NotNull(details.Coding);
            Assert.NotEmpty(details.Coding);

            var errorDetail = details.Coding.FirstOrDefault();

            Assert.Equal("INVALID_RESOURCE", errorDetail.Code);
        }

        [Fact]
        public async void ValidateConditionalUpdate_Invalid_Status()
        {
            var service = new NrlsMaintain(_nrlsApiSettings, _fhirMaintain, _fhirSearch, _cache, _fhirValidation);

            var response = await service.ValidateConditionalUpdate(FhirRequests.Invalid_Update_Bad_Status);

            Assert.IsType<OperationOutcome>(response);

            var outcome = response as OperationOutcome;

            Assert.NotNull(outcome.Issue);
            Assert.NotEmpty(outcome.Issue);

            var details = outcome.Issue.FirstOrDefault().Details;

            Assert.NotNull(details);
            Assert.NotNull(details.Coding);
            Assert.NotEmpty(details.Coding);

            var errorDetail = details.Coding.FirstOrDefault();

            Assert.Equal("INVALID_RESOURCE", errorDetail.Code);
        }

        [Fact]
        public async void Supersede_Valid()
        {
            var service = new NrlsMaintain(_nrlsApiSettings, _fhirMaintain, _fhirSearch, _cache, _fhirValidation);

            var response = await service.SupersedeWithoutValidation<DocumentReference>(FhirRequests.Valid_Create, "5ab13f41957d0ad5d93a1339", "1");

            Assert.IsType<DocumentReference>(response);
        }

        [Fact]
        public async void Supersede_Invalid_Create()
        {
            var service = new NrlsMaintain(_nrlsApiSettings, _fhirMaintain, _fhirSearch, _cache, _fhirValidation);

            var response = await service.SupersedeWithoutValidation<DocumentReference>(FhirRequests.Valid_Create, "5ab13f41957d0ad5d93a1338", "1");

            Assert.IsType<OperationOutcome>(response);

            var outcome = response as OperationOutcome;

            Assert.False(outcome.Success);

            Assert.NotNull(outcome.Issue);
            Assert.NotEmpty(outcome.Issue);

            var issue = outcome.Issue.FirstOrDefault();

            Assert.Equal("Resource is invalid : Unknown", issue.Diagnostics);

        }

        [Fact]
        public async void Supersede_Invalid_Update()
        {
            var service = new NrlsMaintain(_nrlsApiSettings, _fhirMaintain, _fhirSearch, _cache, _fhirValidation);

            var response = await service.SupersedeWithoutValidation<DocumentReference>(FhirRequests.Valid_Create, "5ab13f41957d0ad5d93a1337", "1");

            Assert.IsType<OperationOutcome>(response);

            var outcome = response as OperationOutcome;

            Assert.False(outcome.Success);

            Assert.NotNull(outcome.Issue);
            Assert.NotEmpty(outcome.Issue);

            var issue = outcome.Issue.FirstOrDefault();

            Assert.Equal("Resource is invalid : relatesTo", issue.Diagnostics);

        }

        [Fact]
        public async void Supersede_Fatal_ThrowsException()
        {
            var service = new NrlsMaintain(_nrlsApiSettings, _fhirMaintain, _fhirSearch, _cache, _fhirValidation);

            var exception = await Assert.ThrowsAsync<HttpFhirException>(async delegate
            {
                var response = await service.SupersedeWithoutValidation<DocumentReference>(FhirRequests.Valid_Create, "5ab13f41957d0ad5d93a1336", "1");

            });

            Assert.Equal("Error Updating DocumentReference", exception.Message);

            Assert.NotNull(exception.OperationOutcome);

            var outcome = exception.OperationOutcome;

            Assert.False(outcome.Success);

            Assert.NotNull(outcome.Issue);
            Assert.NotEmpty(outcome.Issue);

            var issue = outcome.Issue.FirstOrDefault();

            Assert.StartsWith("There has been an internal error when attempting to persist the DocumentReference. Please contact the national helpdesk quoting -", issue.Diagnostics);
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
        public async void Delete_Invalid()
        {
            var service = new NrlsMaintain(_nrlsApiSettings, _fhirMaintain, _fhirSearch, _cache, _fhirValidation);

            var response = await service.Delete<DocumentReference>(FhirRequests.Valid_Delete_Alt);

            Assert.IsType<OperationOutcome>(response);

            Assert.False(response.Success);
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

        [Fact]
        public async void ConditionalDelete_Valid()
        {
            var service = new NrlsMaintain(_nrlsApiSettings, _fhirMaintain, _fhirSearch, _cache, _fhirValidation);

            var response = await service.Delete<DocumentReference>(FhirRequests.Valid_ConditionalDelete);

            Assert.IsType<OperationOutcome>(response);

            Assert.True(response.Success);
        }

        [Fact]
        public async void ConditionalDelete_Invalid()
        {
            var service = new NrlsMaintain(_nrlsApiSettings, _fhirMaintain, _fhirSearch, _cache, _fhirValidation);

            var response = await service.Delete<DocumentReference>(FhirRequests.Invalid_ConditionalDelete);

            Assert.IsType<OperationOutcome>(response);

            Assert.False(response.Success);
        }

        [Fact]
        public void ConditionalDelete_Invalid_Subject()
        {
            var service = new NrlsMaintain(_nrlsApiSettings, _fhirMaintain, _fhirSearch, _cache, _fhirValidation);

            Assert.ThrowsAsync<HttpFhirException>(async delegate
            {
                await SystemTasks.Task.Run(async () => {
                    var response = await service.Delete<DocumentReference>(FhirRequests.Invalid_ConditionalDelete_NoSubject);
                });

            });
        }

        [Fact]
        public void ConditionalDelete_Invalid_Identifier()
        {
            var service = new NrlsMaintain(_nrlsApiSettings, _fhirMaintain, _fhirSearch, _cache, _fhirValidation);


            Assert.ThrowsAsync<HttpFhirException>(async delegate
            {
                await SystemTasks.Task.Run(async () => {
                    var response = await service.Delete<DocumentReference>(FhirRequests.Invalid_ConditionalDelete_IncompleteIdentifier);
                });

            });
        }
    }
}
