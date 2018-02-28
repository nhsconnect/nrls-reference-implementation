using System;
using System.Collections.Generic;

namespace Demonstrator.Models.ViewModels.Fhir
{
    public partial class PatientViewModel
    {
        public string ResourceType { get; set; }

        public string Id { get; set; }

        public List<IdentifierViewModel> Identifier { get; set; }

        public bool? Active { get; set; }

        public List<NameViewModel> Name { get; set; }

        public List<TelecomViewModel> Telecom { get; set; }

        public string Gender { get; set; }

        public DateTime? BirthDate { get; set; }

        public bool? DeceasedBoolean { get; set; }

        public List<AddressViewModel> Address { get; set; }

        public ReferenceViewModel ManagingOrganization { get; set; }
    }
}
