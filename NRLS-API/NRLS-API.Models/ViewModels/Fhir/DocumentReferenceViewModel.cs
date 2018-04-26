using System;
using System.Collections.Generic;
using NRLS_API.Models.ViewModels.Fhir;

namespace NRLS_API.Models.ViewModels.Nrls
{
    public partial class DocumentReferenceViewModel
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

        public List<ReferenceViewModel> Author { get; set; }

        public ReferenceViewModel Custodian { get; set; }

        public List<ContentViewModel> Content { get; set; }

    }
}
