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
    public class ThumbnailByMediaIdDataLoader : BatchDataLoader<Guid, MediaThumbnail>
    {
        private readonly IMediaStore _mediaStore;

        public ThumbnailByMediaIdDataLoader(
            IBatchScheduler batchScheduler,
            IMediaStore mediaStore)
             : base(batchScheduler)
        {
            _mediaStore = mediaStore;
        }

        public ThumbnailSizeName Size { get; set; }

        protected async override Task<IReadOnlyDictionary<Guid, MediaThumbnail>> LoadBatchAsync(
            IReadOnlyList<Guid> keys,
            CancellationToken cancellationToken)
        {
            IReadOnlyDictionary<Guid, MediaThumbnail> thumbs = await _mediaStore
                .GetThumbnailsByMediaIdsAsync(
                    keys,
                    Size,
                    cancellationToken);

            return thumbs;
        }
    }
}
