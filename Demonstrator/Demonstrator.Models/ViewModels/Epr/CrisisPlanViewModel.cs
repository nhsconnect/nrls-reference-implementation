using System;

namespace Demonstrator.Models.ViewModels.Epr
{
    public class CrisisPlanViewModel : MedicalRecordViewModel
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

    }
}
