using Demonstrator.Core.Interfaces.Database;
using Demonstrator.Core.Interfaces.Services.Flows;
using Demonstrator.Database;
using Demonstrator.Models.Core.Models;
using Demonstrator.Services.Service.Flows;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Demonstrator.WebApp
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
            services.AddMvc();
            services.Configure<DbSetting>(options =>
            {
                options.ConnectionString = Configuration.GetSection("NRLSMongoDb:ConnectionString").Value;
                options.Database = Configuration.GetSection("NRLSMongoDb:Database").Value;
            });
            services.AddTransient<INRLSMongoDBContext, NRLSMongoDBContext>();
            services.AddTransient<IActorOrganisationService, ActorOrganisationService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
