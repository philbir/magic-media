using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Store;
using Serilog;

namespace MagicMedia;

public class AlbumSummaryService : IAlbumSummaryService
{
    private readonly IMediaStore _mediaStore;
    private readonly IAlbumService _albumService;
    private readonly IAlbumMediaIdResolver _albumMediaIdResolver;

    public AlbumSummaryService(
        IMediaStore mediaStore,
        IAlbumService albumService,
        IAlbumMediaIdResolver albumMediaIdResolver)
    {
        _mediaStore = mediaStore;
        _albumService = albumService;
        _albumMediaIdResolver = albumMediaIdResolver;
    }

    public async Task<Album> UpdateAsync(Guid id, CancellationToken cancellationToken)
    {
        Album album = await _mediaStore.Albums.GetByIdAsync(id, cancellationToken);

        return await UpdateAsync(album, cancellationToken);
    }

    public async Task<Album> UpdateAsync(Album album, CancellationToken cancellationToken)
    {
        //Log.Information("Updating album summary. {Id}", album.Id);

        album = await BuildAsync(album, cancellationToken);
        await _mediaStore.Albums.UpdateAsync(album, cancellationToken);

        return album;
    }


    public async Task UpdateAllAsync(CancellationToken cancellationToken)
    {
        IEnumerable<Album> all = await _albumService.GetAllAsync(cancellationToken);

        foreach (Album? album in all)
        {
            try
            {
                await UpdateAsync(album, cancellationToken);
            }
            catch (Exception ex)
            {
                Log.Error(ex,"Error updating album summary for: {Id}-{Name}", album.Id, album.Title);
            }
        }
    }

    public async Task<Album> BuildAsync(
        Album album,
        CancellationToken cancellationToken)
    {
        IEnumerable<Guid> mediaIds = await _albumMediaIdResolver.GetMediaIdsAsync(
            album,
            cancellationToken);

        if (!mediaIds.Any())
        {
            return album;
        }

        AlbumData data = await GetAlbumDataAsync(mediaIds, cancellationToken);
        album.Persons = GetPersons(data);
        album.Countries = GetCountries(data);

        album.ImageCount = data.Medias.Count(x => x.MediaType == MediaType.Image);
        album.VideoCount = data.Medias.Count(x => x.MediaType == MediaType.Video);
        album.StartDate = data.Medias.Min(x => x.DateTaken);
        album.EndDate = data.Medias.Max(x => x.DateTaken);
        ;
        if (album.CoverMediaId == null)
        {
            album.CoverMediaId = mediaIds.ToArray()[mediaIds.Count() / 2];
        }

        return album;
    }

    private IEnumerable<AlbumCountry> GetCountries(AlbumData data)
    {
        IEnumerable<IGrouping<string, Media>> groupedCountry = data.Medias
            .Where(x => x.GeoLocation?.Address?.Country != null)
            .GroupBy(x => x.GeoLocation!.Address.Country);

        var albumCountries = new List<AlbumCountry>();

        foreach (IGrouping<string, Media>? country in groupedCountry.Where(
            x => x.Key is { }))
        {
            var ab = new AlbumCountry()
            {
                Name = country.Key,
                Code = country.FirstOrDefault()?.GeoLocation?.Address.CountryCode,
                Count = country.Count(),
                Cities = GetCities(country.ToList()).OrderByDescending(x => x.Count)
            };
            albumCountries.Add(ab);
        }

        return albumCountries.OrderByDescending(x => x.Count);
    }

    private IEnumerable<AlbumPerson> GetPersons(AlbumData data)
    {
        ILookup<Guid, Person>? personLookup = data.Persons.ToLookup(x => x.Id);

        IEnumerable<IGrouping<Guid, MediaFace>>? groupedFaces = data.Faces
            .Where(x => x.PersonId.HasValue)
            .GroupBy(x => x.PersonId!.Value);

        var albumPersons = new List<AlbumPerson>();

        foreach (IGrouping<Guid, MediaFace>? group in groupedFaces)
        {
            Person? person = personLookup[group.Key].FirstOrDefault();

            if (person != null)
            {
                var ap = new AlbumPerson
                {
                    PersonId = person.Id,
                    Name = person.Name,
                    Count = group.Count()
                };

                ap.FaceId = group.Where(x => x.Thumbnail != null)
                                 .OrderByDescending(x => x.Thumbnail.Dimensions.Width)
                                 .First().Id;

                albumPersons.Add(ap);
            }
        }

        return albumPersons.OrderByDescending(x => x.Count);
    }

    private IEnumerable<AlbumCity> GetCities(List<Media> medias)
    {
        IEnumerable<IGrouping<string, Media>>? grouped = medias
            .GroupBy(x => x.GeoLocation!.Address.City);

        foreach (IGrouping<string, Media>? group in grouped.Where(x => x.Key is { }))
        {
            yield return new AlbumCity
            {
                Name = group.Key,
                Count = group.Count()
            };
        }
    }

    private async Task<AlbumData> GetAlbumDataAsync(
        IEnumerable<Guid> mediaIds,
        CancellationToken cancellationToken)
    {
        IEnumerable<Media> medias = await _mediaStore.GetManyAsync(
            mediaIds,
            cancellationToken);

        IEnumerable<MediaFace> faces = await _mediaStore.Faces.GetManyByMediaAsync(
            mediaIds,
            cancellationToken);

        Guid[] personIds = faces
            .Where(x => x.PersonId.HasValue)
            .Select(x => x.PersonId!.Value)
            .Distinct()
            .ToArray();

        IEnumerable<Person>? persons = await _mediaStore.Persons.GetPersonsAsync(
            personIds,
            cancellationToken);

        return new AlbumData(medias, persons, faces);
    }
}
