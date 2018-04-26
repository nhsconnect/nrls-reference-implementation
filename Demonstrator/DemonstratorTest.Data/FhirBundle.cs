using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DemonstratorTest.Data.Helpers
{
    public static class FhirBundle
    {
        public static Bundle GetBundle<T>(IList<T> resources) where T : Resource
        {
            var bundle = new Bundle
            {
                Id = Guid.NewGuid().ToString(),
                Meta = new Meta
                {
                    LastUpdated = DateTime.UtcNow,
                    VersionId = Guid.NewGuid().ToString()
                },
                Type = Bundle.BundleType.Searchset,
                Total = resources.Count,
                Link = new List<Bundle.LinkComponent>
                {
                    new Bundle.LinkComponent
                    {
                        Relation = "_self",
                        Url = "TestLink"
                    }
                },
                Entry = resources.Select(r => new Bundle.EntryComponent
                {
                    Search = new Bundle.SearchComponent
                    {
                        Mode = Bundle.SearchEntryMode.Match
                    },
                    FullUrl = $"ResourceLink",
                    Resource = r
                }).ToList()

            };

            return bundle;
        }
    }
}
