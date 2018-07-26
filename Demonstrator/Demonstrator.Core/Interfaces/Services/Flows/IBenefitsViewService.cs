using Demonstrator.Models.Core.Enums;
using Demonstrator.Models.ViewModels.Flows;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Demonstrator.Core.Interfaces.Services.Flows
{
    public interface IBenefitsViewService
    {
        Task<BenefitDialogViewModel> GetFor(BenefitForType listFor, string listForId);

        Task<BenefitDialogViewModel> GetForCategorised(BenefitForType listFor, string listForId);

        Task<BenefitMenuViewModel> GetMenu();

        IDictionary<string, IList<BenefitViewModel>> ParseBenefits(IList<BenefitViewModel> benefits);
    }
}
