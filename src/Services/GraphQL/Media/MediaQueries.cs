using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Types;
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

        public async Task<IEnumerable<Media>> SearchMediaAsync(
            SearchMediaRequest request,
            CancellationToken cancellationToken)
        {
            return await _mediaStore.SearchAsync(request, cancellationToken);
        }

        public async Task<Media> GetMediaByIdAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            return await _mediaStore.GetById(id, cancellationToken);
        }
    }
}
