using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Search;
using MagicMedia.Store;

namespace MagicMedia
{
    public class MediaSearchService : IMediaSearchService
    {
        private readonly IMediaStore _mediaStore;
        private readonly IAlbumService _albumService;

        public MediaSearchService(IMediaStore mediaStore, IAlbumService albumService)
        {
            _mediaStore = mediaStore;
            _albumService = albumService;
        }

        public async Task<SearchResult<Media>> SearchAsync(
                SearchMediaRequest request,
                CancellationToken cancellationToken)
        {
            return await _mediaStore.SearchAsync(
                request,
                _albumService.GetMediaIdsAsync,
                cancellationToken);
        }
    }
}
