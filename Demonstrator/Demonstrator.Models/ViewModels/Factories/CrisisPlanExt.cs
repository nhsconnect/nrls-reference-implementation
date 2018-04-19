using Demonstrator.Models.ViewModels.Epr;
using Demonstrator.Utilities;

namespace Demonstrator.Models.ViewModels.Factories
{
    public static class CrisisPlanExt
    {
        public static CrisisPlanViewModel Cleaned(this CrisisPlanViewModel plan)
        {
            plan.SignsFeelingUnwell = StringHelper.CleanInput(plan.SignsFeelingUnwell);
            plan.PotentialTriggers = StringHelper.CleanInput(plan.PotentialTriggers);
            plan.WhatHelpsInCrisis = StringHelper.CleanInput(plan.WhatHelpsInCrisis);
            plan.ActionForDependants = StringHelper.CleanInput(plan.ActionForDependants);

            plan.EmergencyLocation = StringHelper.CleanInput(plan.EmergencyLocation);
            plan.EmergencyNumber = StringHelper.CleanInput(plan.EmergencyNumber);
            plan.CrisisNumber = StringHelper.CleanInput(plan.CrisisNumber);

            plan.PlanCreatedBy = StringHelper.CleanInput(plan.PlanCreatedBy);
            plan.PlanCreatedByJobTitle = StringHelper.CleanInput(plan.PlanCreatedByJobTitle);
            plan.CrisisNumber = StringHelper.CleanInput(plan.CrisisNumber);

            plan.PatientNhsNumber = StringHelper.CleanInput(plan.PatientNhsNumber);

            plan.Id = StringHelper.CleanInput(plan.Id);
            plan.OrgCode = StringHelper.CleanInput(plan.OrgCode);
            plan.Asid = StringHelper.CleanInput(plan.Asid);

            return plan;
        }
    }
}
