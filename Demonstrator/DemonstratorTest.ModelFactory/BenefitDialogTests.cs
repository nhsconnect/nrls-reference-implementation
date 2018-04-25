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
    public class BenefitDialogTests
    {
        [Fact]
        public void BenefitDialog_Returns_ValidHasEfficiency()
        {
            var models = MongoBenefit.EfficiencyBenefits;

            var viewModel = models.Select(Benefit.ToViewModel).First();

            var dialogViewModel = new BenefitDialogViewModel
            {
                Benefits = new Dictionary<string, IList<BenefitViewModel>>
                {
                    { "Efficiency", new List<BenefitViewModel> { viewModel } }
                }
            };

            Assert.True(dialogViewModel.HasEfficiency);
        }

        [Fact]
        public void BenefitDialog_Returns_ValidNotHasHealth()
        {
            var models = MongoBenefit.HealthBenefits;

            var viewModel = models.Select(Benefit.ToViewModel).First();

            var dialogViewModel = new BenefitDialogViewModel
            {
                Benefits = new Dictionary<string, IList<BenefitViewModel>>
                {
                    { "Health", new List<BenefitViewModel> { viewModel } }
                }
            };

            Assert.False(dialogViewModel.HasHealth);
        }

        [Fact]
        public void BenefitDialog_Returns_ValidCategoryCount()
        {
            var eModels = MongoBenefit.EfficiencyBenefits;
            var hModels = MongoBenefit.HealthBenefits;
            var sModels = MongoBenefit.SafteyBenefits;

            var eViewModels = eModels.Select(Benefit.ToViewModel).ToList();
            var hViewModels = hModels.Select(Benefit.ToViewModel).ToList();
            var sViewModels = sModels.Select(Benefit.ToViewModel).ToList();

            var dialogViewModel = new BenefitDialogViewModel
            {
                Benefits = new Dictionary<string, IList<BenefitViewModel>>
                {
                    { "Efficiency", eViewModels },
                    { "Health", hViewModels },
                    { "Safety", sViewModels }
                }
            };

            Assert.Equal(2, dialogViewModel.TotalCategories);
        }


    }
}
