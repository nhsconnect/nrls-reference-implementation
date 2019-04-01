using Microsoft.AspNetCore.Builder;
using Demonstrator.WebApp.Core.Middlewares;

namespace Demonstrator.WebApp.Core.Configuration
{
    public class SecureInputMiddlewarePipeline
    {
        public void Configure(IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseSecureInputMiddleware();
        }
    }
}
