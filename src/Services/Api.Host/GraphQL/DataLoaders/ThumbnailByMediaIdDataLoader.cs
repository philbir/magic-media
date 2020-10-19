using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GreenDonut;
using HotChocolate.DataLoader;
using MagicMedia.Store;

namespace MagicMedia.Api.GraphQL.DataLoaders
{
    public class ThumbnailByMediaIdDataLoader : BatchDataLoader<Tuple<Guid, ThumbnailSizeName>, MediaThumbnail>
    {
        private readonly IMediaStore _mediaStore;

        public ThumbnailByMediaIdDataLoader(
            IBatchScheduler batchScheduler,
            IMediaStore mediaStore)
             : base(batchScheduler)
        {
            _mediaStore = mediaStore;
        }

        protected async override Task<IReadOnlyDictionary<Tuple<Guid, ThumbnailSizeName>, MediaThumbnail>> LoadBatchAsync(
            IReadOnlyList<Tuple<Guid, ThumbnailSizeName>> keys,
            CancellationToken cancellationToken)
        {
            ThumbnailSizeName size = keys.First().Item2;

            IReadOnlyDictionary<Guid, MediaThumbnail> thumbs = await _mediaStore
                .GetThumbnailsByMediaIdsAsync(
                    keys.Select( x => x.Item1),
                    size,
                    cancellationToken);

            return thumbs.ToDictionary(k => new Tuple<Guid, ThumbnailSizeName>(k.Key, size), v => v.Value);
        }
    }
}
