using System.Collections.Generic;

namespace Demonstrator.Models.ViewModels.Fhir
{
    public partial class NameViewModel
    {
        public string Use { get; set; }

        public string Family { get; set; }

        public List<string> Given { get; set; }

        public string GivenString { get; set; }

        public PeriodViewModel Period { get; set; }
    }
}
