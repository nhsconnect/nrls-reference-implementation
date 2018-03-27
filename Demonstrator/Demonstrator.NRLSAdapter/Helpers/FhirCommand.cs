using Demonstrator.Core.Configuration;
using Demonstrator.Core.Interfaces.Services.Fhir;
using Demonstrator.Models.Core.Models;
using Demonstrator.NRLSAdapter.DocumentReferences;
using Demonstrator.NRLSAdapter.Models;
using Hl7.Fhir.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demonstrator.NRLSAdapter.Helpers
{
    public class FhirCommand
    {
        private readonly IDocumentReferenceServices _docRefService;
        private string[] _args = new string[0];

        private string _body = null;
        private string _format = null;
        private string _method = null;
        private string _output = null;
        private IDictionary<string, string> _parameters = null;
        private string _resource = null;
        private string _uniqueId = null;

        public FhirCommand(string[] args)
        {
            ServiceProvider serviceProvider;
            ConfigureServices(out serviceProvider);

            _docRefService = serviceProvider.GetService<IDocumentReferenceServices>();

            _args = args;
        }

        public CommandResponse StoreValidOptions()
        {
            if (_args.Length == 0)
            {
                return CommandResponse.Set(false, "Please enter valid arguments.");
            }

            for (var a = 0; a < _args.Length; a++)
            {
                //Only interested in option names.
                //If not a valid option name then the command is probably incorrect
                if (!IsEven(a))
                {
                    continue;
                }

                var arg = _args[a];
                if (!string.IsNullOrWhiteSpace(arg))
                {
                    var option = _args.ElementAt(a).Replace("-", "").ToLowerInvariant();
                    var optionValueKey = a + 1;

                    //Check if is valid option
                    var validOption = ValidOptions.FirstOrDefault(k => k.Key.Equals(option) || k.Key.First().ToString() == option);
                    
                    if (validOption.Key == null)
                    {
                        return CommandResponse.Set(false, $"The OPTION {option} is unknown.");
                    }

                    //Checks to see if last OPTION has a value
                    //Apart from -v & -h all other options should have a value
                    if (optionValueKey > _args.Length - 1)
                    {
                        return CommandResponse.Set(false, $"The OPTION {option} has not been supplied with a value.");
                    }

                    var optionValue = _args.ElementAt(optionValueKey)?.ToLowerInvariant();

                    //If the OPTION has set values, check the incoming against this list
                    if (validOption.Key != "parameters" && validOption.Value.Any() && optionValue != null && !validOption.Value.Contains(optionValue))
                    {
                        return CommandResponse.Set(false, $"The OPTION {option} value is invalid.");
                    }

                    //Store the Option values
                    //Doing this here because we already have the key/values parsed
                    StoreOptionValue(validOption.Key, optionValue);
                }
            }

            return CommandResponse.Set(true, "Command Validated.");
        }

        private void StoreOptionValue(string key, string value)
        {
            if (key.Equals("resource"))
            {
                _resource = value;
            }

            if (key.Equals("body"))
            {
                _body = value;
            }

            if (key.Equals("format"))
            {
                _format = value;
            }

            if (key.Equals("method"))
            {
                _method = value;
            }

            if (key.Equals("output"))
            {
                _output = value;
            }

            if (key.Equals("parameters"))
            {
                _parameters = GetOptionDictionay(key, value);
            }

            if (key.Equals("uniqueid"))
            {
                _uniqueId = value;
            }
        }

        private IDictionary<string, string> GetOptionDictionay(string key, string value)
        {
            var paramsList = value.Split(",");
            var parameters = new Dictionary<string, string>();

            for (var p = 0; p < paramsList.Length; p++)
            {
                var keyValParam = paramsList.ElementAt(p).Split("|");

                var validOptions = ValidOptions.FirstOrDefault(k => k.Key.Equals(key));

                if (keyValParam.Length == 2 && validOptions.Value.Contains(keyValParam.ElementAt(0)))
                {
                    parameters.Add(keyValParam.ElementAt(0), keyValParam.ElementAt(1));
                }
            }

            return parameters;
        }

        public CommandResponse ParseCommand()
        {
            var message = new StringBuilder();

            if (string.IsNullOrWhiteSpace(_resource))
            {
                return CommandResponse.Set(false, $"The Resource OPTION is missing. All requests require a Resource type to be specified.");
            }

            message.AppendLine("Resource: " + _resource);
            message.AppendLine("Format: " + _format);

            if (_method == "get")
            {
                if (_parameters == null || !_parameters.Any())
                {
                    return CommandResponse.Set(false, $"The Parameters OPTION is missing. The GET method requires parameters.");
                }

                message.AppendLine("Parameters: " + _parameters);
            }

            if ((_method == "put" || _method == "post") && string.IsNullOrWhiteSpace(_body))
            {
                return CommandResponse.Set(false, $"The Body OPTION is missing. PUT and POST methods require a Body.");
            }

            if (_method == "put")
            {
                if (string.IsNullOrWhiteSpace(_uniqueId))
                {
                    return CommandResponse.Set(false, $"The UniqueId OPTION is missing. PUT methods require a UniqueId.");
                }

                message.AppendLine("UniqueId: " + _uniqueId);
            }

            return CommandResponse.Set(true, message.ToString());
        }

        public async Task<CommandResult<Bundle>> RunCommand()
        {
            var pointers = new Bundle();

            if (_method == "get")
            {
                var patientParam = _parameters.FirstOrDefault(n => n.Key.Equals("patient"));
                var custodianParam = _parameters.FirstOrDefault(n => n.Key.Equals("custodian"));

                pointers = await _docRefService.GetPointersAsBundle(patientParam.Value, custodianParam.Value);
            }

            return CommandResult<Bundle>.Set(true, "Success", pointers);
        }

        public CommandResult<string> GetOutputLocation()
        {
            if (string.IsNullOrWhiteSpace(_output))
            {
                return null;
            }

            var location = Path.GetDirectoryName(_output);

            if (!Directory.Exists(location))
            {
                return CommandResult<string>.Set(false, "Invalid location supplied.", null);
            }

            return CommandResult<string>.Set(true, "Success.", _output);
        }

        private static bool IsEven(int value)
        {
            return value % 2 == 0;
        }

        private static IDictionary<string, List<string>> ValidOptions => new Dictionary<string, List<string>> {
            { "body", new List<string>() },
            { "format", ValidFormats },
            { "help", new List<string>() },
            { "method", ValidMethods },
            { "output", new List<string>() },
            { "parameters", ValidParameters },
            { "resource", ValidResources },
            { "uniqueid", new List<string>() },
            { "version", new List<string>() }
        };

        private static List<string> ValidParameters = new List<string> { "patient", "custodian" };

        private static List<string> ValidFormats = new List<string> { "json" };

        private static List<string> ValidMethods = new List<string> { "get", "post", "put" };

        private static List<string> ValidResources = new List<string> { "documentreference" };

        private void ConfigureServices(out ServiceProvider serviceProvider)
        {
            IConfiguration configuration = ConfigurationHelper.GetConfigurationRoot();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddOptions();
            serviceCollection.Configure<ExternalApiSetting>(options =>
            {
                options.NrlsServerUrl = new Uri(configuration.GetSection("NRLSAPI:ServerUrl").Value);
                options.PdsServerUrl = new Uri(configuration.GetSection("PDSAPI:ServerUrl").Value);
                options.OdsServerUrl = new Uri(configuration.GetSection("ODSAPI:ServerUrl").Value);
            });
            serviceCollection.AddTransient<IDocumentReferenceServices, DocumentReferenceServices>();
            serviceProvider = serviceCollection.BuildServiceProvider();
        }
    }
}
