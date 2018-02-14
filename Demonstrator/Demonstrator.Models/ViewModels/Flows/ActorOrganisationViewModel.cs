using Demonstrator.Models.Core.Enums;

namespace Demonstrator.Models.ViewModels.Flows
{
    public class ActorOrganisationViewModel
    {
        public string Id { get; set; }

        public string TypeName => Type.ToString();

        public ActorType Type { get; set; }

        public string Name { get; set; }

        public string ImageUrl { get; set; }

        public string Context { get; set; }

        public string OrgCode { get; set; }
    }
}
