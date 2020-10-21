using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GreenDonut;
using HotChocolate.DataLoader;
using MagicMedia.Store;
using MagicMedia.Thumbnail;

namespace MagicMedia.GraphQL.DataLoaders
{
    public class CameraByIdDataLoader : BatchDataLoader<Guid, Camera>
    {
        private readonly ICameraStore _cameraStore;

        public CameraByIdDataLoader(
            IBatchScheduler batchScheduler,
            ICameraStore cameraStore)
                : base(batchScheduler)
        {
            _cameraStore = cameraStore;
        }

        protected async override Task<IReadOnlyDictionary<Guid, Camera>> LoadBatchAsync(
            IReadOnlyList<Guid> keys,
            CancellationToken cancellationToken)
        {
            IEnumerable<Camera> cameras = await _cameraStore.GetManyAsync(keys, cancellationToken);

            return cameras.ToDictionary(x => x.Id);
        }
    }
}
