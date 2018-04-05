using Demonstrator.Models.Core.Models;
using System.Collections.Generic;

namespace Demonstrator.Models.ViewModels.Flows
{
    public class PersonnelViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string PersonnelType { get; set; }

        public string ImageUrl { get; set; }

        public IList<ContentView> Context { get; set; }

        public bool UsesNrls { get; set; }

        public string CModule { get; set; }

        public List<string> SystemIds { get; set; }

        public string ActorOrganisationId { get; set; }

        public IList<string> Benefits { get; set; }

    }
}
