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

            var appPath = AppContext.BaseDirectory;
            var pathEnd = appPath.LastIndexOf("bin");
            var isBinPath = !(pathEnd < 0);
            var basePath = isBinPath ? appPath.Substring(0, pathEnd) : appPath;
            var sharedPath = isBinPath ? @"..\Shared" : "Shared";

            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile(Path.Combine(basePath, sharedPath, "globalsettings.json"), optional: false)
                .AddJsonFile("appsettings.json", optional: true);

            if (!string.IsNullOrEmpty(environmentName))
            {
                configurationBuilder = configurationBuilder.AddJsonFile($"appsettings.{environmentName}.json", optional: true);
                configurationBuilder = configurationBuilder.AddJsonFile(Path.Combine(basePath, sharedPath, $"globalsettings.{environmentName}.json"), optional: true);
            }

            configuration = configurationBuilder.Build();
                
            return configuration;
        }
    }
}
