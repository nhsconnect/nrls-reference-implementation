using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;
using NRLS_API.Core.Helpers;
using NRLS_API.Core.Interfaces.Database;
using NRLS_API.Core.Interfaces.Helpers;
using NRLS_API.Core.Interfaces.Services;
using NRLS_API.Database;
using NRLS_API.Models;
using NRLS_API.Models.Core;
using NRLS_API.Services;
using NRLS_API.WebApp.Core.Formatters;
using NRLS_API.WebApp.Core.Middlewares;
using Swashbuckle.AspNetCore.Swagger;

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

                //Default to fhir+xml

                config.InputFormatters.Insert(0, new FhirXmlInputFormatter());
                config.OutputFormatters.Insert(0, new FhirXmlOutputFormatter());
                config.InputFormatters.Insert(1, new FhirJsonInputFormatter());
                config.OutputFormatters.Insert(1, new FhirJsonOutputFormatter());

                // Add FHIR Content Negotiation
                config.RespectBrowserAcceptHeader = true;
                config.ReturnHttpNotAcceptable = false;

            });
            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new Info
            //    {
            //        Version = "v1",
            //        Title = "NRLS API Reference Implementation",
            //        Description = "A reference implementation of the NRLS API which conforms to the NRLS Technical Specification.",
            //        Contact = new Contact()
            //        {
            //            Name = "NRLS Team",
            //            Email = "nrls@nhs.net"
            //        }
            //    });

            //    var basePath = PlatformServices.Default.Application.ApplicationBasePath;
            //    var xmlPath = Path.Combine(basePath, "NRLS-API.WebApp.xml");
            //    c.IncludeXmlComments(xmlPath);
            //});
            services.AddOptions();
            services.Configure<DbSetting>(options =>
            {
                options.ConnectionString = Configuration.GetSection("NRLSMongoDb:ConnectionString").Value;
                options.Database = Configuration.GetSection("NRLSMongoDb:Database").Value;
            });
            services.Configure<ApiSetting>("NrlsApiSetting", options =>
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
            services.Configure<ApiSetting>("PdsApiSetting", options =>
            {
                options.ProfileUrl = Configuration.GetSection("PDSAPI:ProfileUrl").Value;
            });
            services.Configure<ApiSetting>("OdsApiSetting", options =>
            {
                options.ProfileUrl = Configuration.GetSection("ODSAPI:ProfileUrl").Value;
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
            services.AddTransient<IPdsSearch, PdsSearch>();
            services.AddTransient<IOdsSearch, OdsSearch>();
            services.AddTransient<INrlsMaintain, NrlsMaintain>();
            services.AddTransient<INrlsConformance, NrlsConformance>();
            services.AddTransient<INrlsValidation, NrlsValidation>();
            services.AddTransient<IJwtHelper, JwtHelper>();
            services.AddTransient<IFhirSearchHelper, FhirSearchHelper>();
            services.AddTransient<IFhirCacheHelper, FhirCacheHelper>();
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

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Resources")), RequestPath = "/Resources"
            });

            //app.UseSwagger();
            app.UseSwaggerUI(c => {
                c.RoutePrefix = "";
                c.SwaggerEndpoint("/Resources/swagger.json", "NRLS Reference Implementation");
                c.InjectStylesheet("/Resources/swagger-custom.css");
                c.DefaultModelsExpandDepth(-1);
                c.IndexStream = () =>   File.OpenText(Path.Combine(Directory.GetCurrentDirectory(), "Resources", "swagger-ui-index.html")).BaseStream; 
            });
        }
    }
}
