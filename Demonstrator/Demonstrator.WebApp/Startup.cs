﻿using Demonstrator.Core.Interfaces.Database;
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
            services.Configure<NrlsApiSetting>(options =>
            {
                options.ServerUrl = new Uri(Configuration.GetSection("NRLSAPI:ServerUrl").Value);
            });
            services.AddTransient<INRLSMongoDBContext, NRLSMongoDBContext>();
            services.AddTransient<IActorOrganisationService, ActorOrganisationService>();
            services.AddTransient<IPersonnelService, PersonnelService>();
            services.AddTransient<IGenericSystemService, GenericSystemService>();
            services.AddTransient<IPatientServices, PatientServices>();
            services.AddTransient<IOrganisationServices, OrganisationServices>();
            services.AddTransient<IDocumentReferenceServices, DocumentReferenceServices>();
            services.AddTransient<IPointerService, PointerService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseMvc();
        }
    }
}
