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
        private readonly IUserAuthorizationService _userAuthorizationService;

        public SearchFacetService(
            IMediaStore mediaStore,
            IUserAuthorizationService userAuthorizationService)
        {
            _mediaStore = mediaStore;
            _userAuthorizationService = userAuthorizationService;
        }

        public async Task<IEnumerable<SearchFacetItem>> GetCountryFacetsAsync(
            CancellationToken cancellationToken)
        {
            UserResourceAccessInfo accessInfo = await _userAuthorizationService.GetAuthorizedOnAsync(
                ProtectedResourceType.Media,
                cancellationToken);

            return await _mediaStore.GetGroupedCountriesAsync(accessInfo.Ids, cancellationToken);
        }

        public async Task<IEnumerable<SearchFacetItem>> GetCityFacetsAsync(
            CancellationToken cancellationToken)
        {
            UserResourceAccessInfo accessInfo = await _userAuthorizationService.GetAuthorizedOnAsync(
                ProtectedResourceType.Media,
                cancellationToken);

            return await _mediaStore.GetGroupedCitiesAsync(accessInfo.Ids, cancellationToken);
        }

        public async Task<IEnumerable<SearchFacetItem>> GetAITagFacetsAsync(
            CancellationToken cancellationToken)
        {
            UserResourceAccessInfo accessInfo = await _userAuthorizationService.GetAuthorizedOnAsync(
                ProtectedResourceType.Media,
                cancellationToken);

            return await _mediaStore.MediaAI.GetGroupedAITagsAsync(accessInfo.Ids, cancellationToken);
        }

        public async Task<IEnumerable<SearchFacetItem>> GetAIObjectsFacetsAsync(
            CancellationToken cancellationToken)
        {
            UserResourceAccessInfo accessInfo = await _userAuthorizationService.GetAuthorizedOnAsync(
                ProtectedResourceType.Media,
                cancellationToken);

            return await _mediaStore.MediaAI.GetGroupedAIObjectsAsync(accessInfo.Ids, cancellationToken);
        }
    }
}
