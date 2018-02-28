using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Demonstrator.Core.Configuration
{
    public static class ConfigurationHelper
    {
        public static IConfiguration GetConfigurationRoot()
        {
            IConfiguration configuration = null;

            var basePath = AppContext.BaseDirectory.Substring(0, AppContext.BaseDirectory.LastIndexOf("bin"));
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile(Path.Combine(basePath, "..", "Shared", "globalsettings.json"), optional: false)
                .AddJsonFile("appsettings.json", optional: true);

            if (!string.IsNullOrEmpty(environmentName))
            {
                configurationBuilder = configurationBuilder.AddJsonFile($"appsettings.{environmentName}.json", optional: true);
            }

            configuration = configurationBuilder.Build();

            return configuration;
        }
    }
}
