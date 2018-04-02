using Demonstrator.Models.DataModels.Flows;
using Demonstrator.Models.ViewModels.Flows;
using DemonstratorTest.Comparer;
using DemonstratorTest.Data.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DemonstratorTest.ModelFactory
{
    public class BenefitTests
    {
        [Fact]
        public void Benefit_Returns_ValidViewModel()
        {
            var models = MongoBenefit.Benefits;

            var viewModel = models.Select(Benefit.ToViewModel).First();

            var expectedViewModel = new BenefitViewModel
            {
                Id = "5a8417f68317338c8e080a62",
                Text = "Benefit",
                Categories = new List<string> { "Test" },
                Order = 1,
                Type = "Test"
            };

            Assert.Equal(expectedViewModel, viewModel, Comparers.ModelComparer<BenefitViewModel>());
        }

        [Fact]
        public void Benefit_Null_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => Benefit.ToViewModel(null));
        }
    }
}
