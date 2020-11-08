using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GreenDonut;
using HotChocolate.DataLoader;
using MagicMedia.Store;

namespace MagicMedia.GraphQL.DataLoaders
{
    public class MediaByIdDataLoader : BatchDataLoader<Guid, Media>
    {
        private readonly IMediaStore _mediaStore;

        public MediaByIdDataLoader(
            IBatchScheduler batchScheduler,
            IMediaStore mediaStore) :
            base(batchScheduler)
        {
            _mediaStore = mediaStore;
        }

        protected async override Task<IReadOnlyDictionary<Guid, Media>> LoadBatchAsync(
            IReadOnlyList<Guid> keys,
            CancellationToken cancellationToken)
        {
            IEnumerable<Media>? medias = await _mediaStore.GetManyAsync(keys, cancellationToken);

            return medias.ToDictionary(x => x.Id);
        }
    }
}
