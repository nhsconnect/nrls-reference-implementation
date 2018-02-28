using Demonstrator.Models.ViewModels.Fhir;
using Hl7.Fhir.Model;
using System.Collections.Generic;

namespace Demonstrator.Models.ViewModels.Factories
{
    public static class ContactPointFactoryExt
    {

        public static List<ContactPointViewModel> ToViewModelList(this List<ContactPoint> contactPoints)
        {
            var viewModels = new List<ContactPointViewModel>();
            
            foreach(var contactPoint in contactPoints)
            {
                viewModels.Add(contactPoint.ToViewModel());
            }

            return viewModels;
        }

        public static ContactPointViewModel ToViewModel(this ContactPoint contactPoint)
        {
            var viewModel = new ContactPointViewModel
            {
                System = contactPoint.System?.ToString(),
                Value = contactPoint.Value,
                Use = contactPoint.Use?.ToString()
            };

            return viewModel;
        }
    }
}
