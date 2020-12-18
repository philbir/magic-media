using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Search;
using MagicMedia.Security;
using MagicMedia.Store;

namespace MagicMedia
{
    public class SearchFacetService : ISearchFacetService
    {
        private readonly IMediaStore _mediaStore;
        private readonly IUserContextFactory _userContextFactory;

        public SearchFacetService(
            IMediaStore mediaStore,
            IUserContextFactory userContextFactory)
        {
            _mediaStore = mediaStore;
            _userContextFactory = userContextFactory;
        }

        public async Task<IEnumerable<SearchFacetItem>> GetCountryFacetsAsync(
            CancellationToken cancellationToken)
        {
            IEnumerable<Guid>? ids = await GetAuthorizedOnMediaAync(cancellationToken);

            return await _mediaStore.GetGroupedCountriesAsync(ids, cancellationToken);
        }

        public async Task<IEnumerable<SearchFacetItem>> GetCityFacetsAsync(
            CancellationToken cancellationToken)
        {
            IEnumerable<Guid>? ids = await GetAuthorizedOnMediaAync(cancellationToken);

            return await _mediaStore.GetGroupedCitiesAsync(ids, cancellationToken);
        }

        private async Task<IEnumerable<Guid>?> GetAuthorizedOnMediaAync(CancellationToken cancellationToken)
        {
            IUserContext userContext = await _userContextFactory.CreateAsync(cancellationToken);

            IEnumerable<Guid>? ids = null;

            if (!userContext.HasPermission(Permissions.Media.ViewAll))
            {
                ids = await userContext.GetAuthorizedMediaAsync(cancellationToken);
            }

            return ids;
        }

        public async Task<IEnumerable<SearchFacetItem>> GetAITagFacetsAsync(
            CancellationToken cancellationToken)
        {
            IEnumerable<Guid>? ids = await GetAuthorizedOnMediaAync(cancellationToken);

            return await _mediaStore.MediaAI.GetGroupedAITagsAsync(ids, cancellationToken);
        }

        public async Task<IEnumerable<SearchFacetItem>> GetAIObjectsFacetsAsync(
            CancellationToken cancellationToken)
        {
            IEnumerable<Guid>? ids = await GetAuthorizedOnMediaAync(cancellationToken);

            return await _mediaStore.MediaAI.GetGroupedAIObjectsAsync(ids, cancellationToken);
        }
    }
}
