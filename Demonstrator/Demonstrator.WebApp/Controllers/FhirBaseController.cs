using Demonstrator.Models.ViewModels.Base;
using Microsoft.AspNetCore.Mvc;

namespace Demonstrator.WebApp.Controllers
{
    public class FhirBaseController : Controller
    {
        public FhirBaseController() {}

        protected void SetHeaders<T>(T resource) where T : RequestViewModel
        {
            if (Request.Headers.ContainsKey("Asid"))
            {
                resource.Asid = Request.Headers["Asid"];
            }

            if (Request.Headers.ContainsKey("OrgCode"))
            {
                resource.OrgCode = Request.Headers["OrgCode"];
            }

        }

    }
}
