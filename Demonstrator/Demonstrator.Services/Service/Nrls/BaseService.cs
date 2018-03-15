using Hl7.Fhir.Model;
using System.Collections.Generic;
using System.Linq;

namespace Demonstrator.Services.Service.Nrls
{
    public class BaseService
    {
        protected List<T> ListEntries<T>(List<Bundle.EntryComponent> entries, ResourceType resType) where T : Resource
        {
            return entries
                    .Where(entry => entry.Resource.ResourceType.Equals(resType))
                    .Select(entry => (T)entry.Resource)
                    .ToList();
        }
    }
}
