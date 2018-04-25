using Demonstrator.Core.Interfaces.Services.Flows;
using Demonstrator.Models.DataModels.Flows;
using Demonstrator.Models.ViewModels.Flows;
using Demonstrator.Services.Service.Flows;
using DemonstratorTest.Data.Helpers;
using Moq;
using System.Linq;
using Xunit;

namespace DemonstratorTest.Services
{
    public class BenefitsViewServiceTests
    {
        [Fact]
        public void BenefitViewService_Returns_ValidParseBenefits()
        {
            var models = MongoBenefit.Benefits;

            //There is a separate test for this to ensure ToViewModel works
            var viewModels = models.Select(Benefit.ToViewModel).ToList();

            var personnelServiceMock = new Mock<IPersonnelService>();
            var actorOrganisationServiceMock = new Mock<IActorOrganisationService>();
            var benefitsServiceMock = new Mock<IBenefitsService>();

            var benefitsViewService = new BenefitsViewService(personnelServiceMock.Object, actorOrganisationServiceMock.Object, benefitsServiceMock.Object);

            var dialogViewModel = new BenefitDialogViewModel();

            dialogViewModel.Benefits = benefitsViewService.ParseBenefits(viewModels);

            Assert.Equal(2, dialogViewModel.TotalCategories);
        }
    }
}
