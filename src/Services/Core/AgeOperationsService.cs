using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Store;
using NodaTime;

namespace MagicMedia;

public class AgeOperationsService : IAgeOperationsService
{
    private readonly IMediaStore _mediaStore;

    public AgeOperationsService(
        IMediaStore mediaStore)
    {
        _mediaStore = mediaStore;
    }

    public async Task UpdateAgesByPersonAsync(
        Guid personId,
        CancellationToken cancellationToken)
    {
        Person person = await _mediaStore.Persons.GetByIdAsync(personId, cancellationToken);

        await UpdateAgesByPersonAsync(person, cancellationToken);
    }

    public async Task UpdateAgesByPersonAsync(
        Person person,
        CancellationToken cancellationToken)
    {
        if (person.DateOfBirth == null)
        {
            return;
        }

        IEnumerable<MediaFace> faces = await _mediaStore.Faces.GetFacesByPersonAsync(
            person.Id,
            null,
            cancellationToken);

        IEnumerable<MediaHeaderData>? medias = await _mediaStore.GetHeaderDataAsync(
            faces.Select(x => x.MediaId),
            cancellationToken);

        ILookup<Guid, MediaHeaderData>? mediaLookup = medias.ToLookup(x => x.Id);

        List<UpdateAgeRequest> updates = new();

        foreach (MediaFace? face in faces)
        {
            MediaHeaderData? media = mediaLookup[face.MediaId].Single();
            var age = CalculateAge(media.DateTaken, person.DateOfBirth.Value);
            updates.Add(new UpdateAgeRequest(face.Id, age));
        }

        await _mediaStore.Faces.BulkUpdateAgesAsync(updates, cancellationToken);
    }

    public async Task UpdateAgesByMediaAsync(Media media, CancellationToken cancellationToken)
    {
        IEnumerable<MediaFace> faces = await _mediaStore.Faces.GetFacesByMediaAsync(media.Id, cancellationToken);
        IEnumerable<MediaFace>? withPerson = faces.Where(x => x.PersonId.HasValue);

        if (withPerson.Any())
        {
            IEnumerable<Person> persons = await _mediaStore.Persons.GetPersonsAsync(withPerson.Select(x => x.PersonId!.Value), cancellationToken);
            List<UpdateAgeRequest> updates = new();

            foreach (MediaFace face in faces)
            {
                Person? person = persons.FirstOrDefault(x => x.Id == face.PersonId!.Value && x.DateOfBirth.HasValue);

                if (person != null)
                {
                    var age = CalculateAge(media.DateTaken, person.DateOfBirth!.Value);
                    updates.Add(new UpdateAgeRequest(face.Id, age));
                }
            }

            if (updates.Count > 0)
            {
                await _mediaStore.Faces.BulkUpdateAgesAsync(updates, cancellationToken);
            }
        }
    }

    public int? CalculateAge(DateTimeOffset? dateTaken, DateTime dateOfBirth)
    {
        if (dateTaken.HasValue)
        {
            var taken = LocalDate.FromDateTime(dateTaken.Value.Date);
            var birth = LocalDate.FromDateTime(dateOfBirth);
            Period period = Period.Between(taken, birth, PeriodUnits.Months);

            return Math.Abs(period.Months);
        }

        return null;
    }
}
