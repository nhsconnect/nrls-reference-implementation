using Demonstrator.Models.Core.Enums;
using System.Collections.Generic;
using System.Linq;

namespace Demonstrator.Models.ViewModels.Flows
{
    public class BenefitDialogViewModel
    {
        public string BenefitsTitle { get; set; }

        public IList<string> BenefitIds { get; set; }

        public IList<string> Categories => ActiveCategories();

        public IDictionary<string, IList<BenefitViewModel>> Benefits { get; set; }

        public bool HasEfficiency => CheckBenefitCategory(BenefitCategories.Efficiency.ToString());

        public bool HasFinancial => CheckBenefitCategory(BenefitCategories.Financial.ToString());

        public bool HasHealth => CheckBenefitCategory(BenefitCategories.Health.ToString());

        public bool HasSafety => CheckBenefitCategory(BenefitCategories.Safety.ToString());

        public int TotalCategories => Categories.Count;

        private bool CheckBenefitCategory(string category)
        {
            return Benefits != null && Benefits.ContainsKey(category) && Benefits[category].Any(x => x.Active);
        }

        private IList<string> ActiveCategories()
        {
            var categories = new List<string>();
            if (Benefits == null || Benefits.Keys == null || Benefits.Keys.Count == 0) {
                return categories;
            }

            foreach(var key in Benefits.Keys)
            {
                if(Benefits[key] != null && Benefits[key].Count > 0 && Benefits[key].Any(x => x.Active))
                {
                    categories.Add(key);
                }
            }

            return categories;
        }

    }
}
