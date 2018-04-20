using Demonstrator.Core.Exceptions;
using Demonstrator.Models.ViewModels.Base;
using Microsoft.AspNetCore.Mvc;

//In reality all end points would be secured
namespace Demonstrator.WebApp.Controllers
{
    public class FhirBaseController : Controller
    {
        public FhirBaseController() {}

        protected void SetHeaders<T>(T resource) where T : RequestViewModel
        {
            if(resource == null)
            {
                throw new HttpFhirException($"Supplied model is invalid.");
            }

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
