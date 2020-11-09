using System;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Operations;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MagicMedia.Store.MongoDb
{
    public class OperationStore : IOperationStore
    {
        private readonly MediaStoreContext _mediaStoreContext;

        public OperationStore(MediaStoreContext mediaStoreContext)
        {
            _mediaStoreContext = mediaStoreContext;
        }

        public async Task AddAsync(MediaOperation operation, CancellationToken cancellationToken)
        {
            await _mediaStoreContext.Operations.InsertOneAsync(
                operation,
                options: null,
                cancellationToken);
        }

        public async Task UpdateAsync(
            MediaOperation operation,
            CancellationToken cancellationToken)
        {
            await _mediaStoreContext.Operations.ReplaceOneAsync(
                x => x.Id == operation.Id,
                operation,
                options: new ReplaceOptions(),
                cancellationToken);
        }

        public async Task<MediaOperation> GetAsync(
            Guid operationId,
            CancellationToken cancellationToken)
        {
            return await _mediaStoreContext.Operations.AsQueryable()
                .Where(x => x.Id == operationId)
                .SingleAsync(cancellationToken);
        }

        public async Task UpdateTaskAsync(
            Guid operationId,
            MediaOperationTask task,
            CancellationToken cancellationToken)
        {
            FilterDefinition<MediaOperation> filter = Builders<MediaOperation>.Filter.Eq(x => x.Id, operationId)
                & Builders<MediaOperation>.Filter.ElemMatch(
                    x => x.Tasks,
                    Builders<MediaOperationTask>.Filter.Eq(t => t.Id, task.Id));

            UpdateDefinition<MediaOperation>? update = Builders<MediaOperation>.Update.Set("Tasks.$", task);

            await _mediaStoreContext.Operations.UpdateOneAsync(
                filter,
                update,
                options: null,
                cancellationToken);
        }
    }
}
