using Demonstrator.Models.ViewModels.Fhir;
using Demonstrator.Utilities.Extensions;
using Hl7.Fhir.Model;
using System;

namespace Demonstrator.Models.ViewModels.Factories
{
    public static class PeriodFactoryExt
    {
        public static PeriodViewModel ToPeriodViewModel(this Period period)
        {
            var viewModel = new PeriodViewModel
            {
                Start = period.Start?.ToDateTime(),
                End = period.End?.ToDateTime()
            };

            var now = DateTime.UtcNow;
            viewModel.IsActive = (!viewModel.Start.HasValue || viewModel.Start.Value <= now) && (!viewModel.End.HasValue || viewModel.End.Value > now);

            return viewModel;
        }
    }
}
