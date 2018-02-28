using Demonstrator.NRLSAdapter.Helpers;
using Hl7.Fhir.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Demonstrator.NRLSAdapter
{
    class Program
    {
        static void Main(string[] args)
        {
            var cmdHelper = new FhirCommand(args);

            var argsList = args.ToList();

            if (argsList.FirstOrDefault(a => new List<string> { "-h", "--help" }.Contains(a.ToLowerInvariant())) != null)
            {
                PrintHelp();
                return;
            }

            if (argsList.FirstOrDefault(a => new List<string> { "-v", "--version" }.Contains(a.ToLowerInvariant())) != null)
            {
                PrintVersion();
                return;
            }

            var validOptions = cmdHelper.StoreValidOptions();

            if (!validOptions.Success)
            {
                PrintError(validOptions.Message);
                return;
            }

            var validCmd = cmdHelper.ParseCommand();

            if (!validCmd.Success)
            {
                PrintError(validCmd.Message);
                return;
            }


            var response = cmdHelper.RunCommand().Result;

            if (!response.Success)
            {
                PrintError(response.Message);
                return;
            }

            var jsonSerializer = new FhirJsonSerializer();
            var jsonPointers = jsonSerializer.SerializeToString(response.Result);
            var jsonContent = jsonPointers.ToString();

            var outputLocation = cmdHelper.GetOutputLocation();

            if(outputLocation == null)
            {
                Console.WriteLine(jsonContent);
            }
            else
            {
                if (!outputLocation.Success)
                {
                    PrintError(outputLocation.Message);
                    return;
                }

                File.WriteAllText(outputLocation.Result, jsonContent);
            }


            Console.WriteLine("Request Complete.");
            Console.WriteLine("Press any key to stop...");
            Console.ReadKey();
        }

        

        static void PrintVersion()
        {
            Console.WriteLine(Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>().Version);
        }

        static void PrintHelp()
        {
            Console.WriteLine("Usage: Demonstrator.NRLSAdapter [OPTIONS]");
            Console.WriteLine("Allows you to Query the NRLS API via the CLI.");
            Console.WriteLine("");

            Console.WriteLine("OPTIONS:");
            Console.WriteLine("    -b,--body          The body of the request in the format specficied by the 'format' OPTION.");
            Console.WriteLine("                       Can be used with 'Method' OPTIONS of POST or PUT. The body should be ");
            Console.WriteLine("                       wrapped in double quotes.");

            Console.WriteLine("    -f,--format        The format of the content supplied (when POST or PUT methods are used) ");
            Console.WriteLine("                       and the format of the response. Currently only JSON is supported. If not ");
            Console.WriteLine("                       supplied defaults to JSON.");

            Console.WriteLine("    -h,--help          Displays this Help content.");

            Console.WriteLine("    -m,--method        The HTTP method to use in the request. Allowed values include GET, POST, ");
            Console.WriteLine("                       PUT. The Method GET supports Search but not Read. Methods POST and PUT ");
            Console.WriteLine("                       require the 'body' OPTION to be populated. The Method PUT requires the ");
            Console.WriteLine("                       'uniqueid' option to be populated.");

            Console.WriteLine("    -o,--output        The location and file name of where to store the results. If not provided ");
            Console.WriteLine("                       the output will be printed inline.");

            Console.WriteLine("    -p,--parameters    A array of pipe delimited key/value pair search parameters to include in a ");
            Console.WriteLine("                       GET request. Allowed keys are limited to 'patient' and 'custodian' ");
            Console.WriteLine("                       e.g. custodian|RR8,patient|500000000");

            Console.WriteLine("    -r,--resource      The FHIR Resource type. Allowed values are limited to 'DocumentReference'");

            Console.WriteLine("    -u,--uniqueid      The unique id of the resource stored in the NRLS. This is required for the ");
            Console.WriteLine("                       PUT method OPTION.");

            Console.WriteLine("    -v,--version       Displays the current console application version.");
        }

        static void PrintError(string error)
        {
            Console.WriteLine($"{error} Use the --help option to understand the allowed options.");
            return;
        }

        

    }
}
