﻿using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace NRLS_APITest.StubClasses
{
    public class MemoryCacheStub
    {
        public static class MockMemoryCacheService
        {
            public static IMemoryCache GetMemoryCache(object expectedValue)
            {
                var mockMemoryCache = new Mock<IMemoryCache>();
                mockMemoryCache
                    .Setup(x => x.TryGetValue(It.IsAny<object>(), out expectedValue))
                    .Returns(true);
                return mockMemoryCache.Object;
            }
        }
    }
}
