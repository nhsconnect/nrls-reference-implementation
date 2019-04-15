using System;
using System.Net;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using Demonstrator.Core.Helpers;
using Demonstrator.Models.Core.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Demonstrator.WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = BuildWebHost(args);

            //var logger = host.Services.GetRequiredService<ILogger<Program>>();
            //logger.LogInformation("Somethign just happened!");

            host.Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {

            var config = GetConfigurationRoot();

            ApiSetting apiSettings = new ApiSetting();
            config.GetSection("DemonstratorApi").Bind(apiSettings);

            return WebHost.CreateDefaultBuilder(args)
                .UseKestrel(options =>
                {
                    options.AddServerHeader = false;
                    // if we use the apiSettings.Secure flag we can toggle the below
                    // for demo we will leave both open


                    // listen for HTTP
                    if (!apiSettings.SecureOnly)
                    {
                        options.Listen(IPAddress.Any, int.Parse(apiSettings.DefaultPort));
                    }
                   

                    // listen for HTTPS
                    if (apiSettings.Secure)
                    {
                        var certificate = ServerCertificate();

                        if (certificate != null)
                        {
                            options.Listen(IPAddress.Any, int.Parse(apiSettings.SecurePort), listenOptions =>
                            {
                                listenOptions.UseHttps(new HttpsConnectionAdapterOptions
                                {
                                    SslProtocols = SslProtocols.Tls12,
                                    CheckCertificateRevocation = false, //TODO: turn this on
                                    ClientCertificateMode = ClientCertificateMode.NoCertificate,
                                    ServerCertificate = certificate
                                });
                            });
                        }
                    }

                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                    //logging.AddDebug();

                    //TODO: add log persistence store
                })
                .UseStartup<Startup>()
                .Build();
        }

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


        private static X509Certificate2 ServerCertificate()
        {
            using (var store = new X509Store(StoreName.My, StoreLocation.CurrentUser))
            {
                store.Open(OpenFlags.ReadOnly);
                var serverCertificates = store.Certificates.Find(X509FindType.FindByThumbprint, "b8062eae75405afafb975e126b179ea1211f06c8", false);
                if (serverCertificates.Count > 0)
                {
                    return serverCertificates[0];
                }

                return null;
            }
        }
    }
}
