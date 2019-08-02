using Moq;
using NRLS_API.Models.Core;
using NRLS_API.Services;
using NRLS_APITest.Data;
using System;
using System.Collections.Generic;
using Xunit;
using Microsoft.Extensions.Caching.Memory;
using NRLS_APITest.StubClasses;
using NRLS_API.Models.ViewModels.Core;
using MongoDB.Bson;
using MongoDB.Driver;
using NRLS_API.Core.Interfaces.Database;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace NRLS_APITest.Services
{
    public class SdsServiceTests : IDisposable
    {

        private INRLSMongoDBCaller _nrlsMongoDBCaller;
        private IMemoryCache _cache;

        public SdsServiceTests()
        {

            IEnumerable<SdsViewModel> testBsons = new List<SdsViewModel> {
                new SdsViewModel  {
                    Id = "5cb5fd22c892d7e5f291190a",
                    Asid = "000",
                    OdsCode = "TestOrgCode",
                    Interactions = new List<string> { "urn:nhs:names:services:nrl:DocumentReferenceRead.read" },
                    Thumbprint = "TestThumbprint",
                    Active = true,
                    Fqdn = "fqdn.com",
                    PartyKey = Guid.Parse("0fc80dba-58c4-4dd2-9827-af724ea4eb92"),
                    EndPoints = new List<Uri> { new Uri("http://fqdn.com/hello") }
                },
                new SdsViewModel  {
                    Id = "5cb5fdbdc892d7e5f291190b",
                    Asid = "002",
                    OdsCode = "TestOrgCode2",
                    Interactions = new List<string> (),
                    Thumbprint = "TestThumbprint",
                    Active = true,
                    Fqdn = "fqdn2.com",
                    PartyKey = Guid.Parse("158a8f7f-7d62-445a-ba26-947f741f5a74"),
                    EndPoints = new List<Uri> { new Uri("http://fqdn2.com/mello") }
                }
            };

            var nrlsMongoDBCaller = new Mock<INRLSMongoDBCaller>();
            nrlsMongoDBCaller.Setup(m => m.FindSds(It.IsAny<FilterDefinition<Sds>>())).Returns(Task.Run(() => testBsons));

            _nrlsMongoDBCaller = nrlsMongoDBCaller.Object;


            var sdsCache = new List<SdsViewModel>
            {
                SdsViewModels.SdsAsid000,
                SdsViewModels.SdsAsid002
            };

            var cacheMock = MemoryCacheStub.MockMemoryCacheService.GetMemoryCache(sdsCache);

            _cache = cacheMock;
        }

        public void Dispose()
        {
            _nrlsMongoDBCaller = null;
            _cache = null;
        }


        [Fact]
        public void SdsService_GetForAsid()
        {
            var service = new SdsService(_nrlsMongoDBCaller, _cache);

            var result = service.GetFor("000");

            Assert.NotNull(result);

            Assert.Equal("5cb5fd22c892d7e5f291190a", result.Id);
            Assert.Equal("000", result.Asid);
            Assert.Equal("TestOrgCode", result.OdsCode);
        }

        [Fact]
        public void SdsService_GetForAsidNull()
        {
            var service = new SdsService(_nrlsMongoDBCaller, _cache);

            var result = service.GetFor("999");

            Assert.Null(result);
        }

        [Fact]
        public void SdsService_GetForOrgCode()
        {
            var service = new SdsService(_nrlsMongoDBCaller, _cache);

            var result = service.GetFor("TestOrgCode", null);

            Assert.NotNull(result);

            Assert.Equal("5cb5fd22c892d7e5f291190a", result.Id);
            Assert.Equal("000", result.Asid);
            Assert.Equal("TestOrgCode", result.OdsCode);
        }

        [Fact]
        public void SdsService_GetForOrgCode2()
        {
            var service = new SdsService(_nrlsMongoDBCaller, _cache);

            var result = service.GetFor("TestOrgCode2", null);

            Assert.NotNull(result);

            Assert.Equal("5cb5fdbdc892d7e5f291190b", result.Id);
            Assert.Equal("002", result.Asid);
            Assert.Equal("TestOrgCode2", result.OdsCode);
        }

        [Fact]
        public void SdsService_GetForOrgNUll()
        {
            var service = new SdsService(_nrlsMongoDBCaller, _cache);

            var result = service.GetFor("TestOrgCodeBlaaa", null);

            Assert.Null(result);
        }

        [Fact]
        public void SdsService_GetForOrgInteraction()
        {
            var service = new SdsService(_nrlsMongoDBCaller, _cache);

            var result = service.GetFor("TestOrgCode", "urn:nhs:names:services:nrl:DocumentReferenceRead.read");

            Assert.NotNull(result);

            Assert.Equal("5cb5fd22c892d7e5f291190a", result.Id);
            Assert.Equal("000", result.Asid);
            Assert.Equal("TestOrgCode", result.OdsCode);
        }

        [Fact]
        public void SdsService_GetForOrgInteractionInvalid()
        {
            var service = new SdsService(_nrlsMongoDBCaller, _cache);

            var result = service.GetFor("TestOrgCode", "urn:nhs:names:services:nrls:fhir:rest:danger:documentreference");

            Assert.Null(result);
        }

        [Fact]
        public async void SdsService_GetAllCache()
        {
            var service = new SdsService(_nrlsMongoDBCaller, _cache);

            var result = await service.GetAll();

            Assert.NotNull(result);
            Assert.NotEmpty(result);

            Assert.Equal(2, result.Count());

            var cache000 = result.FirstOrDefault(x =>x.Asid == "000");

            Assert.NotNull(cache000);

            Assert.Equal("5cb5fd22c892d7e5f291190a", cache000.Id);
            Assert.Equal("000", cache000.Asid);
            Assert.Equal("TestOrgCode", cache000.OdsCode);
            Assert.Null(cache000.Fqdn);
        }

        [Fact]
        public async void SdsService_GetAllSource()
        {
            var service = new SdsService(_nrlsMongoDBCaller, _cache);

            var result = await service.GetAllFromSource();

            Assert.NotNull(result);
            Assert.NotEmpty(result);

            Assert.Equal(2, result.Count());

            var cache000 = result.FirstOrDefault(x => x.Asid == "000");

            Assert.NotNull(cache000);

            Assert.Equal("5cb5fd22c892d7e5f291190a", cache000.Id);
            Assert.Equal("000", cache000.Asid);
            Assert.Equal("TestOrgCode", cache000.OdsCode);
            Assert.Equal("fqdn.com", cache000.Fqdn);
        }
    }
}
