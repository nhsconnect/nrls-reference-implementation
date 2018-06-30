using Hl7.Fhir.Model;

namespace NRLS_API.Models.Extensions
{
    public static class SearchParameters
    {
        public static string[] GetAllowed(this ResourceType resourceType)
        {
            return AllowedParameters(resourceType.ToString());
        }


        //move to config
        private static string[] AllowedParameters(string resourceType)
        {
            switch (resourceType)
            {
                case "Patient":
                    return new[] { "identifier" };
                case "Organization":
                    return new[] { "identifier" };
                case "DocumentReference":
                    return new[] { "custodian", "subject", "_id", "type" };
                default:
                    return new string[] { };
            }
        }
    }
}
