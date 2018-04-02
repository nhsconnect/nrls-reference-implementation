using System.Collections.Generic;

namespace Demonstrator.Models.ViewModels.Flows
{
    public class BenefitMenuItem
    {
        public string Title { get; set; }

        public string Id { get; set; }

        public string Type { get; set; }

        public bool Active { get; set; }

        public IList<BenefitMenuItem> Children { get; set; }
    }
}
