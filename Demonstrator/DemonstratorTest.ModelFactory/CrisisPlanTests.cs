using Demonstrator.Models.Core.Enums;
using Demonstrator.Models.ViewModels.Epr;
using Demonstrator.Models.ViewModels.Factories;
using DemonstratorTest.Comparer;
using Xunit;

namespace DemonstratorTest.ModelFactory
{
    public class CrisisPlanTests
    {
        [Fact]
        public void Returns_CleanedModel()
        {
            var model = new CrisisPlanViewModel {
                ActionForDependants = "test <",
                Active = true,
                Asid = "test <",
                CrisisNumber = "test <",
                EmergencyLocation = "test <",
                EmergencyNumber = "test <",
                Id = "test <",
                InvolveFamilyOrCarer = true,
                OrgCode = "test <",
                PatientAcceptsPlan = true,
                PatientNhsNumber = "test <",
                PlanCreatedBy = "test <",
                PlanCreatedByJobTitle = "test <",
                PotentialTriggers = "test <",
                RecordType = RecordType.MentalHealthCrisisPlan,
                SignsFeelingUnwell = "test <",
                Version = 1,
                WhatHelpsInCrisis = "test <"
            };

            var cleanedModel = model.Cleaned();

            var expected = new CrisisPlanViewModel
            {
                ActionForDependants = "test ",
                Active = true,
                Asid = "test ",
                CrisisNumber = "test ",
                EmergencyLocation = "test ",
                EmergencyNumber = "test ",
                Id = "test ",
                InvolveFamilyOrCarer = true,
                OrgCode = "test ",
                PatientAcceptsPlan = true,
                PatientNhsNumber = "test ",
                PlanCreatedBy = "test ",
                PlanCreatedByJobTitle = "test ",
                PotentialTriggers = "test ",
                RecordType = RecordType.MentalHealthCrisisPlan,
                SignsFeelingUnwell = "test ",
                Version = 1,
                WhatHelpsInCrisis = "test "
            };

            Assert.Equal(expected, cleanedModel, Comparers.ModelComparer<CrisisPlanViewModel>());
        }

    }
}
