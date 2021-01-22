using System;
using System.Collections.Generic;
using Demonstrator.Models.ViewModels.Fhir;

namespace Demonstrator.Models.ViewModels.Nrls
{
    public partial class PointerViewModel
    {
        public string ResourceType { get; set; }

        public string Id { get; set; }

        public MetaViewModel Meta { get; set; }

        public List<IdentifierViewModel> Identifier { get; set; }

        public string Status { get; set; }

        public CodeableConceptViewModel Type { get; set; }

        public ReferenceViewModel Subject { get; set; }

        public DateTimeOffset? Created { get; set; }

        public DateTimeOffset? Indexed { get; set; }

        public ReferenceViewModel Author { get; set; }

        public ReferenceViewModel Custodian { get; set; }

        public List<ContentViewModel> Content { get; set; }

        //Navigation Properties
        public PatientViewModel SubjectViewModel { get; set; }

        public OrganizationViewModel CustodianViewModel { get; set; }

        public OrganizationViewModel AuthorViewModel { get; set; }

        public string PracticeSetting { get; set; }

        public string ContactUrl { get; set; }
    }
}
