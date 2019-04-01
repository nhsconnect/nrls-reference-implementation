using Microsoft.AspNetCore.Builder;
using NRLS_API.WebApp.Core.Middlewares;

namespace NRLS_API.WebApp.Core.Configuration
{
    public class SpineAuthorizationPipeline
    {
        public void Configure(IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseSpineAuthorizationMiddleware();
        }
    }
}
