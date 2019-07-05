using MongoDB.Driver;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NRLS_APITest.StubClasses
{
    public static class MongoStubs
    {

        public static IAsyncCursor<T> AsyncCursor<T>(IEnumerable<T> entities)
        {
            var mockCursor = new Mock<IAsyncCursor<T>>();
            mockCursor.Setup(_ => _.Current).Returns(entities);
            mockCursor
                .SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);
            mockCursor
                .SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true))
                .Returns(Task.FromResult(false));

            return mockCursor.Object;
        }
    }
}
