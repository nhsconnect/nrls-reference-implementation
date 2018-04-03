using Demonstrator.Models.Core.Enums;
using Demonstrator.Models.ViewModels.Epr;
using Demonstrator.Utilities;
using System;

namespace Demonstrator.Models.DataModels.Epr
{
    public class CrisisPlan : MedicalRecord
    {
        public bool InvolveFamilyOrCarer { get; set; }

        public string SignsFeelingUnwell { get; set; }

        public string PotentialTriggers { get; set; }

        public string WhatHelpsInCrisis { get; set; }

        public string ActionForDependants { get; set; }

        public string EmergencyLocation { get; set; }

        public string EmergencyNumber { get; set; }

        public string CrisisNumber { get; set; }

        public bool PatientAcceptsPlan { get; set; }

        public string PlanCreatedBy { get; set; }

        public string PlanCreatedByJobTitle { get; set; }

        public DateTime? PlanCreated { get; set; }

        public DateTime? PlanUpdated { get; set; }

        public static CrisisPlanViewModel ToViewModel(CrisisPlan model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), "Cannot map null CrisisPlanViewModel");
            }

            var viewModel = new CrisisPlanViewModel
            {
                Id = model.Id.ToString(),
                Version = model.Version,
                RecordType = EnumHelpers.GetEnum<RecordType>(model.RecordType),
                ActionForDependants = model.ActionForDependants,
                Active = model.Active,
                CrisisNumber = "0800 0001 001",
                EmergencyLocation = "Leeds General Infirmary",
                EmergencyNumber = "0113 225 8088",
                InvolveFamilyOrCarer = model.InvolveFamilyOrCarer,
                PatientAcceptsPlan = model.PatientAcceptsPlan,
                PatientNhsNumber = model.PatientNhsNumber,
                PlanCreated = model.PlanCreated,
                PlanCreatedBy = model.PlanCreatedBy,
                PlanCreatedByJobTitle = model.PlanCreatedByJobTitle,
                PlanUpdated = model.PlanUpdated,
                PotentialTriggers = model.PotentialTriggers,
                SignsFeelingUnwell = model.SignsFeelingUnwell,
                WhatHelpsInCrisis = model.WhatHelpsInCrisis
            };

            return viewModel;
        }

        public static CrisisPlan ToModel(CrisisPlanViewModel viewModel)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException(nameof(viewModel), "Cannot map null CrisisPlan");
            }

            var model = new CrisisPlan
            {
                Version = viewModel.Version,
                RecordType = viewModel.RecordType.ToString(),
                ActionForDependants = viewModel.ActionForDependants,
                Active = viewModel.Active,
                InvolveFamilyOrCarer = viewModel.InvolveFamilyOrCarer,
                PatientAcceptsPlan = viewModel.PatientAcceptsPlan,
                PatientNhsNumber = viewModel.PatientNhsNumber,
                PlanCreated = viewModel.PlanCreated,
                PlanCreatedBy = viewModel.PlanCreatedBy,
                PlanCreatedByJobTitle = viewModel.PlanCreatedByJobTitle,
                PlanUpdated = viewModel.PlanUpdated,
                PotentialTriggers = viewModel.PotentialTriggers,
                SignsFeelingUnwell = viewModel.SignsFeelingUnwell,
                WhatHelpsInCrisis = viewModel.WhatHelpsInCrisis
            };

            return model;
        }

    }
}
