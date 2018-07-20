using Demonstrator.Models.Core.Enums;
using Demonstrator.Models.Core.Models;
using Demonstrator.Utilities;
using System.Collections.Generic;

namespace Demonstrator.Models.ViewModels.Flows
{
    public class ActorOrganisationViewModel
    {
        public string Id { get; set; }

        public string TypeName => Type.ToString();

        public ActorType Type { get; set; }

        public string Name { get; set; }

        public string SafeName => StringHelper.UrlString(Name);

        public string ImageUrl { get; set; }

        public IList<ContentView> Context { get; set; }

        public string OrgCode { get; set; }

        public IList<string> Benefits { get; set; }

        public string PersonnelLinkId { get; set; }

    }
}
