using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Search;
using MagicMedia.Security;
using MagicMedia.Store;
using NGeoHash;

namespace MagicMedia
{
    public class MediaSearchService : IMediaSearchService
    {
        private readonly IMediaStore _mediaStore;
        private readonly IAlbumService _albumService;
        private readonly IAlbumMediaIdResolver _albumMediaIdResolver;
        private readonly IUserContextFactory _userContextFactory;

        public MediaSearchService(
            IMediaStore mediaStore,
            IAlbumService albumService,
            IAlbumMediaIdResolver albumMediaIdResolver,
            IUserContextFactory userContextFactory)
        {
            _mediaStore = mediaStore;
            _albumService = albumService;
            _albumMediaIdResolver = albumMediaIdResolver;
            _userContextFactory = userContextFactory;
        }

        public async Task<SearchResult<Media>> SearchAsync(
                SearchMediaRequest request,
                CancellationToken cancellationToken)
        {
            IUserContext userContext = await _userContextFactory.CreateAsync(cancellationToken);

            if (!userContext.HasPermission(Permissions.Media.ViewAll))
            {
                request.AuthorizedOnMedia = await userContext.GetAuthorizedMediaAsync(cancellationToken);
            }

            return await _mediaStore.SearchAsync(
                request,
                _albumMediaIdResolver.GetMediaIdsAsync,
                cancellationToken);
        }

        public async Task<IEnumerable<GeoClusterLocation>> GetGeoLocationClustersAsync(
            GetGeoLocationClustersRequest request,
            CancellationToken cancellationToken)
        {
            IEnumerable<MediaGeoLocation> medias = await _mediaStore.FindMediaInGeoBoxAsync(
                request.Box,
                100000,
                cancellationToken);

            var clusters = new List<GeoClusterLocation>();

            if (medias.Count() < 500 && request.Precision > 10)
            {
                //Add every item as cluster
                foreach (MediaGeoLocation media in medias)
                {
                    clusters.Add(new GeoClusterLocation()
                    {
                        Hash = GeoHash.Encode(
                            media.Coordinates.Latitude,
                            media.Coordinates.Longitude),

                        Coordinates = media.Coordinates,
                        Id = media.Id,
                        Count = 1
                    });
                }
            }
            else
            {
                medias.ToList().ForEach(x => x.GeoHash = x.GeoHash.Substring(0, request.Precision));
                IEnumerable<IGrouping<string, MediaGeoLocation>> grouped = medias
                    .GroupBy(x => x.GeoHash);

                foreach (IGrouping<string, MediaGeoLocation>? group in grouped)
                {
                    GeohashDecodeResult decoded = GeoHash.Decode(group.Key);
                    var cluster = new GeoClusterLocation
                    {
                        Hash = group.Key,
                        Count = group.Count(),
                        Coordinates = new GeoCoordinate
                        {
                            Latitude = decoded.Coordinates.Lat,
                            Longitude = decoded.Coordinates.Lon
                        }
                    };
                    if (group.Count() == 1)
                    {
                        cluster.Id = group.First().Id;
                    }
                    clusters.Add(cluster);
                }
            }

            return clusters;
        }
    }
}
