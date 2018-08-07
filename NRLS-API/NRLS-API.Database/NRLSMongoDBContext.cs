using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using NRLS_API.Core.Interfaces.Database;
using NRLS_API.Models;

namespace NRLS_API.Database
{
    public class NRLSMongoDBContext : INRLSMongoDBContext
    {
        private readonly IMongoDatabase _database = null;
        private readonly IMongoClient _client = null;

        public NRLSMongoDBContext(IOptions<DbSetting> settings)
        {
            _client = new MongoClient(settings.Value.ConnectionString);

            if (_client != null)
            {
                _database = _client.GetDatabase(settings.Value.Database);
            }
        }

        public IMongoCollection<BsonDocument> Resource(string resourceType)
        {
            return _database.GetCollection<BsonDocument>(resourceType);
        }

        // Below requires MongoDB v4+ AND C# Mongo Drivers 2.7+

        //In a replication world we could use transactions to perform roll backs
        //public async Task<T> StartTransactionWithRetry<T>(Func<IClientSessionHandle, Action<IClientSessionHandle>, Task<T>> transactionTasks)
        //{
        //    // start a session
        //    using (var session = _client.StartSession())
        //    {
        //        try
        //        {
        //            return await RunTransactionWithRetry<T>(transactionTasks, session);
        //        }
        //        catch (Exception ex)
        //        {
        //            throw new HttpFhirException("Data Update Transaction Error", OperationOutcomeFactory.CreateInternalError(ex.Message));
        //        }
        //    }
        //}

        //private void CommitTransactionWithRetry(IClientSessionHandle session)
        //{
        //    while (true)
        //    {
        //        try
        //        {
        //            session.CommitTransaction();
        //            break;
        //        }
        //        catch (MongoException ex)
        //        {
        //            // can retry commit
        //            if (ex.HasErrorLabel("UnknownTransactionCommitResult"))
        //            {
        //                continue;
        //            }
        //            else
        //            {
        //                throw new HttpFhirException("Data Update Transaction Error", OperationOutcomeFactory.CreateInternalError(ex.Message));
        //            }
        //        }
        //    }
        //}

        //private async Task<T> RunTransactionWithRetry<T>(Func<IClientSessionHandle, Action<IClientSessionHandle>, Task<T>> txnFunc, IClientSessionHandle session)
        //{
        //    while (true)
        //    {
        //        try
        //        {
        //            return await txnFunc(session, this.CommitTransactionWithRetry); // performs transaction
        //        }
        //        catch (MongoException ex)
        //        {
        //            // if transient error, retry the whole transaction
        //            if (ex.HasErrorLabel("TransientTransactionError"))
        //            {
        //                continue;
        //            }
        //            else
        //            {
        //                throw new HttpFhirException("Data Update Transaction Error", OperationOutcomeFactory.CreateInternalError(ex.Message));
        //            }
        //        }
        //    }
        //}

    }
}
