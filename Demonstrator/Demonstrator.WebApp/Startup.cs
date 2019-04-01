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
using Microsoft.Extensions.Options;
using System;
using Demonstrator.Core.Interfaces.Services.Nrls;
using Demonstrator.Services.Service.Nrls;
using Demonstrator.WebApp.Core.Middlewares;
using Demonstrator.Services.Service.Epr;
using Demonstrator.Core.Interfaces.Services.Epr;
using Demonstrator.Core.Interfaces.Services;
using Demonstrator.NRLSAdapter.Helpers;
using System.Collections.Generic;
using Demonstrator.Core.Interfaces.Helpers;
using Demonstrator.Core.Helpers;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace Demonstrator.WebApp
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup()
        {
            Configuration = ConfigurationHelper.GetConfigurationRoot();
        }

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

                options.SspServerUrl = new Uri(Configuration.GetSection("SSP:ServerUrl").Value);
                options.SspSecureServerUrl = new Uri(Configuration.GetSection("SSP:SecureServerUrl").Value);
                options.SspUseSecure = bool.Parse(Configuration.GetSection("SSP:Secure").Value);

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
            services.AddTransient<IJwtHelper, JwtHelper>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IOptionsSnapshot<ApiSetting> nrlsApiSettings)
        {
            app.UseExceptionHandler(new ExceptionHandlerOptions
            {
                ExceptionHandler = new FhirExceptionMiddleware(env, nrlsApiSettings).Invoke
            });

            app.UseClientInteractionCacheMiddleware();

            app.UseWhen(cxt => cxt.Request.Path.StartsWithSegments(new PathString("/provider")), HandleProviderEndpoints);

            app.UseWhen(cxt => !cxt.Request.Path.StartsWithSegments(new PathString("/provider")), HandleSpaRequests);

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseMvc();
        }

        private static void HandleProviderEndpoints(IApplicationBuilder app)
        {
            app.UseLoggingMiddleware();
            app.UseSecureInputMiddleware();
            app.UseFhirInputMiddleware();

        }

        private static void HandleSpaRequests(IApplicationBuilder app)
        {
            app.UseSpaPushStateMiddleware();
        }
    }
}
