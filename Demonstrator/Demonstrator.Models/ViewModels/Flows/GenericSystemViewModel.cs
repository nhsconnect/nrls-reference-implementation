using Demonstrator.Models.Core.Enums;
using Demonstrator.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace Demonstrator.Models.ViewModels.Flows
{
    public class GenericSystemViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string SafeName => StringHelper.UrlString(Name);

        public string Context { get; set; }

        public string FModule { get; set; }

        public string Asid { get; set; }

        public List<string> ActionTypeNames => ActionTypes.Select(x => x.ToString()).ToList();

        public List<ActorType> ActionTypes { get; set; }
    }
}
