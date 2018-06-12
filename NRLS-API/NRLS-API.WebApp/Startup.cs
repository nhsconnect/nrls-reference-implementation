using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NRLS_API.Core.Helpers;
using NRLS_API.Core.Interfaces.Database;
using NRLS_API.Core.Interfaces.Services;
using NRLS_API.Database;
using NRLS_API.Models;
using NRLS_API.Models.Core;
using NRLS_API.Services;
using NRLS_API.WebApp.Core.Formatters;
using NRLS_API.WebApp.Core.Middlewares;

namespace NRLS_API.WebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddMemoryCache();
            services.AddMvc(config =>
            {
                config.InputFormatters.Clear();

                //Default to fhir+json
                config.InputFormatters.Insert(0, new FhirJsonInputFormatter());
                config.OutputFormatters.Insert(0, new FhirJsonOutputFormatter());
                config.InputFormatters.Insert(1, new FhirXmlInputFormatter());
                config.OutputFormatters.Insert(1, new FhirXmlOutputFormatter());

                // Add FHIR Content Negotiation
                config.RespectBrowserAcceptHeader = true;
                config.ReturnHttpNotAcceptable = false;

            });
            services.AddOptions();
            services.Configure<DbSetting>(options =>
            {
                options.ConnectionString = Configuration.GetSection("NRLSMongoDb:ConnectionString").Value;
                options.Database = Configuration.GetSection("NRLSMongoDb:Database").Value;
            });
            services.Configure<NrlsApiSetting>(options =>
            {
                options.SupportedResources = Configuration.GetSection("NRLSAPI:SupportedResources").Value.Split(",").ToList();
                options.SupportedContentTypes = Configuration.GetSection("NRLSAPI:SupportedContentTypes").Value.Split(",").ToList();
                options.ProfileUrl = Configuration.GetSection("NRLSAPI:ProfileUrl").Value;
                options.BaseUrl = Configuration.GetSection("NRLSAPI:BaseUrl").Value;
                options.Secure = bool.Parse(Configuration.GetSection("NRLSAPI:Secure").Value);
                options.SecureOnly = bool.Parse(Configuration.GetSection("NRLSAPI:SecureOnly").Value);
                options.DefaultPort = Configuration.GetSection("NRLSAPI:DefaultPort").Value;
                options.SecurePort = Configuration.GetSection("NRLSAPI:DefaultPort").Value;
            });
            services.Configure<SpineSetting>(options =>
            {
                options.Asid = Configuration.GetSection("Spine:Asid").Value;
                options.ClientAsidMapFile = Configuration.GetSection("Spine:ClientAsidMapFile").Value;
                options.Thumbprint = Configuration.GetSection("Spine:Thumbprint").Value;
                options.ThumbprintKey = Configuration.GetSection("Spine:ThumbprintKey").Value;
            });
            services.AddTransient<INRLSMongoDBContext, NRLSMongoDBContext>();
            services.AddTransient<IFhirSearch, FhirSearch>();
            services.AddTransient<IFhirMaintain, FhirMaintain>();
            services.AddTransient<IValidationHelper, ValidationHelper>();
            services.AddTransient<IFhirValidation, FhirValidation>();
            services.AddTransient<INrlsSearch, NrlsSearch>();
            services.AddTransient<INrlsMaintain, NrlsMaintain>();
            services.AddTransient<INrlsConformance, NrlsConformance>();
            services.AddTransient<INrlsValidation, NrlsValidation>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            app.UseExceptionHandler(new ExceptionHandlerOptions
            {
                ExceptionHandler = new JsonExceptionMiddleware(env).Invoke
            });

            app.UseCors(builder => builder.WithOrigins(new[] { "*" }).WithMethods(new[]{ "GET", "POST", "PUT", "DELETE" }).AllowAnyHeader());

            app.UseClientInteractionCacheMiddleware();

            //handle compression as per spec
            app.UseFhirInputMiddleware();
            //app.UseFhirOuputMiddleware();

            app.UseMvc();

        }
    }
}
