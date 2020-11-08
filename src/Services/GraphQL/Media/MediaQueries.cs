using System;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Types;
using MagicMedia.Search;
using MagicMedia.Store;

namespace MagicMedia.GraphQL
{
    [ExtendObjectType(Name = "Query")]
    public class MediaQueries
    {
        private readonly IMediaStore _mediaStore;

        public MediaQueries(IMediaStore mediaStore)
        {
            _mediaStore = mediaStore;
        }

        public async Task<SearchResult<Media>> SearchMediaAsync(
            SearchMediaRequest request,
            CancellationToken cancellationToken)
        {
            return await _mediaStore.SearchAsync(request, cancellationToken);
        }

        public async Task<Media> GetMediaByIdAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            return await _mediaStore.GetByIdAsync(id, cancellationToken);
        }
    }
}
