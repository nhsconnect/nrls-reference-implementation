﻿using Microsoft.AspNetCore.Builder;
using Demonstrator.WebApp.Core.Middlewares;

namespace Demonstrator.WebApp.Core.Configuration
{
    public class FhirInputMiddlewarePipeline
    {
        public void Configure(IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseFhirInputMiddleware();
        }
    }
}
