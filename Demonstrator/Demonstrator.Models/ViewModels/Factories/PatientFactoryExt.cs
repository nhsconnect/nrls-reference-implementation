using Demonstrator.Models.ViewModels.Fhir;
using Demonstrator.Utilities.Extensions;
using Hl7.Fhir.Model;
using System.Linq;

namespace Demonstrator.Models.ViewModels.Factories
{
    public static class PatientFactoryExt
    {
        //TODO : Set up test
        public static PatientViewModel ToViewModel(this Patient patient, string nhsNumberIdentifier)
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
                Identifier = patient.Identifier.ToViewModelList(),
                ManagingOrganization = patient.ManagingOrganization?.ToViewModel(),
                Telecom = patient.Telecom?.OrderBy(x => x.Rank).FirstOrDefault()?.ToViewModel()
            };

            viewModel.NhsNumber = viewModel.Identifier.FirstOrDefault(x => !string.IsNullOrEmpty(nhsNumberIdentifier) && !string.IsNullOrEmpty(x.System) && x.System.Equals(nhsNumberIdentifier))?.Value;

            viewModel.CurrentName = viewModel.Name.FirstOrDefault(x => x.Period == null || x.Period.IsActive);

            viewModel.CurrentAddress = viewModel.Address.FirstOrDefault(x => x.Period == null || x.Period.IsActive);

            return viewModel;
        }
    }
}
