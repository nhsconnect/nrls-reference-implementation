using Demonstrator.Models.ViewModels.Fhir;
using Hl7.Fhir.Model;
using System.Collections.Generic;
using System.Linq;

namespace Demonstrator.Models.ViewModels.Factories
{
    public static class HumanNameFactoryExt
    {

        public static List<NameViewModel> ToViewModelList(this List<HumanName> names)
        {
            var viewModels = new List<NameViewModel>();
            
            foreach(var name in names)
            {
                viewModels.Add(name.ToViewModel());
            }

            return viewModels;
        }

        public static NameViewModel ToViewModel(this HumanName name)
        {
            var viewModel = new NameViewModel
            {
                Family = name.Family,
                Given = name.Given.ToList(),
                Use = name.Use?.ToString(),
                Period = name.Period?.ToPeriodViewModel()
            };

            viewModel.GivenString = string.Join(" ", viewModel.Given);

            return viewModel;
        }
    }
}
