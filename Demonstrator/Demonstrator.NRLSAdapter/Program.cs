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

            var outputLocation = cmdHelper.GetOutputLocation();

            if(outputLocation == null)
            {
                Console.WriteLine(response.Result);
            }
            else
            {
                if (!outputLocation.Success)
                {
                    PrintError(outputLocation.Message);
                    return;
                }

                File.WriteAllText(outputLocation.Result, response.Result);
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
            Console.WriteLine("                       Can be used with 'Method' options of POST or PUT. The body should be ");
            Console.WriteLine("                       wrapped in double quotes. An example: ");
            Console.WriteLine("                       {");
            Console.WriteLine("                           'Id': 'string',");
            Console.WriteLine("                           'OrgCode': 'string',");
            Console.WriteLine("                           'NhsNumber': 'string',");
            Console.WriteLine("                           'Url': 'string',");
            Console.WriteLine("                           'ContentType': 'string',");
            Console.WriteLine("                           'TypeCode': 'string',");
            Console.WriteLine("                           'TypeDisplay': 'string',");
            Console.WriteLine("                           'Creation': 'FHIR Date'");
            Console.WriteLine("                       }");
            Console.WriteLine("                       NOTE: Id is only required for the PUT option.");
            Console.WriteLine("                       This is an alternative to the input option. If Body is supplied input will be ignored.");

            Console.WriteLine("    -f,--format        The format of the content supplied (when POST or PUT methods are used) ");
            Console.WriteLine("                       and the format of the response. Currently only JSON is supported. If not ");
            Console.WriteLine("                       supplied defaults to JSON.");

            Console.WriteLine("    -h,--help          Displays this Help content.");

            Console.WriteLine("    -i,--input         The location and filename of where to get the complete NRLS DocumentReference ");
            Console.WriteLine("                       for POST and PUT methods. This is an alternative to the body option.");
            Console.WriteLine("                       If Body is supplied Input will be ignored.");

            Console.WriteLine("    -m,--method        The HTTP method to use in the request. Allowed values include GET, POST, ");
            Console.WriteLine("                       PUT. The Method GET supports Search but not Read. Methods POST and PUT ");
            Console.WriteLine("                       require the 'body' option to be populated. The Method PUT requires the ");
            Console.WriteLine("                       'uniqueid' option to be populated.");

            Console.WriteLine("    -o,--output        The location and file name of where to store the results. If not provided ");
            Console.WriteLine("                       the output will be printed inline.");

            Console.WriteLine("    -p,--parameters    A array of pipe delimited key/value pair search parameters to include in a ");
            Console.WriteLine("                       GET request. Allowed keys are limited to 'patient' and 'custodian' ");
            Console.WriteLine("                       e.g. custodian|RR8,patient|500000000");

            Console.WriteLine("    -r,--resource      The FHIR Resource type. Allowed values are limited to 'DocumentReference'");

            Console.WriteLine("    -s,--setHeaders    An array of pipe delimited key/value pair headers used for the request. ");
            Console.WriteLine("                       The Header array is required. See the NRLS spec for specific details. ");
            Console.WriteLine("                       NOTE: Accept is generated from the format value and Authorization is auto generated.");
            Console.WriteLine("                       e.g. fromASID|200000000117,toASID|999999999999");

            //Now a parameter
            //Console.WriteLine("    -i,--id            The unique id of the resource stored in the NRLS. This is required for the ");
            //Console.WriteLine("                       PUT method OPTION.");

            Console.WriteLine("    -v,--version       Displays the current console application version.");
        }

        static void PrintError(string error)
        {
            Console.WriteLine($"{error} Use the --help option to understand the allowed options.");
            return;
        }

        

    }
}
