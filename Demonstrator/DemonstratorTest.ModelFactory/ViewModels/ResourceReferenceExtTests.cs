using Demonstrator.Models.ViewModels.Factories;
using Demonstrator.Models.ViewModels.Fhir;
using DemonstratorTest.Data.FhirModels;
using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DemonstratorTest.ModelFactory
{
    public class ResourceReferenceExtTests
    {
        [Fact]
        public void ReferenceViewModel_Returns_ValidViewModel_Full()
        {
            var model = new ResourceReference
            {
                Reference = "reference/abc"
            };

            var viewModel = model.ToViewModel();

            Assert.NotNull(viewModel);

            Assert.Equal("abc", viewModel.Id);
            Assert.Equal("reference/abc", viewModel.Reference);
        }

        [Fact]
        public void ReferenceViewModel_Returns_ValidViewModel_NoSlash()
        {
            var model = new ResourceReference
            {
                Reference = "reference-abc"
            };

            var viewModel = model.ToViewModel();

            Assert.NotNull(viewModel);

            Assert.Equal("reference-abc", viewModel.Id);
            Assert.Equal("reference-abc", viewModel.Reference);
        }

        [Fact]
        public void ReferenceViewModel_Returns_ValidViewModel_NoId()
        {
            var model = new ResourceReference
            {
                Reference = "reference/"
            };

            var viewModel = model.ToViewModel();

            Assert.NotNull(viewModel);

            Assert.Equal(string.Empty, viewModel.Id);
            Assert.Equal("reference/", viewModel.Reference);
        }

        [Fact]
        public void ReferenceViewModel_Returns_InvalidViewModel_Null()
        {
            ResourceReference model = null;

            Assert.Throws<NullReferenceException>(() => model.ToViewModel());
        }

    }
}
