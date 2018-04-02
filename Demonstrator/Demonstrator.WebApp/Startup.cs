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
            services.Configure<DbSetting>(options =>
            {
                options.ConnectionString = Configuration.GetSection("NRLSMongoDb:ConnectionString").Value;
                options.Database = Configuration.GetSection("NRLSMongoDb:Database").Value;
            });
            services.Configure<ExternalApiSetting>(options =>
            {
                options.NrlsServerUrl = new Uri(Configuration.GetSection("NRLSAPI:ServerUrl").Value);
                options.PdsServerUrl = new Uri(Configuration.GetSection("PDSAPI:ServerUrl").Value);
                options.OdsServerUrl = new Uri(Configuration.GetSection("ODSAPI:ServerUrl").Value);
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
            services.AddTransient<IBenefitsService, BenefitsService>(); 
            services.AddTransient<IBenefitsViewService, BenefitsViewService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseExceptionHandler(new ExceptionHandlerOptions
            {
                ExceptionHandler = new JsonExceptionMiddleware(env).Invoke
            });

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseMvc();
        }
    }
}
