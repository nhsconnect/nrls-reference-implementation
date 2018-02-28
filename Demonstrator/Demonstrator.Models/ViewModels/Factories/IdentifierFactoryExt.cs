using Demonstrator.Models.ViewModels.Fhir;
using Hl7.Fhir.Model;
using System.Collections.Generic;

namespace Demonstrator.Models.ViewModels.Factories
{
    public static class IdentifierFactoryExt
    {

        public static List<IdentifierViewModel> ToViewModelList(this List<Identifier> identifiers)
        {
            var viewModels = new List<IdentifierViewModel>();
            
            foreach(var identifier in identifiers)
            {
                viewModels.Add(identifier.ToViewModel());
            }

            return viewModels;
        }

        public static IdentifierViewModel ToViewModel(this Identifier identifier)
        {
            var viewModel = new IdentifierViewModel
            {
                System = identifier.System,
                Value = identifier.Value
            };

            return viewModel;
        }
    }
}
