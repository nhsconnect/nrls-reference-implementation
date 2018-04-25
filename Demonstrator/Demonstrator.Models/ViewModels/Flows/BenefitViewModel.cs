using System;
using System.Collections.Generic;
using System.Text;

namespace Demonstrator.Models.ViewModels.Flows
{
    public class BenefitViewModel
    {
        public string Id { get; set; }

        public string Text { get; set; }

        public IList<string> Categories { get; set; }

        public int Order { get; set; }

        public string Type { get; set; }

        public bool Active { get; set; }
    }
}
