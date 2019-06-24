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
    public class HumanNameExtTests
    {
        [Fact]
        public void NameViewModel_Returns_ValidViewModel_Full()
        {
            var model = new HumanName
            {
                Family = "Jones",
                Given = new List<string> { "Mark", "Andrew" },
                Use = HumanName.NameUse.Official,
                Period = new Period(new FhirDateTime(2019, 01, 01), new FhirDateTime(2019, 12, 31))
            };

            var viewModel = model.ToViewModel();

            Assert.NotNull(viewModel);

            Assert.Equal("Jones", viewModel.Family);
            Assert.Collection(viewModel.Given, item => Assert.Equal("Mark", item),
                                  item => Assert.Equal("Andrew", item));

            Assert.Equal("Mark Andrew", viewModel.GivenString);

            Assert.Equal("Official", viewModel.Use);

            Assert.NotNull(viewModel.Period);
            Assert.Equal(new DateTime(2019, 01, 01), viewModel.Period.Start);
            Assert.Equal(new DateTime(2019, 12, 31), viewModel.Period.End);
        }

        [Fact]
        public void NameViewModel_Returns_ValidViewModel_NullUse()
        {
            var model = new HumanName
            {
                Family = "Jones",
                Given = new List<string> { "Mark", "Andrew" },
                Use = null,
                Period = new Period(new FhirDateTime(2019, 01, 01), new FhirDateTime(2019, 12, 31))
            };

            var viewModel = model.ToViewModel();
            Assert.Null(viewModel.Use);
        }


        [Fact]
        public void NameViewModel_Returns_ValidViewModel_NullGiven()
        {
            var model = new HumanName
            {
                Family = "Jones",
                Given = null,
                Use = null,
                Period = new Period(new FhirDateTime(2019, 01, 01), new FhirDateTime(2019, 12, 31))
            };

            var viewModel = model.ToViewModel();
            Assert.Empty(viewModel.Given);
            Assert.Equal(string.Empty, viewModel.GivenString);
        }
        

    }
}
