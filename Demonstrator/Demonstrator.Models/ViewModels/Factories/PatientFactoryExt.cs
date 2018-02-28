using Demonstrator.Models.ViewModels.Fhir;
using Demonstrator.Utilities.Extensions;
using Hl7.Fhir.Model;

namespace Demonstrator.Models.ViewModels.Factories
{
    public static class PatientFactoryExt
    {
        public static PatientViewModel ToViewModel(this Patient patient)
        {
            var dob = patient.BirthDateElement;
            var viewModel = new PatientViewModel
            {
                Id = patient.Id,
                Name = patient.Name.ToViewModelList(),
                Active = patient.Active,
                Gender = patient.Gender?.ToString(),
                BirthDate = patient.BirthDate?.ToDateTime(),
                Address = patient.Address.ToViewModelList(),
                Identifier = patient.Identifier.ToViewModelList()
            };

            return viewModel;
        }
    }
}
