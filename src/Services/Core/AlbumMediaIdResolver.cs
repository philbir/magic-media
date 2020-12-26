using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Extensions;
using MagicMedia.Store;

namespace MagicMedia
{
    public class AlbumMediaIdResolver : IAlbumMediaIdResolver
    {
        private readonly IMediaStore _mediaStore;

        public AlbumMediaIdResolver(IMediaStore mediaStore)
        {
            _mediaStore = mediaStore;
        }

        public async Task<IEnumerable<Guid>> GetMediaIdsAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            Album? album = await _mediaStore.Albums.GetByIdAsync(id, cancellationToken);

            return await GetMediaIdsAsync(album, cancellationToken);
        }

        public async Task<IEnumerable<Guid>> GetMediaIdsAsync(
            Album album,
            CancellationToken cancellationToken)
        {
            HashSet<Guid> ids = new();

            foreach (AlbumInclude include in album.Includes)
            {
                switch (include.Type)
                {
                    case AlbumIncludeType.Ids:
                        ids.AddRange(include.MediaIds);
                        break;
                    case AlbumIncludeType.Folder:

                        foreach (var folder in include.Folders!)
                        {
                            IEnumerable<Guid> folderIds = await _mediaStore.GetIdsByFolderAsync(folder, cancellationToken);
                            ids.AddRange(folderIds);
                        }
                        break;
                    case AlbumIncludeType.Query:
                        IEnumerable<Guid> queryIds = await GetIdsFromFilter(include.Filters, cancellationToken);
                        ids.AddRange(queryIds);
                        break;
                }
            }

            return ids;
        }

        private async Task<IEnumerable<Guid>> GetIdsFromFilter(IEnumerable<FilterDescription> filters, CancellationToken cancellationToken)
        {
            SearchMediaRequest request = MapToSearchRequest(filters);

            return await _mediaStore.GetIdsFromSearchRequestAsync(request, cancellationToken);
        }

        private SearchMediaRequest MapToSearchRequest(IEnumerable<FilterDescription> filters)
        {
            var request = new SearchMediaRequest()
            {
                PageSize = 100000
            };

            foreach (FilterDescription? filter in filters)
            {
                switch (filter.Key.ToLower())
                {
                    case "folder":
                        request.Folder = filter.Value;
                        break;
                    case "date":
                        request.Date = filter.Value;
                        break;
                    case "persons":
                        request.Persons = filter.Value.Split(',').Select(x => Guid.Parse(x));
                        break;
                    case "groups":
                        request.Groups = filter.Value.Split(',').Select(x => Guid.Parse(x));
                        break;
                    case "cameras":
                        request.Cameras = filter.Value.Split(',').Select(x => Guid.Parse(x));
                        break;
                    case "countries":
                        request.Countries = filter.Value.Split(',');
                        break;
                    case "cities":
                        request.Cities = filter.Value.Split(',');
                        break;
                    case "albumId":
                        request.AlbumId = Guid.Parse(filter.Value);
                        break;
                    case "mediaTypes":
                        request.MediaTypes = filter.Value.Split(',').Select(x => Enum.Parse<MediaType>(x, true));
                        break;
                    case "geoRadius":
                        var parts = filter.Value.Split(':');
                        var coords = parts[1].Split(',');
                        request.GeoRadius = new GeoRadiusFilter
                        {
                            Distance = int.Parse(parts[0]),
                            Latitude = double.Parse(coords[0]),
                            Longitude = double.Parse(coords[1]),
                        };
                        break;
                    case "tags":
                        request.Tags = filter.Value.Split(',');
                        break;
                    case "objects":
                        request.Objects = filter.Value.Split(',');
                        break;
                    default:
                        throw new InvalidOperationException($"Invalid filter key: {filter.Key}");
                }
            }

            return request;
        }

    }
}
