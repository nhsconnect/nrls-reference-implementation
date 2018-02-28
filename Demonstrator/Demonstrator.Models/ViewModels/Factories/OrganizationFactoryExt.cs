using Demonstrator.Models.ViewModels.Fhir;
using Hl7.Fhir.Model;

namespace Demonstrator.Models.ViewModels.Factories
{
    public static class OrganizationFactoryExt
    {
        public static OrganizationViewModel ToViewModel(this Organization organization)
        {
            var viewModel = new OrganizationViewModel
            {
                Id = organization.Id,
                Address = organization.Address.ToViewModelList(),
                Name = organization.Name,
                Telecom = organization.Telecom.ToViewModelList()
            };

            return viewModel;
        }
    }
}
