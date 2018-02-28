using Demonstrator.Models.ViewModels.Fhir;
using Hl7.Fhir.Model;
using System.Collections.Generic;

namespace Demonstrator.Models.ViewModels.Factories
{
    public static class ResourceReferenceFactoryExt
    {

        public static List<ReferenceViewModel> ToViewModelList(this List<ResourceReference> resourceReferences)
        {
            var viewModels = new List<ReferenceViewModel>();
            
            foreach(var resourceReference in resourceReferences)
            {
                viewModels.Add(resourceReference.ToViewModel());
            }

            return viewModels;
        }

        public static ReferenceViewModel ToViewModel(this ResourceReference resourceReference)
        {
            var refIdPos = resourceReference.Reference.LastIndexOf("/");

            var viewModel = new ReferenceViewModel
            {
                Id = resourceReference.Reference?.Substring(refIdPos + 1),
                Reference = resourceReference.Reference
            };

            return viewModel;
        }
    }
}
