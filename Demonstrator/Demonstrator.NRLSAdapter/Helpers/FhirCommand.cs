using Demonstrator.Core.Configuration;
using Demonstrator.Core.Interfaces.Services;
using Demonstrator.Core.Interfaces.Services.Fhir;
using Demonstrator.Models.Core.Models;
using Demonstrator.Models.Nrls;
using Demonstrator.NRLSAdapter.DocumentReferences;
using Demonstrator.NRLSAdapter.Models;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
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
        private readonly IClientAsidHelper _clientAsidHelper;

        private string[] _args = new string[0];

        private string _body = null;
        private NrlsPointerBody _pointerBody = null;
        private DocumentReference _pointer = null;
        private string _format = ContentType.JSON_CONTENT_HEADER;
        private string _input = null;
        private string _method = null;
        private string _output = null;
        private IDictionary<string, string> _parameters = null;
        private IDictionary<string, string> _headers = null;
        private string _resource = null;
        //private string _uniqueId = null;

        public FhirCommand(string[] args)
        {
            ServiceProvider serviceProvider;
            ConfigureServices(out serviceProvider);

            _docRefService = serviceProvider.GetService<IDocumentReferenceServices>();
            _clientAsidHelper = serviceProvider.GetService<IClientAsidHelper>();

            _clientAsidHelper.Load();

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

                    var optionValue = _args.ElementAt(optionValueKey);

                    if (!string.IsNullOrEmpty(optionValue) && !SensitiveOptions.Contains(validOption.Key))
                    {
                        optionValue = optionValue.ToLowerInvariant();
                    }

                    //If the OPTION has set values, check the incoming against this list
                    if (validOption.Key != "parameters" && validOption.Key != "setheaders" && validOption.Value.Any() && optionValue != null && !validOption.Value.Contains(optionValue))
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

            if (key.Equals("input"))
            {
                _input = value;
            }

            if (key.Equals("setheaders"))
            {
                _headers = GetOptionDictionay(key, value);
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

            //now a parameter
            //if (key.Equals("id"))
            //{
            //    _uniqueId = value;
            //}
        }

        private IDictionary<string, string> GetOptionDictionay(string key, string value)
        {
            var paramsList = value?.Split(",") ?? new string[0];

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

            if (_headers == null || !_headers.Any())
            {
                return CommandResponse.Set(false, $"All requests requires the setheaders option.");
            }

            if (!ValidMethods.Contains(_method))
            {
                return CommandResponse.Set(false, $"The Method {_method} is invalid.");
            }

            if (_method == "get" || _method == "delete" || _method == "put")
            {
                if (_parameters == null || !_parameters.Any())
                {
                    return CommandResponse.Set(false, $"The Parameters OPTION is missing. The GET, PUT and DELETE method requires parameters.");
                }

                message.AppendLine("Parameters: " + _parameters);
            }

            if ((_method == "put" || _method == "post"))
            {
                if (!string.IsNullOrWhiteSpace(_body))
                {
                    try
                    {
                        _pointerBody = JsonConvert.DeserializeObject<NrlsPointerBody>(_body);
                    }
                    catch(Exception ex)
                    {
                        return CommandResponse.Set(false, $"The Body OPTION is invalid. Exception Message: {ex.Message}");
                    }

                }
                else if (!string.IsNullOrWhiteSpace(_input))
                {
                    var inputLocation = GetInputLocation();

                    if (inputLocation == null)
                    {
                        return CommandResponse.Set(false, $"The Input OPTION is invalid. See --help for more details.");
                    }
                    else
                    {
                        if (!inputLocation.Success)
                        {
                            return inputLocation;
                        }

                        try
                        {
                            var jsonParser = new FhirJsonParser();
                            var pointer = File.ReadAllText(inputLocation.Result);
                            _pointer = jsonParser.Parse<DocumentReference>(pointer);
                        }
                        catch(Exception ex)
                        {
                            return CommandResponse.Set(false, $"Error trying to parse input. Exception Message: {ex.Message}");
                        }
                        
                       
                    }
                }
                else
                {
                    return CommandResponse.Set(false, $"Both the Body OPTION and the Input OPTION are missing. PUT and POST methods require at least one.");
                }
            }

            ////not for beta
            //if (_method == "put")
            //{
            //    //its now a parameter
            //    if (string.IsNullOrWhiteSpace(_uniqueId))
            //    {
            //        return CommandResponse.Set(false, $"The id OPTION is missing. PUT methods require a id.");
            //    }

            //    message.AppendLine("id: " + _uniqueId);
            //}

            return CommandResponse.Set(true, message.ToString());
        }

        public async Task<CommandResult<string>> RunCommand()
        {
            
            var jsonSerializer = new FhirJsonSerializer();

            var jsonResponse = string.Empty;
            NrlsPointerRequest pointerRequest;

            var idParam = _parameters?.FirstOrDefault(n => n.Key.Equals("_id"));
            var asid = _headers.FirstOrDefault(n => n.Key.Equals("fromasid"));
            var interaction = _headers.FirstOrDefault(n => n.Key.Equals("ssp-interactionid"));


            //Massive Try/catch

            try
            {
                if (_method == "get")
                {
                    var patientParam = _parameters.FirstOrDefault(n => n.Key.Equals("patient"));
                    var custodianParam = _parameters.FirstOrDefault(n => n.Key.Equals("custodian"));


                    if (!string.IsNullOrEmpty(idParam.Value.Value))
                    {
                        pointerRequest = NrlsPointerRequest.Read(idParam.Value.Value, asid.Value, interaction.Value);
                    }
                    else
                    {
                        pointerRequest = NrlsPointerRequest.Search(custodianParam.Value, patientParam.Value, asid.Value, interaction.Value);
                    }

                    var pointers = await _docRefService.GetPointersAsBundle(pointerRequest);

                    jsonResponse = jsonSerializer.SerializeToString(pointers);
                }

                if (_method == "post")
                {
                    OperationOutcome documentOutcome;

                    if (_pointerBody != null)
                    {
                        pointerRequest = NrlsPointerRequest.Create(_pointerBody.OrgCode, _pointerBody.NhsNumber, _pointerBody.Url, _pointerBody.ContentType, _pointerBody.TypeCode, _pointerBody.TypeDisplay, asid.Value, interaction.Value);

                        var response = await _docRefService.GenerateAndCreatePointer(pointerRequest);

                        documentOutcome = response.Resource as OperationOutcome;
                    }
                    else
                    {
                        pointerRequest = NrlsPointerRequest.Create(null, null, null, null, null, null, asid.Value, interaction.Value);

                        var response = await _docRefService.CreatePointer(pointerRequest, _pointer);

                        documentOutcome = response.Resource as OperationOutcome;
                    }


                    jsonResponse = jsonSerializer.SerializeToString(documentOutcome);
                }

                if (_method == "put")
                {
                    //not implemented
                }

                if (_method == "delete")
                {

                    pointerRequest = NrlsPointerRequest.Delete(idParam.Value.Value, asid.Value, interaction.Value);

                    var outcome = await _docRefService.DeletePointer(pointerRequest);

                    jsonResponse = jsonSerializer.SerializeToString(outcome);
                }
            }
            catch(Exception ex)
            {
                jsonResponse = ex.Message;
            }
            


            return CommandResult<string>.Set(true, "Success", jsonResponse.ToString());
        }

        public CommandResult<string> GetOutputLocation()
        {
            return GetLocation(_output);
        }

        public CommandResult<string> GetInputLocation()
        {
            return GetLocation(_input);
        }

        public CommandResult<string> GetLocation(string _location)
        {
            if (string.IsNullOrWhiteSpace(_location))
            {
                return null;
            }

            var location = Path.GetDirectoryName(_location);

            if (!Directory.Exists(location))
            {
                return CommandResult<string>.Set(false, "Invalid location supplied.", null);
            }

            return CommandResult<string>.Set(true, "Success.", _location);
        }

        private static bool IsEven(int value)
        {
            return value % 2 == 0;
        }

        private static IDictionary<string, List<string>> ValidOptions => new Dictionary<string, List<string>> {
            { "body", new List<string>() },
            { "format", ValidFormats },
            { "help", new List<string>() },
            { "input", new List<string>() },
            { "method", ValidMethods },
            { "output", new List<string>() },
            { "parameters", ValidParameters },
            { "resource", ValidResources },
            { "setheaders", ValidHeaders },
            //{ "id", new List<string>() },
            { "version", new List<string>() }
        };

        private static List<string> SensitiveOptions = new List<string> { "parameters", "input", "output", "body" };

        private static List<string> ValidParameters = new List<string> { "patient", "custodian", "_id" };

        //Set to lower case for cmd line
        private static List<string> ValidHeaders = new List<string> { "ssp-traceid", "fromasid", "toasid", "ssp-interactionid", "ssp-version" };

        private static List<string> ValidFormats = new List<string> { "json" };

        private static List<string> ValidMethods = new List<string> { "get", "post", "delete" }; // put not in for beta

        private static List<string> ValidResources = new List<string> { "documentreference" };

        private void ConfigureServices(out ServiceProvider serviceProvider)
        {
            IConfiguration configuration = ConfigurationHelper.GetConfigurationRoot();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddMemoryCache();
            serviceCollection.AddOptions();
            serviceCollection.Configure<ExternalApiSetting>(options =>
            {
                options.NrlsServerUrl = new Uri(configuration.GetSection("NRLSAPI:ServerUrl").Value);
                options.NrlsSecureServerUrl = new Uri(configuration.GetSection("NRLSAPI:SecureServerUrl").Value);
                options.NrlsUseSecure = bool.Parse(configuration.GetSection("NRLSAPI:Secure").Value);
                options.NrlsDefaultprofile = configuration.GetSection("NRLSAPI:DefaultProfile").Value;

                options.PdsServerUrl = new Uri(configuration.GetSection("PDSAPI:ServerUrl").Value);
                options.PdsSecureServerUrl = new Uri(configuration.GetSection("PDSAPI:SecureServerUrl").Value);
                options.PdsUseSecure = bool.Parse(configuration.GetSection("PDSAPI:Secure").Value);
                options.PdsDefaultprofile = configuration.GetSection("PDSAPI:DefaultProfile").Value;

                options.OdsServerUrl = new Uri(configuration.GetSection("ODSAPI:ServerUrl").Value);
                options.OdsSecureServerUrl = new Uri(configuration.GetSection("ODSAPI:SecureServerUrl").Value);
                options.OdsUseSecure = bool.Parse(configuration.GetSection("ODSAPI:Secure").Value);
                options.OdsDefaultprofile = configuration.GetSection("ODSAPI:DefaultProfile").Value;


                options.SpineAsid = configuration.GetSection("Spine:Asid").Value;
                options.SpineThumbprint = configuration.GetSection("Spine:SslThumbprint").Value;
                options.ClientAsidMapFile = configuration.GetSection("Spine:ClientAisMapFile").Value;
            });
            serviceCollection.Configure<ApiSetting>(options =>
            {
                options.BaseUrl = configuration.GetSection("DemonstratorApi:BaseUrl").Value;
                options.Secure = bool.Parse(configuration.GetSection("DemonstratorApi:Secure").Value);
                options.DefaultPort = configuration.GetSection("DemonstratorApi:DefaultPort").Value;
                options.SecurePort = configuration.GetSection("DemonstratorApi:SecurePort").Value;
            });
            serviceCollection.AddTransient<IDocumentReferenceServices, DocumentReferenceServices>();
            serviceCollection.AddTransient<IClientAsidHelper, ClientAsidHelper>();
            serviceProvider = serviceCollection.BuildServiceProvider();
        }
    }
}
