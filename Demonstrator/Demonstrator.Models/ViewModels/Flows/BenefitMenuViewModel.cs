using System.Collections.Generic;

namespace Demonstrator.Models.ViewModels.Flows
{
    public class BenefitMenuViewModel
    {
        public BenefitMenuViewModel()
        {
            MenuItems = new List<BenefitMenuItem>();
        }

        public IList<BenefitMenuItem> MenuItems { get; set; }
    }
}
