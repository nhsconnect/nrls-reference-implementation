using Microsoft.AspNetCore.Builder;
using NRLS_API.WebApp.Core.Middlewares;

namespace NRLS_API.WebApp.Core.Configuration
{
    public class SspAuthorizationPipeline
    {
        public void Configure(IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseSspAuthorizationMiddleware();
        }
    }
}
