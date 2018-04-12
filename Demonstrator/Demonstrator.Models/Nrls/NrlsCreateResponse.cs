using Demonstrator.Utilities.Extensions;
using Hl7.Fhir.Model;
using System;

namespace Demonstrator.Models.Nrls
{
    public class NrlsCreateResponse
    {
        public Resource Resource { get; set; }

        public Uri ResponseLocation { get; set; }

        public string ResourceId => GetResourceId();

        private string GetResourceId()
        {
            var param = ResponseLocation?.Query?.ParseQuery();

            if (param != null && param.ContainsKey("_id"))
            {
                return param["_id"];
            }

            return null;
        }
    }
}
