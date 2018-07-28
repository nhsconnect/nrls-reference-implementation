using MongoDB.Driver;

namespace NRLS_APITest.TestModels
{
    public class MockDeleteResult : DeleteResult
    {
        public MockDeleteResult() { }

        public MockDeleteResult(long deletedCount, bool isAcknowledged)
        {
            _deleteCount = deletedCount;
            _isAcknowledged = isAcknowledged;
        }

        public override long DeletedCount => _deleteCount;

        public override bool IsAcknowledged => _isAcknowledged;

        private long _deleteCount { get; set; }

        private bool _isAcknowledged { get; set; }
    }
}
