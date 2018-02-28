using System.Collections.Generic;

namespace Demonstrator.Models.ViewModels.Fhir
{
    public partial class OrganizationViewModel
    {
        public string ResourceType { get; set; }

        public string Id { get; set; }

        public MetaViewModel Meta { get; set; }

        public List<IdentifierViewModel> Identifier { get; set; }

        public string Name { get; set; }

        public List<ContactPointViewModel> Telecom { get; set; }

        public List<AddressViewModel> Address { get; set; }
    }
}
