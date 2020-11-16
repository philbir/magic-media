using System;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Types;
using MagicMedia.Search;
using MagicMedia.Store;

namespace MagicMedia.GraphQL
{
    [ExtendObjectType(Name = "Query")]
    public class MediaQueries
    {
        private readonly IMediaStore _mediaStore;
        private readonly IFolderTreeService _folderTreeService;
        private readonly IMediaSearchService _mediaSearchService;

        public MediaQueries(
            IMediaStore mediaStore,
            IFolderTreeService folderTreeService,
            IMediaSearchService mediaSearchService)
        {
            _mediaStore = mediaStore;
            _folderTreeService = folderTreeService;
            _mediaSearchService = mediaSearchService;
        }

        public async Task<SearchResult<Media>> SearchMediaAsync(
            SearchMediaRequest request,
            CancellationToken cancellationToken)
        {
            return await _mediaSearchService.SearchAsync(request, cancellationToken);
        }

        public async Task<Media> GetMediaByIdAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            return await _mediaStore.GetByIdAsync(id, cancellationToken);
        }

        public async Task<FolderItem> GetFolderTreeAsync(CancellationToken cancellationToken)
        {
            return await _folderTreeService.GetTreeAsync(cancellationToken);
        }
    }
}
