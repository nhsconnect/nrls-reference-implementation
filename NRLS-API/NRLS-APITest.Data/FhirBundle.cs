using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NRLS_APITest.Data
{
    public class FhirBundle
    {
        public static Bundle GetBundle<T>(IList<T> resources) where T : Resource
        {
            var bundle = new Bundle
            {
                Id = "b54e3ad5-04b5-4f8f-8dbd-0e41d2465b5c",
                Meta = new Meta
                {
                    LastUpdated = new DateTimeOffset(new DateTime(2018, 3, 1, 0, 0, 0, DateTimeKind.Utc)),
                    VersionId = "b54e3ad5-04b5-4f8f-8dbd-0e41d2465b5c"
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
                    FullUrl = "ResourceLink",
                    Resource = r
                }).ToList()

            };

            return bundle;
        }
    }
}
