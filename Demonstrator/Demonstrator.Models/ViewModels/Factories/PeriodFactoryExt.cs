using Demonstrator.Models.ViewModels.Fhir;
using Demonstrator.Utilities.Extensions;
using Hl7.Fhir.Model;

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

            return viewModel;
        }
    }
}
