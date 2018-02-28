using Demonstrator.Models.ViewModels.Fhir;
using Hl7.Fhir.Model;
using System.Collections.Generic;

namespace Demonstrator.Models.ViewModels.Factories
{
    public static class CodeableConceptFactoryExt
    {

        public static List<CodeableConceptViewModel> ToViewModelList(this List<CodeableConcept> codeableConcepts)
        {
            var viewModels = new List<CodeableConceptViewModel>();
            
            foreach(var codeableConcept in codeableConcepts)
            {
                viewModels.Add(codeableConcept.ToViewModel());
            }

            return viewModels;
        }

        public static CodeableConceptViewModel ToViewModel(this CodeableConcept codeableConcept)
        {
            var viewModel = new CodeableConceptViewModel
            {
                Coding = codeableConcept.Coding.ToCodingViewModelList()
            };

            return viewModel;
        }

        public static List<CodingViewModel> ToCodingViewModelList(this List<Coding> codings)
        {
            var viewModels = new List<CodingViewModel>();

            foreach (var coding in codings)
            {
                viewModels.Add(coding.ToCodingViewModel());
            }

            return viewModels;
        }

        private static CodingViewModel ToCodingViewModel(this Coding coding)
        {
            var viewModel = new CodingViewModel
            {
                Code = coding.Code,
                Display = coding.Display,
                System = coding.System
            };

            return viewModel;
        }
    }
}
