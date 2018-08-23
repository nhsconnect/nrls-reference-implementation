using MongoDB.Bson;
using MongoDB.Driver;

namespace NRLS_APITest.TestModels
{
    public class MockDUpdateResult : UpdateResult
    {
        public MockDUpdateResult() { }

        public MockDUpdateResult(long updatedCount, bool isAcknowledged)
        {
            _modifiedCount = updatedCount;
            _isAcknowledged = isAcknowledged;
        }

        public override long ModifiedCount => _modifiedCount;

        public override bool IsAcknowledged => _isAcknowledged;

        public override bool IsModifiedCountAvailable => throw new System.NotImplementedException();

        public override long MatchedCount => throw new System.NotImplementedException();

        public override BsonValue UpsertedId => throw new System.NotImplementedException();

        private long _modifiedCount { get; set; }

        private bool _isAcknowledged { get; set; }
    }
}
