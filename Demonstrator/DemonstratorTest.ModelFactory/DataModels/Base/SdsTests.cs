using Demonstrator.Models.DataModels.Base;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DemonstratorTest.ModelFactory.DataModels.Base
{
    public class SdsTests
    {
        [Fact]
        public void Sds_ToViewModel()
        {
            var model = new Sds
            {
                Id = new ObjectId("5cb5ddefc892d7e5f2911909"),
                PartyKey = Guid.Parse("1e5ed8d1-438c-4e6a-8edf-bcb1f739ad5e"),
                Fqdn = "fqdn.com",
                EndPoints = new List<Uri> { new Uri("http://uri.com/hello") },
                OdsCode = "OrgCode1",
                Interactions = new List<string> { "InterActionId:Create" },
                Asid = 100000000099,
                Thumbprint = "CertThumb1",
                Active = true
            };

            var viewModel = Sds.ToViewModel(model);

            Assert.Equal("5cb5ddefc892d7e5f2911909", viewModel.Id);
            Assert.Equal(Guid.Parse("1e5ed8d1-438c-4e6a-8edf-bcb1f739ad5e"), viewModel.PartyKey);
            Assert.Equal("fqdn.com", viewModel.Fqdn);
            Assert.NotNull(viewModel.EndPoints);
            Assert.NotEmpty(viewModel.EndPoints);
            Assert.Equal("/hello", viewModel.EndPoints.ElementAt(0).AbsolutePath);
            Assert.Equal("OrgCode1", viewModel.OdsCode);
            Assert.NotNull(viewModel.Interactions);
            Assert.NotEmpty(viewModel.Interactions);
            Assert.Equal("InterActionId:Create", viewModel.Interactions.ElementAt(0));
            Assert.Equal("100000000099", viewModel.Asid);
            Assert.Equal("CertThumb1", viewModel.Thumbprint);
            Assert.True(viewModel.Active);
        }

        [Fact]
        public void Sds_ToViewModelHandlesNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var viewModel = Sds.ToViewModel(null);
            });
        }

        [Fact]
        public void Sds_CorrectCacheKey()
        {
            Assert.Equal("Sds", Sds.CacheKey);
        }

    }
}
