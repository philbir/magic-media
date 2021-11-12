using GreenDonut;
using MagicMedia.Store;

namespace MagicMedia.GraphQL.DataLoaders
{
    public class ThumbnailDataDataLoader : BatchDataLoader<MediaThumbnail, MediaThumbnail>
    {
        private readonly IThumbnailBlobStore _thumbnailBlobStore;

        public ThumbnailDataDataLoader(
            IBatchScheduler batchScheduler,
            IThumbnailBlobStore thumbnailBlobStore)
                : base(batchScheduler)
        {
            _thumbnailBlobStore = thumbnailBlobStore;
        }

        protected async override Task<IReadOnlyDictionary<MediaThumbnail, MediaThumbnail>> LoadBatchAsync(
            IReadOnlyList<MediaThumbnail> keys,
            CancellationToken cancellationToken)
        {
            Dictionary<MediaThumbnail, MediaThumbnail> result = new();

            var tasks = new List<Task>();

            foreach (MediaThumbnail thumb in keys)
            {
                tasks.Add(LoadThumbnailData(thumb, cancellationToken));
            }

            await Task.WhenAll(tasks);

            return keys.ToDictionary(x => x, y => y);
        }

        private async Task LoadThumbnailData(MediaThumbnail thumbnail, CancellationToken cancellationToken)
        {
            thumbnail.Data = await _thumbnailBlobStore.GetAsync(thumbnail.Id, cancellationToken);
        }

    }



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
                    keys.Select(x => x.Item1),
                    size,
                    cancellationToken);

            return thumbs.ToDictionary(k => new Tuple<Guid, ThumbnailSizeName>(k.Key, size), v => v.Value);
        }
    }
}
