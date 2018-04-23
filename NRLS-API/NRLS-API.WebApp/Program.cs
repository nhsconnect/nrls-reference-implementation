using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NRLS_API.Core.Exceptions;
using NRLS_API.Core.Factories;
using NRLS_API.Core.Helpers;
using NRLS_API.Models.Core;

namespace NRLS_API.WebApp
{
    public class Program
    {
        private static SpineSetting _spineSettings;

        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            var config = ConfigurationHelper.GetConfigurationRoot();

            _spineSettings = new SpineSetting();
            config.GetSection("Spine").Bind(_spineSettings);

            NrlsApiSetting apiSettings = new NrlsApiSetting();
            config.GetSection("NRLSAPI").Bind(apiSettings);

            return WebHost.CreateDefaultBuilder(args)
                .UseKestrel(options =>
                {
                    options.AddServerHeader = false;
                    // if we use the apiSettings.Secure flag we can toggle the below
                    // for demo we will leave both open

                    // listen for HTTP
                    options.Listen(IPAddress.Any, int.Parse(apiSettings.DefaultPort));


                    // listen for HTTPS
                    if (apiSettings.Secure)
                    {
                        var certificate = ServerCertificate(int.Parse(apiSettings.SecurePort));

                        if(certificate != null)
                        {
                            options.Listen(IPAddress.Any, int.Parse(apiSettings.SecurePort), listenOptions =>
                            {
                                listenOptions.UseHttps(new HttpsConnectionAdapterOptions
                                {
                                    SslProtocols = SslProtocols.Tls12,
                                    CheckCertificateRevocation = false, //TODO: turn this on
                                    ClientCertificateMode = ClientCertificateMode.AllowCertificate,
                                    ServerCertificate = certificate,
                                    ClientCertificateValidation = (cert, chain, error) => ValidateClient(cert, chain, error)
                                });
                            });
                        }
                    }

                })
                .UseStartup<Startup>()
                .Build();
        }

        private static X509Certificate2 ServerCertificate(int securePort)
        {
            using (var store = new X509Store(StoreName.My, StoreLocation.CurrentUser))
            {
                store.Open(OpenFlags.ReadOnly);
                var serverCertificates = store.Certificates.Find(X509FindType.FindByThumbprint, _spineSettings.Thumbprint, false);
                if (serverCertificates.Count > 0)
                {
                    return serverCertificates[0];
                }

                return null;
            }
        }

        private static bool ValidateClient(X509Certificate2 cert, X509Chain chain, SslPolicyErrors error)
        {
            using (var store = new X509Store(StoreName.TrustedPeople, StoreLocation.CurrentUser))
            {
                store.Open(OpenFlags.ReadOnly);

                //Just validate that we recognise
                //Update to ensure we grab certs in a cross platform way
                //Asid match will be done in middleware
                if(!store.Certificates.Contains(cert) || error != SslPolicyErrors.None)
                {
                    throw new HttpFhirException("Invalid Client Request Exception", OperationOutcomeFactory.CreateAccessDenied(), HttpStatusCode.Unauthorized);
                }

                return true;
            }
        }
    }
}
