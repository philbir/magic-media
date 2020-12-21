using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Types;
using MagicMedia.Audit;
using MagicMedia.Authorization;
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
        private readonly IAuditService _auditService;

        public MediaQueries(
            IMediaStore mediaStore,
            IFolderTreeService folderTreeService,
            IMediaSearchService mediaSearchService,
            IAuditService auditService)
        {
            _mediaStore = mediaStore;
            _folderTreeService = folderTreeService;
            _mediaSearchService = mediaSearchService;
            _auditService = auditService;
        }

        public async Task<SearchResult<Media>> SearchMediaAsync(
            SearchMediaRequest request,
            CancellationToken cancellationToken)
        {
            await AuditSearch(request, cancellationToken);

            return await _mediaSearchService.SearchAsync(request, cancellationToken);
        }

        [Authorize(Apply = ApplyPolicy.BeforeResolver, Policy = AuthorizationPolicies.Names.MediaView)]
        public async Task<Media> GetMediaByIdAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            return await _mediaStore.GetByIdAsync(id, cancellationToken);
        }

        public async Task<IEnumerable<GeoClusterLocation>> GetGeoLocationClustersAsync(
            GetGeoLocationClustersRequest input,
            CancellationToken cancellationToken)
        {
            return await _mediaSearchService.GetGeoLocationClustersAsync(input, cancellationToken);
        }

        public async Task<FolderItem> GetFolderTreeAsync(CancellationToken cancellationToken)
        {
            return await _folderTreeService.GetTreeAsync(cancellationToken);
        }

        private async Task AuditSearch(SearchMediaRequest request, CancellationToken cancellationToken)
        {
            var auditRequest = new LogAuditEventRequest
            {
                Action = "Search",
                Success = true,
                Resource = new AuditResource
                {
                    Type = ProtectedResourceType.Media,
                }
            };

            await _auditService.LogEventAsync(auditRequest, cancellationToken);
        }
    }
}
