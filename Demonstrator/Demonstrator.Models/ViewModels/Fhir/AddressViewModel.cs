using System.Collections.Generic;

namespace Demonstrator.Models.ViewModels.Fhir
{
    public partial class AddressViewModel
    {
        public string Use { get; set; }

        public string Type { get; set; }

        public List<string> Line { get; set; }

        public string City { get; set; }

        public string District { get; set; }

        public string PostalCode { get; set; }

        public PeriodViewModel Period { get; set; }
    }
}
