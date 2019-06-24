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
    public class IdentifierExtTests
    {
        [Fact]
        public void IdentifierViewModel_Returns_ValidViewModel_Full()
        {
            var model = new Identifier
            {
                System = "system1",
                Value = "value1"
            };

            var viewModel = model.ToViewModel();

            Assert.NotNull(viewModel);

            Assert.Equal("system1", viewModel.System);
            Assert.Equal("value1", viewModel.Value);
        }

        [Fact]
        public void IdentifierViewModel_Returns_ValidViewModel_NullValue()
        {
            var model = new Identifier
            {
                System = "system1",
                Value = null
            };

            var viewModel = model.ToViewModel();

            Assert.NotNull(viewModel);

            Assert.Null(viewModel.Value);
        }

        [Fact]
        public void IdentifierViewModel_Returns_InvalidViewModel_Null()
        {
            Identifier model = null;

            Assert.Throws<NullReferenceException>(() => model.ToViewModel());
        }

    }
}
