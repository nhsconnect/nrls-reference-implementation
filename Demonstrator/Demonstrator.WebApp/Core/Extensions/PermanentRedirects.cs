using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Net.Http.Headers;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Demonstrator.WebApp.Core.Extensions
{
    public class PermanentRedirects
    {
        public static void RedirectRequests(RewriteContext context)
        {
            var request = context.HttpContext.Request;

            var redirect = _redirectMap.FirstOrDefault(x => Regex.IsMatch(request.Path.Value, x.Key));

            if (redirect.Key != null)
            {
                var response = context.HttpContext.Response;
                response.StatusCode = StatusCodes.Status301MovedPermanently;
                context.Result = RuleResult.EndResponse;
                response.Headers[HeaderNames.Location] = redirect.Value;
            }
        }

        private static IDictionary<string, string> _redirectMap
        {
            get
            {
                return new Dictionary<string, string>
                {
                    { "^/about$", "https://digital.nhs.uk/services/national-record-locator" },
                    { "^/about/consumers-providers$", "https://digital.nhs.uk/services/national-record-locator" },
                    { "^/about/onboarding$", "https://digital.nhs.uk/services/national-record-locator" },
                    { "^/about/timeline$", "https://digital.nhs.uk/services/national-record-locator" },
                    { "^/about/benefits$", "https://digital.nhs.uk/services/national-record-locator/benefits-of-the-national-record-locator" },
                    { "^/developers$", "https://digital.nhs.uk/services/national-record-locator/national-record-locator-for-developers" },
                    { "^/actor-organisation/5a8daa5c952e372cbdb317eb(/[^/]+)?$", "https://digital.nhs.uk/services/national-record-locator/national-record-locator-for-ambulance-services" },
                    { "^/actor-organisation/5a8daa5c952e372cbdb317ea(/[^/]+)?$", "https://digital.nhs.uk/services/national-record-locator/national-record-locator-for-mental-health-trusts" },
                    { "^/personnel/5a8dabea952e372cbdb318dc(/[^/]+)?$", "https://digital.nhs.uk/services/national-record-locator/national-record-locator-for-ambulance-services" },
                    { "^/personnel/5a8dabea952e372cbdb318dd(/[^/]+)?$", "https://digital.nhs.uk/services/national-record-locator/national-record-locator-for-ambulance-services" },
                    { "^/personnel/5a8dabea952e372cbdb318df(/[^/]+)?$", "https://digital.nhs.uk/services/national-record-locator/national-record-locator-for-ambulance-services" },
                    { "^/personnel/5a8dabea952e372cbdb318e0(/[^/]+)?$", "https://digital.nhs.uk/services/national-record-locator/national-record-locator-for-ambulance-services" },
                    { "^/personnel/5a9879fd96592c5454267599(/[^/]+)?$", "https://digital.nhs.uk/services/national-record-locator/national-record-locator-for-ambulance-services" },
                    { "^/personnel/5a8dabea952e372cbdb318de(/[^/]+)?$", "https://digital.nhs.uk/services/national-record-locator/national-record-locator-for-mental-health-trusts" },
                    { "^/personnel/5a8dabea952e372cbdb318e1(/[^/]+)?$", "https://digital.nhs.uk/services/national-record-locator/national-record-locator-for-mental-health-trusts" },
                    { "^/personnel/5a8dabea952e372cbdb318e2(/[^/]+)?$", "https://digital.nhs.uk/services/national-record-locator/national-record-locator-for-mental-health-trusts" },
                    { "^/personnel/5a8dabea952e372cbdb318e3(/[^/]+)?$", "https://digital.nhs.uk/services/national-record-locator/national-record-locator-for-mental-health-trusts" },
                    { "^/privacy-policy$", "https://digital.nhs.uk/about-nhs-digital/privacy-and-cookies" },
                    { "^/cookie-policy$", "https://digital.nhs.uk/about-nhs-digital/privacy-and-cookies" },
                    { "^/accessibility$", "https://digital.nhs.uk/about-nhs-digital/accessibility" },
                    { "^/phase-one-beta-go-live$", "https://digital.nhs.uk/services/national-record-locator" }
                };
            }
        }
    }
}
