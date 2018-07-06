using Hl7.Fhir.Model;
using Hl7.Fhir.Specification.Source;
using Moq;

namespace NRLS_APITest.StubClasses
{
    public class ResourceResolverStub
    {
        public static class MockResourceResolver
        {
            public static IResourceResolver GetSource()
            {
                var mockMemoryCache = new Mock<IResourceResolver>();
                mockMemoryCache.Setup(x => x.ResolveByUri(It.IsAny<string>())).Returns((Resource)null);
                mockMemoryCache.Setup(x => x.ResolveByCanonicalUri(It.IsAny<string>())).Returns((Resource)null);

                return mockMemoryCache.Object;
            }
        }
    }
}
