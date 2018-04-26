using Demonstrator.Models.ViewModels.Fhir;
using Hl7.Fhir.Model;
using System.Linq;

namespace Demonstrator.Models.ViewModels.Factories
{
    public static class OrganizationFactoryExt
    {
        //TODO : Set up test
        public static OrganizationViewModel ToViewModel(this Organization organization, string orgCodeSystem)
        {
            var viewModel = new OrganizationViewModel
            {
                Id = organization.Id,
                Address = organization.Address.ToViewModelList(),
                Name = organization.Name,
                Telecom = organization.Telecom.ToViewModelList(),
                Identifier = organization.Identifier.ToViewModelList()
            };

            viewModel.OrgCode = viewModel.Identifier.FirstOrDefault(x => !string.IsNullOrEmpty(orgCodeSystem) && !string.IsNullOrEmpty(x.System) && x.System.Equals(orgCodeSystem))?.Value;

            return viewModel;
        }
    }
}
