using Hl7.Fhir.Model;
using Moq;
using NRLS_API.Core.Interfaces.Services;
using NRLS_API.Models.Core;
using NRLS_API.Services;
using NRLS_APITest.Comparer;
using NRLS_APITest.Data;
using System;
using System.Collections.Generic;
using SystemTasks = System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Options;
using NRLS_API.Core.Exceptions;
using NRLS_API.Models.ViewModels.Core;

namespace NRLS_APITest.Services
{
    public class NrlsSearchTests : IDisposable
    {

        private IFhirValidation _fhirValidation;
        private IFhirSearch _fhirSearch;
        private IOptionsSnapshot<NrlsApiSetting> _nrlsApiSettings;
        private ISdsService _sdsService;

        private Bundle _expectedBundle;

        public NrlsSearchTests()
        {
            var validationMock = new Mock<IFhirValidation>();
            validationMock.Setup(op => op.ValidatePatientParameter(It.IsAny<string>())).Returns(delegate { return null; });

            validationMock.Setup(op => op.ValidateCustodianParameter(It.IsAny<string>())).Returns(delegate { return null; });
            validationMock.Setup(op => op.ValidateCustodianIdentifierParameter(It.IsAny<string>())).Returns(delegate { return null; });

            validationMock.Setup(op => op.ValidSummaryParameter(It.Is<string>(p => p == "notcount"))).Returns(delegate { return new OperationOutcome(); });
            validationMock.Setup(op => op.ValidSummaryParameter(It.Is<string>(p => p == "count"))).Returns(delegate { return null; });

            validationMock.Setup(op => op.GetOrganizationParameterId(It.Is<string>(p => p == "https://directory.spineservices.nhs.uk/STU3/Organization/TestOrgCode"))).Returns("TestOrgCode");
            validationMock.Setup(op => op.GetOrganizationParameterIdentifierId(It.Is<string>(p => p == "https://fhir.nhs.uk/Id/ods-organization-code|TestOrgCode"))).Returns("TestOrgCode");

            validationMock.Setup(op => op.GetOrganizationParameterIdentifierId(It.Is<string>(p => p == "https://fhir.nhs.uk/Id/ods-organization-code|TestOrgCode"))).Returns("TestOrgCode");

            validationMock.Setup(op => op.GetSubjectReferenceParameterId(It.IsAny<string>())).Returns("1445545101");


            _fhirValidation = validationMock.Object;

            var searchPointer = NrlsPointers.Valid;
            var searchPointerList = new List<DocumentReference> { searchPointer };
            var searchBundle = FhirBundle.GetBundle<DocumentReference>(searchPointerList);

            var searchMock = new Mock<IFhirSearch>();
            searchMock.Setup(op => op.GetAsBundle<DocumentReference>(It.IsAny<FhirRequest>())).Returns(SystemTasks.Task.Run(() => searchBundle));
            searchMock.Setup(op => op.Find<DocumentReference>(It.IsAny<FhirRequest>(), It.IsAny<bool>())).Returns(SystemTasks.Task.Run(() => searchBundle));


            var orgSearchBundle = FhirBundle.GetBundle<Organization>(new List<Organization> { FhirOrganizations.Valid_Organization });
            searchMock.Setup(op => op.Find<Organization>(It.IsAny<FhirRequest>(), It.IsAny<bool>())).Returns(SystemTasks.Task.Run(() => orgSearchBundle));

            var patientSearchBundle = FhirBundle.GetBundle<Patient>(new List<Patient> { FhirPatients.Valid_Patient });
            searchMock.Setup(op => op.Find<Patient>(It.IsAny<FhirRequest>(), It.IsAny<bool>())).Returns(SystemTasks.Task.Run(() => patientSearchBundle));

            var patientSearchEmptyBundle = FhirBundle.GetBundle<Patient>(new List<Patient>());
            searchMock.Setup(op => op.Find<Patient>(It.Is<FhirRequest>(x => x.Id == "invalid-patient-resource-id"), It.IsAny<bool>())).Returns(SystemTasks.Task.Run(() => patientSearchEmptyBundle));

            _fhirSearch = searchMock.Object;


            var opts = new NrlsApiSetting
            {
                BaseUrl = "https://baseurl.com/",
                DefaultPort = "80",
                ProfileUrl = "https://profileurl.com",
                Secure = false,
                SecurePort = "443",
                SupportedContentTypes = new List<string> { "application/fhir+json" },
                SupportedResources = new List<string> { "DocumentReference" }
            };

            var mock = new Mock<IOptionsSnapshot<NrlsApiSetting>>();
            mock.Setup(op => op.Get(It.IsAny<string>())).Returns(opts);

            _nrlsApiSettings = mock.Object;

            _expectedBundle = new Bundle
            {
                Id = "b54e3ad5-04b5-4f8f-8dbd-0e41d2465b5c",
                Meta = new Meta
                {
                    LastUpdated = new DateTimeOffset(new DateTime(2018, 3, 1, 0, 0, 0, DateTimeKind.Utc)),
                    VersionId = "b54e3ad5-04b5-4f8f-8dbd-0e41d2465b5c"
                },
                Type = Bundle.BundleType.Searchset,
                Total = 1,
                Link = new List<Bundle.LinkComponent>
                {
                    new Bundle.LinkComponent
                    {
                        Relation = "_self",
                        Url = "TestLink"
                    }
                },
                Entry = new List<Bundle.EntryComponent>
                {
                    new Bundle.EntryComponent
                    {
                        Search = new Bundle.SearchComponent
                        {
                            Mode = Bundle.SearchEntryMode.Match
                        },
                        FullUrl = $"ResourceLink",
                        Resource = new DocumentReference
                        {
                            Id = "5ab13f41957d0ad5d93a1339",
                            Meta = new Meta
                            {
                                Profile = new List<string> { "https://fhir.nhs.uk/STU3/StructureDefinition/NRL-DocumentReference-1" },
                                LastUpdated = new DateTimeOffset(new DateTime(2018, 3, 1, 0, 0, 0, DateTimeKind.Utc)),
                                VersionId = "1"
                            },
                            Status = DocumentReferenceStatus.Current,
                            Type = new CodeableConcept
                            {
                                Coding = new List<Coding>
                                {
                                    new Coding
                                    {
                                        System = "http://snomed.info/sct",
                                        Code = "736253002",
                                        Display = "Mental health crisis plan (record artifact)"
                                    }
                                }
                            },
                            Subject = new ResourceReference
                            {
                                Reference = "https://demographics.spineservices.nhs.uk/STU3/Patient/2686033207"
                            },
                            Indexed = new DateTime(2005, 12, 24, 9, 43, 41),
                            Author = new List<ResourceReference>
                            {
                                new ResourceReference
                                {
                                    Reference = "https://directory.spineservices.nhs.uk/STU3/Organization/1XR"
                                }
                            },
                            Custodian = new ResourceReference
                            {
                                Reference = "https://directory.spineservices.nhs.uk/STU3/Organization/1XR"
                            },
                            Content = new List<DocumentReference.ContentComponent>
                            {
                                new DocumentReference.ContentComponent
                                {
                                    Attachment = new Attachment
                                    {
                                        ContentType = "application/pdf",
                                        Url = "http://example.org/xds/mhd/Binary/07a6483f-732b-461e-86b6-edb665c45510.pdf",
                                        Title = "Mental health Care Plan Report",
                                        Creation = "2016-03-08T15:26:00+00:00"
                                    }
                                }
                            },
                            Context = new DocumentReference.ContextComponent
                            {
                                PracticeSetting = new CodeableConcept
                                {
                                    Coding = new List<Coding>
                                    {
                                        new Coding
                                        {
                                          System = "http://snomed.info/sct",
                                          Code = "708168004",
                                          Display = "Mental health service"
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            };

            var sdsMock = new Mock<ISdsService>();
            sdsMock.Setup(op => op.GetFor(It.IsAny<string>())).Returns((SdsViewModel)null);
            sdsMock.Setup(op => op.GetFor(It.Is<string>(x => x == "000"))).Returns(SdsViewModels.SdsAsid000);
            sdsMock.Setup(op => op.GetFor(It.Is<string>(x => x == "002"))).Returns(SdsViewModels.SdsAsid002);
            sdsMock.Setup(op => op.GetFor(It.Is<string>(x => x == "003"))).Returns(SdsViewModels.SdsAsid003);
            //sdsMock.Setup(op => op.GetFor(It.IsAny<string>(), null)).Returns((SdsViewModel)null);

            _sdsService = sdsMock.Object;
        }

        public void Dispose()
        {
            _fhirValidation = null;
            _fhirSearch = null;
            _nrlsApiSettings = null;
            _expectedBundle = null;
            _sdsService = null;
        }


        //[Fact]
        //public async void Get_Read_Valid()
        //{
            //var search = new NrlsSearch(_nrlsApiSettings, _fhirSearch, _sdsService, _fhirValidation);

            //var actualBundle = await search.Find(FhirRequests.Valid_Read) as Bundle;

            //Assert.Equal(_expectedBundle, actualBundle, Comparers.ModelComparer<Bundle>());
        //}


        [Fact]
        public async void Find_Read_Valid()
        {
            var search = new NrlsSearch(_nrlsApiSettings, _fhirSearch, _sdsService, _fhirValidation);

            var actualBundle = await search.Find(FhirRequests.Valid_Read) as Bundle;

            Assert.Equal(_expectedBundle, actualBundle, Comparers.ModelComparer<Bundle>());
        }

        [Fact]
        public async void Find_Read_Invalid_Id()
        {
            var search = new NrlsSearch(_nrlsApiSettings, _fhirSearch, _sdsService, _fhirValidation);

            await Assert.ThrowsAsync<HttpFhirException>(async () => { var actualBundle = await search.Find(FhirRequests.Invalid_Read_EmptyId); });

            //TODO check exception thrown
        }

        [Fact]
        public void Find_Read_Invalid_Params()
        {
            var search = new NrlsSearch(_nrlsApiSettings, _fhirSearch, _sdsService, _fhirValidation);

            Assert.ThrowsAsync<HttpFhirException>(async () => { var actual = await search.Find(FhirRequests.Invalid_Read_TooManyParams); });
        }

        [Fact]
        public async void Find_Search_Valid()
        {
            var search = new NrlsSearch(_nrlsApiSettings, _fhirSearch, _sdsService, _fhirValidation);

            var actualBundle = await search.Find(FhirRequests.Valid_Search) as Bundle;

            Assert.Equal(_expectedBundle, actualBundle, Comparers.ModelComparer<Bundle>());
        }

        [Fact]
        public void Find_Search_Invalid()
        {
            var search = new NrlsSearch(_nrlsApiSettings, _fhirSearch, _sdsService, _fhirValidation);

            Assert.ThrowsAsync<HttpFhirException>(async () => { var actual = await search.Find(FhirRequests.Invalid_Search); });
        }

        //TODO: add patient search exception
        //TODO: add custodian search exception


        [Fact]
        public async void Find_Search_Valid_Summary()
        {
            var search = new NrlsSearch(_nrlsApiSettings, _fhirSearch, _sdsService, _fhirValidation);

            var actualBundle = await search.Find(FhirRequests.Valid_Search_Summary) as Bundle;

            Assert.Equal(_expectedBundle, actualBundle, Comparers.ModelComparer<Bundle>());
        }

        [Fact]
        public void Find_Search_Invalid_Summary()
        {
            var search = new NrlsSearch(_nrlsApiSettings, _fhirSearch, _sdsService, _fhirValidation);


            Assert.ThrowsAsync<HttpFhirException>(async () => 
            {
                var actualBundle = await search.Find(FhirRequests.Invalid_Search_IncorrectSummary);
            });

        }

        [Fact]
        public void Find_Search_Invalid_Patient()
        {
            var search = new NrlsSearch(_nrlsApiSettings, _fhirSearch, _sdsService, _fhirValidation);


            Assert.ThrowsAsync<HttpFhirException>(async () =>
            {
                var actualBundle = await search.Find(FhirRequests.Invalid_Search_Invalid_Patient);
            });

        }

    }
}
