using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;
using MongoDB.Driver.Linq;


namespace MagicMedia.Store.MongoDb
{
    public class MediaFilterBuilder
    {
        private readonly MediaStoreContext _dbContext;
        private readonly Func<Guid, CancellationToken, Task<IEnumerable<Guid>>> _albumMediaResolver;
        private readonly CancellationToken _cancellationToken;
        private FilterDefinition<Media> _filter;
        private List<Task> _tasks;
        bool _showRecycled = false;

        public MediaFilterBuilder(
            MediaStoreContext dbContext,
            Func<Guid, CancellationToken, Task<IEnumerable<Guid>>> albumMediaResolver,
            CancellationToken cancellationToken)
        {
            _filter = Builders<Media>.Filter.Empty;
            _tasks = new();
            _dbContext = dbContext;
            _albumMediaResolver = albumMediaResolver;
            _cancellationToken = cancellationToken;
        }

        public MediaFilterBuilder AddFolder(string? folder)
        {
            if (!string.IsNullOrEmpty(folder))
            {
                if (folder.StartsWith("SPECIAL"))
                {
                    var special = folder.Split(':').LastOrDefault();

                    switch (special!.ToUpper())
                    {
                        case "FAVORITES":
                            _filter &= Builders<Media>.Filter.Eq(x => x.IsFavorite, true);
                            break;
                        case "RECYCLED":
                            _showRecycled = true;
                            break;
                        case "RECENTLY_ADDED":
                            _filter &= Builders<Media>.Filter
                                .Lt(x => x.Source.ImportedAt, DateTime.Now.AddDays(5));
                            break;
                    }
                }
                else
                {
                    _filter &= Builders<Media>.Filter.Regex(
                        x => x.Folder,
                        new BsonRegularExpression("^" + Regex.Escape(folder), "i"));
                }
            }

            return this;
        }

        internal MediaFilterBuilder AddDate(string? date)
        {
            if (!string.IsNullOrEmpty(date))
            {
                var dateParts = date.Split('-', StringSplitOptions.RemoveEmptyEntries);

                switch (dateParts.Length)
                {
                    case 1:
                        _filter &= BuildYearFilter(int.Parse(dateParts[0]));
                        break;
                    case 2:
                        _filter &= BuildMonthFilter(dateParts);
                        break;
                    case 3:
                        _filter &= BuildDayFilter(date);
                        break;
                }
            }

            return this;
        }

        private FilterDefinition<Media> BuildDayFilter(string date)
        {
            DateTime startDate = DateTime.Parse(date);
            DateTime endDate = startDate.AddDays(1);

            return Builders<Media>.Filter.And(
                Builders<Media>.Filter.Gte(x => x.DateTaken, startDate),
                Builders<Media>.Filter.Lt(x => x.DateTaken, endDate));
        }

        private FilterDefinition<Media> BuildMonthFilter(string[] dateParts)
        {
            DateTime startDate = new DateTime(int.Parse(dateParts[0]), int.Parse(dateParts[1]), 1);
            DateTime endDate = startDate.AddMonths(1);

            return Builders<Media>.Filter.And(
                Builders<Media>.Filter.Gte(x => x.DateTaken, startDate),
                Builders<Media>.Filter.Lt(x => x.DateTaken, endDate));
        }

        private FilterDefinition<Media> BuildYearFilter(int yaer)
        {
            return Builders<Media>.Filter.And(
                    Builders<Media>.Filter.Gte(x => x.DateTaken, new DateTime(yaer, 1, 1)),
                    Builders<Media>.Filter.Lt(x => x.DateTaken, new DateTime(yaer + 1, 1, 1)));
        }

        public MediaFilterBuilder AddPersons(IEnumerable<Guid>? persons)
        {
            if (persons is { } p && p.Any())
            {
                _tasks.Add(CreatePersonFilter(p));
            }

            return this;
        }

        public MediaFilterBuilder AddCities(IEnumerable<string>? cities)
        {
            if (cities is { } c && c.Any())
            {
                _filter &= Builders<Media>.Filter.In(
                    x => x.GeoLocation.Address.City,
                    c);
            }

            return this;
        }

        public MediaFilterBuilder AddMediaTypes(IEnumerable<MediaType>? mediaTypes)
        {
            if (mediaTypes is { } types && types.Any() && types.Count() < 2)
            {
                _filter &= Builders<Media>.Filter.In(
                    x => x.MediaType, types);
            }

            return this;
        }

        public MediaFilterBuilder AddCountries(IEnumerable<string>? countries)
        {
            if (countries is { } c && c.Any())
            {
                _filter &= Builders<Media>.Filter.In(
                    x => x.GeoLocation.Address.CountryCode,
                    c);
            }

            return this;
        }

        public MediaFilterBuilder AddAlbum(Guid? albumId)
        {
            if (albumId.HasValue)
            {
                _tasks.Add(CreateAlbumFilter(albumId.Value));
            }

            return this;
        }

        public MediaFilterBuilder AddGeoRadius(GeoRadiusFilter? geoRadius)
        {
            if (geoRadius != null)
            {
                GeoJsonPoint<GeoJson2DGeographicCoordinates>? point = GeoJson.Point(
                    GeoJson.Geographic(
                        geoRadius.Longitude,
                        geoRadius.Latitude));

                _filter &= Builders<Media>.Filter.NearSphere(x =>
                       x.GeoLocation.Point,
                        point,
                        maxDistance: geoRadius.Distance * 1000
                        );
            }

            return this;
        }

        private async Task CreateAlbumFilter(Guid id)
        {
            IEnumerable<Guid>? mediaIds = await _albumMediaResolver(id, _cancellationToken);

            if (mediaIds.Any())
            {
                _filter &= Builders<Media>.Filter.In(x => x.Id, mediaIds);
            }
        }

        private async Task CreatePersonFilter(
            IEnumerable<Guid> persons)
        {
            IEnumerable<Guid> mediaIds = await GetMediaIdsByPersons(
                persons,
                _cancellationToken);

            if (mediaIds.Any())
            {
                _filter &= Builders<Media>.Filter.In(x => x.Id, mediaIds);
            }
        }


        private async Task<IEnumerable<Guid>> GetMediaIdsByPersons(
            IEnumerable<Guid> persons,
            CancellationToken cancellationToken)
        {
            List<Guid> ids = await _dbContext.Faces.AsQueryable()
                .Where(x => x.PersonId.HasValue)
                .Where(x => persons.ToList().Contains(x.PersonId.Value))
                .Select(x => x.MediaId)
                .ToListAsync(cancellationToken);

            return ids;
        }

        public async Task<FilterDefinition<Media>> BuildAsync()
        {
            FilterDefinition<Media> recycledFilter = Builders<Media>.Filter
                .Eq(x => x.State, MediaState.Recycled);

            if (_showRecycled)
            {
                _filter &= recycledFilter;
            }
            else
            {
                _filter &= Builders<Media>.Filter.Not(recycledFilter);
            }

            await Task.WhenAll(_tasks);

            return _filter;
        }
    }
}
