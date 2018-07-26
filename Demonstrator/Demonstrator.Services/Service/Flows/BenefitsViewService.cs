using Demonstrator.Models.DataModels.Flows;
using Demonstrator.Services.Extensions;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Demonstrator.Models.ViewModels.Flows;
using MongoDB.Bson;
using Demonstrator.Core.Interfaces.Database;
using Demonstrator.Core.Interfaces.Services.Flows;
using System.Linq;
using Demonstrator.Core.Exceptions;
using Demonstrator.Models.Core.Enums;

namespace Demonstrator.Services.Service.Flows
{
    public class BenefitsViewService : IBenefitsViewService
    {
        private readonly IPersonnelService _personnelService;
        private readonly IActorOrganisationService _actorOrganisationService;
        private readonly IBenefitsService _benefitsService;

        public BenefitsViewService(IPersonnelService personnelService, IActorOrganisationService actorOrganisationService, IBenefitsService benefitsService)
        {
            _personnelService = personnelService;
            _actorOrganisationService = actorOrganisationService;
            _benefitsService = benefitsService;
        }

        public async Task<BenefitDialogViewModel> GetFor(BenefitForType listFor, string listForId)
        {
  
            var benefitsDialog = await GetBenefitIds(listFor, listForId);
            if (benefitsDialog.BenefitIds == null || benefitsDialog.BenefitIds.Count() == 0)
            {
                return null;
            }

            var benefits = await _benefitsService.GetByIdList(benefitsDialog.BenefitIds);
            benefitsDialog.Benefits = new Dictionary<string, IList<BenefitViewModel>> { { "All", benefits } };

            return benefitsDialog;

        }

        public async Task<BenefitDialogViewModel> GetForCategorised(BenefitForType listFor, string listForId)
        {

            var benefitsDialog = await GetFor(listFor, listForId);

            if(benefitsDialog == null)
            {
                return null;
            }

            benefitsDialog.Benefits = ParseBenefits(benefitsDialog.Benefits["All"]);

            return benefitsDialog;
        }

        public async Task<BenefitMenuViewModel> GetMenu()
        {
            try
            {
                var benefitMenuViewModel = new BenefitMenuViewModel();

                var personnel = await _personnelService.GetAll();
                var personnelOrgs = personnel.Select(x => x.ActorOrganisationId).Distinct().ToList();

                var actorOrganisations = await _actorOrganisationService.GetAll();
                var actorOrganisationIds = actorOrganisations.Select(x => x.Id).ToList();

                var orphanedOrgs = personnelOrgs.Where(x => !actorOrganisationIds.Contains(x));
                var orphanedPersonnel = personnel.Where(x => !string.IsNullOrEmpty(x.ActorOrganisationId) && orphanedOrgs.Contains(x.ActorOrganisationId) && x.Benefits != null && x.Benefits.Count > 0);

                foreach (var orphan in orphanedPersonnel)
                {
                    benefitMenuViewModel.MenuItems.Add(new BenefitMenuItem {
                        Id = orphan.Id,
                        Title = orphan.Name,
                        Type = "Personnel",
                        Active = true
                    });
                }

                foreach (var orgs in actorOrganisations)
                {
                    var children = personnel.Where(x => x.ActorOrganisationId == orgs.Id && x.Benefits != null && x.Benefits.Count > 0);
                    var childItems = new List<BenefitMenuItem>();

                    foreach (var child in children)
                    {
                        childItems.Add(new BenefitMenuItem {
                            Id = child.Id,
                            Title = child.Name,
                            Type = "Personnel",
                            Active = true
                        });
                    }

                    var canBeActive = orgs.Benefits != null && orgs.Benefits.Count > 0;

                    if(!childItems.Any() && !canBeActive)
                    {
                        continue;
                    }

                    benefitMenuViewModel.MenuItems.Add(new BenefitMenuItem
                    {
                        Id = orgs.Id,
                        Title = orgs.Name,
                        Type = "ActorOrganisation",
                        Active = canBeActive,
                        Children = childItems
                    });
                }

                return benefitMenuViewModel;
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        public IDictionary<string, IList<BenefitViewModel>> ParseBenefits(IList<BenefitViewModel> benefits)
        {
            var categorisedBenefits = new Dictionary<string, IList<BenefitViewModel>>();

            var types = benefits.SelectMany(x => x.Categories).Distinct().ToList();

            foreach(var type in types)
            {
                var benefitTypes = benefits.Where(x => x.Categories.Contains(type)).ToList();
                categorisedBenefits.Add(type, benefitTypes);
            }

            return categorisedBenefits;
        }

        private async Task<BenefitDialogViewModel> GetBenefitIds(BenefitForType listFor, string listForId)
        {
            var benefits = new BenefitDialogViewModel();

            switch (listFor)
            {
                case BenefitForType.Personnel:
                    var personnel = await _personnelService.GetModelById(listForId);
                    benefits.BenefitsTitle = personnel.BenefitsTitle;
                    benefits.BenefitIds = personnel.Benefits;
                    break;
                case BenefitForType.ActorOrganisation:
                    var orgs = await _actorOrganisationService.GetModelById(listForId);
                    benefits.BenefitsTitle = orgs.BenefitsTitle;
                    benefits.BenefitIds = orgs.Benefits;
                    break;
                default:
                    throw new BenefitForException(listFor.ToString());
            }

            return benefits;
        }
    }
}
