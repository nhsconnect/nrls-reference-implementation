using Microsoft.Extensions.Configuration;
using System;

namespace NRLS_API.Core.Helpers
{
    public class ConfigurationHelper
    {
        public static IConfiguration GetConfigurationRoot()
        {
            IConfiguration configuration = null;

            var basePath = DirectoryHelper.GetBaseDirectory();

            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(basePath)
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
