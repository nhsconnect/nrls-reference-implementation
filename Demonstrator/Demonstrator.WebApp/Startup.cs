using Demonstrator.Core.Interfaces.Database;
using Demonstrator.Core.Interfaces.Services.Fhir;
using Demonstrator.Core.Interfaces.Services.Flows;
using Demonstrator.Database;
using Demonstrator.NRLSAdapter.Organisations;
using Demonstrator.NRLSAdapter.Patients;
using Demonstrator.Models.Core.Models;
using Demonstrator.Services.Service.Flows;
using Demonstrator.NRLSAdapter.DocumentReferences;
using Demonstrator.Core.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using Demonstrator.Core.Interfaces.Services.Nrls;
using Demonstrator.Services.Service.Nrls;
using Demonstrator.WebApp.Core.Middlewares;
using Demonstrator.Services.Service.Epr;
using Demonstrator.Core.Interfaces.Services.Epr;
using Demonstrator.Core.Interfaces.Services;
using Demonstrator.NRLSAdapter.Helpers;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace Demonstrator.WebApp
{
    public class Startup
    {
        public Startup()
        {
            Configuration = ConfigurationHelper.GetConfigurationRoot();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddOptions();
            services.AddNodeServices();
            services.Configure<DbSetting>(options =>
            {
                options.ConnectionString = Configuration.GetSection("NRLSMongoDb:ConnectionString").Value;
                options.Database = Configuration.GetSection("NRLSMongoDb:Database").Value;
            });
            services.Configure<ApiSetting>(options =>
            {
                options.BaseUrl = Configuration.GetSection("DemonstratorApi:BaseUrl").Value;
                options.Secure = bool.Parse(Configuration.GetSection("DemonstratorApi:Secure").Value);
                options.SecureOnly = bool.Parse(Configuration.GetSection("DemonstratorApi:SecureOnly").Value);
                options.DefaultPort = Configuration.GetSection("DemonstratorApi:DefaultPort").Value;
                options.SecurePort = Configuration.GetSection("DemonstratorApi:SecurePort").Value;
                options.SupportedContentTypes = Configuration.GetSection("DemonstratorApi:SupportedContentTypes").Get<IList<string>>();
            });
            services.Configure<ExternalApiSetting>(options =>
            {
                options.NrlsServerUrl = new Uri(Configuration.GetSection("NRLSAPI:ServerUrl").Value);
                options.NrlsSecureServerUrl = new Uri(Configuration.GetSection("NRLSAPI:SecureServerUrl").Value);
                options.NrlsUseSecure = bool.Parse(Configuration.GetSection("NRLSAPI:Secure").Value);
                options.NrlsDefaultprofile = Configuration.GetSection("NRLSAPI:DefaultProfile").Value;

                options.PdsServerUrl = new Uri(Configuration.GetSection("PDSAPI:ServerUrl").Value);
                options.PdsSecureServerUrl = new Uri(Configuration.GetSection("PDSAPI:SecureServerUrl").Value);
                options.PdsUseSecure = bool.Parse(Configuration.GetSection("PDSAPI:Secure").Value);
                options.PdsDefaultprofile = Configuration.GetSection("PDSAPI:DefaultProfile").Value;

                options.OdsServerUrl = new Uri(Configuration.GetSection("ODSAPI:ServerUrl").Value);
                options.OdsSecureServerUrl = new Uri(Configuration.GetSection("ODSAPI:SecureServerUrl").Value);
                options.OdsUseSecure = bool.Parse(Configuration.GetSection("ODSAPI:Secure").Value);
                options.OdsDefaultprofile = Configuration.GetSection("ODSAPI:DefaultProfile").Value;

                options.SpineAsid = Configuration.GetSection("Spine:Asid").Value;
                options.SpineThumbprint = Configuration.GetSection("Spine:SslThumbprint").Value;
                options.ClientAsidMapFile = Configuration.GetSection("Spine:ClientAisMapFile").Value;
            });
            services.AddTransient<INRLSMongoDBContext, NRLSMongoDBContext>();
            services.AddTransient<IActorOrganisationService, ActorOrganisationService>();
            services.AddTransient<IPersonnelService, PersonnelService>();
            services.AddTransient<IGenericSystemService, GenericSystemService>();
            services.AddTransient<IPatientServices, PatientServices>();
            services.AddTransient<IOrganisationServices, OrganisationServices>();
            services.AddTransient<IDocumentReferenceServices, DocumentReferenceServices>();
            services.AddTransient<IPointerService, PointerService>();
            services.AddTransient<IPatientViewService, PatientViewService>();
            services.AddTransient<ICrisisPlanService, CrisisPlanService>();
            services.AddTransient<IBenefitsService, BenefitsService>(); 
            services.AddTransient<IBenefitsViewService, BenefitsViewService>();
            services.AddTransient<IPointerMapService, PointerMapService>();
            services.AddTransient<IClientAsidHelper, ClientAsidHelper>();
            services.AddTransient<IDocumentsServices, DocumentsServices>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseExceptionHandler(new ExceptionHandlerOptions
            {
                ExceptionHandler = new JsonExceptionMiddleware(env).Invoke
            });

            app.UseClientInteractionCacheMiddleware();
            app.Use(async (context, next) =>
            {
                await next();

                var validLocations = new[] { "resources", "images", "api" };

                if (context.Response.StatusCode == 404)
                {
                    context.Request.Path = "/index.html";
                    await next();
                }
            });

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseMvc();
        }
    }
}
