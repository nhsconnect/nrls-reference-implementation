using System.Collections.Generic;

namespace NRLS_API.Models.ViewModels.Fhir
{
    public partial class CodeableConceptViewModel
    {
        public List<CodingViewModel> Coding { get; set; }
    }
}
