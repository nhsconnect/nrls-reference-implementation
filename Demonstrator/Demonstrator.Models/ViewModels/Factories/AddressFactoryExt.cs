using Demonstrator.Models.ViewModels.Fhir;
using Hl7.Fhir.Model;
using System.Collections.Generic;
using System.Linq;

namespace Demonstrator.Models.ViewModels.Factories
{
    public static class AddressFactoryExt
    {

        public static List<AddressViewModel> ToViewModelList(this List<Address> addresses)
        {
            var viewModels = new List<AddressViewModel>();
            
            foreach(var address in addresses)
            {
                viewModels.Add(address.ToViewModel());
            }

            return viewModels;
        }

        public static AddressViewModel ToViewModel(this Address address)
        {
            var viewModel = new AddressViewModel
            {
                City = address.City,
                District = address.District,
                Line = address.Line.ToList(),
                PostalCode = address.PostalCode,
                Type = address.Type?.ToString(),
                Use = address.Use?.ToString(),
                Period = address.Period?.ToPeriodViewModel()
            };

            return viewModel;
        }
    }
}
