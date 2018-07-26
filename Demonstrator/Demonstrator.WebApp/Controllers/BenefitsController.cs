using Demonstrator.Core.Interfaces.Services.Flows;
using Demonstrator.Models.Core.Enums;
using Demonstrator.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

//In reality all end points would be secured
namespace Demonstrator.WebApp.Controllers
{
    [Route("api/[controller]")]
    public class BenefitsController : Controller
    {
        private readonly IBenefitsViewService _benefitsViewService;

        public BenefitsController(IBenefitsViewService benefitsViewService)
        {
            _benefitsViewService = benefitsViewService;
        }

        [HttpGet("Menu")]
        public async Task<IActionResult> GetMenu()
        {
            var benefits = await _benefitsViewService.GetMenu();

            if (benefits == null)
            {
                return NotFound($"Benefits menu not found.");
            }

            return Ok(benefits);
        }

        // GET api/Benefits/ActorOrganisation/5a82c6cecb969daa58d32dkfl9
        [HttpGet("Has/{listFor}/{listForId:regex(^[[A-Fa-f0-9]]{{1,1024}}$)}")]
        public async Task<IActionResult> Has(string listFor, string listForId)
        {
            var benefitForType = EnumHelpers.GetEnum<BenefitForType>(listFor);

            var benefits = await _benefitsViewService.GetFor(benefitForType, listForId);

            var hasBenefits = (benefits != null);

            return Ok(hasBenefits);
        }

        // GET api/Benefits/ActorOrganisation/5a82c6cecb969daa58d32dkfl9
        [HttpGet("{listFor}/{listForId:regex(^[[A-Fa-f0-9]]{{1,1024}}$)}")]
        public async Task<IActionResult> Get(string listFor, string listForId)
        {
            var benefitForType = EnumHelpers.GetEnum<BenefitForType>(listFor);

            var benefits = await _benefitsViewService.GetForCategorised(benefitForType, listForId);

            if (benefits == null)
            {
                return NotFound($"Benefits not found for type {listForId} with id {listFor}.");
            }

            return Ok(benefits);
        }

    }
}
