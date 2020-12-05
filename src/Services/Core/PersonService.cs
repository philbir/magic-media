using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Messaging;
using MagicMedia.Search;
using MagicMedia.Store;
using MassTransit;
using Serilog;
using SixLabors.ImageSharp.ColorSpaces;

namespace MagicMedia
{
    public class PersonService : IPersonService
    {
        private readonly IPersonStore _personStore;
        private readonly IGroupService _groupService;
        private readonly IFaceStore _faceStore;
        private readonly IThumbnailBlobStore _thumbnailBlob;
        private readonly IBus _bus;

        public PersonService(
            IPersonStore personStore,
            IGroupService groupService,
            IFaceStore faceStore,
            IThumbnailBlobStore thumbnailBlob,
            IBus bus)
        {
            _personStore = personStore;
            _groupService = groupService;
            _faceStore = faceStore;
            _thumbnailBlob = thumbnailBlob;
            _bus = bus;
        }

        public async Task<Person> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            return await _personStore.GetByIdAsnc(id, cancellationToken);
        }

        public async Task<IEnumerable<Person>> GetPersonsAsync(
            IEnumerable<Guid> ids,
            CancellationToken cancellationToken)
        {
            return await _personStore.GetPersonsAsync(ids, cancellationToken);
        }

        public async Task<IEnumerable<Person>> GetAllAsync(
            CancellationToken cancellationToken)
        {
            return await _personStore.GetAllAsync(cancellationToken);
        }

        public async Task<SearchResult<Person>> SearchAsync(
            SearchPersonRequest request,
            CancellationToken cancellationToken)
        {
            return await _personStore.SearchAsync(request, cancellationToken);
        }

        public async Task<Person> UpdatePersonAsync(
            UpdatePersonRequest request,
            CancellationToken cancellationToken)
        {
            List<Guid> userGroups = request.Groups?.ToList() ?? new List<Guid>();

            if (request.NewGroups is { } newGroupNames && newGroupNames.Any())
            {
                foreach (var newGroup in newGroupNames)
                {
                    Group? group = await _groupService.AddAsync(newGroup, cancellationToken);
                    userGroups.Add(group.Id);
                }
            }

            Person person = await _personStore.GetByIdAsnc(request.Id, cancellationToken);

            person.Name = request.Name;
            person.Groups = userGroups;
            person.ProfileFaceId = request.ProfileFaceId;
            person.DateOfBirth = request.DateOfBirth?.Date;

            await _personStore.UpdateAsync(person, cancellationToken);

            await _bus.Publish(new PersonUpdatedMessage(person.Id, "Update"));

            return person;
        }

        public async Task<Person> GetOrCreatePersonAsync(
            string name,
            CancellationToken cancellationToken)
        {
            Person? person = await _personStore.TryGetByNameAsync(name, cancellationToken);

            if (person == null)
            {
                person = new Person
                {
                    Id = Guid.NewGuid(),
                    Name = name
                };
                person = await _personStore.AddAsync(person, cancellationToken);

                await _bus.Publish(new PersonUpdatedMessage(person.Id, "New"));
            }

            return person;
        }

        public async Task UpdateAllSummaryAsync(
            CancellationToken cancellationToken)
        {
            IEnumerable<Person> allpersons = await GetAllAsync(cancellationToken);

            foreach (Person person in allpersons)
            {
                await UpdateSummaryAsync(person, cancellationToken);
            }
        }

        public async Task<Person> UpdateSummaryAsync(
            Person person,
            CancellationToken cancellationToken)
        {
            Log.Information("Update person summary: {Id}", person.Id);
            person.Summary = await GetSummaryAsync(person, cancellationToken);

            await _personStore.UpdateAsync(person, cancellationToken);

            return person;
        }

        public async Task<PersonSummary> GetSummaryAsync(
            Person person,
            CancellationToken cancellationToken)
        {
            IEnumerable<MediaFace> faces = await _faceStore.GetFacesByPersonAsync(
                person.Id,
                cancellationToken);

            PersonSummary summary = new();
            summary.MediaCount = faces.Select(x => x.MediaId).Distinct().Count();
            summary.HumanCount = faces.Count(x => x.RecognitionType == FaceRecognitionType.Human);
            summary.ComputerCount = faces.Count(x => x.RecognitionType == FaceRecognitionType.Computer);
            summary.ValidatedCount = faces.Count(x => x.State == FaceState.Validated);

            return summary;
        }

        public async Task<MediaThumbnail?> TryGetFaceThumbnailAsync(Guid personId, CancellationToken cancellationToken)
        {
            Person? person = await GetByIdAsync(personId, cancellationToken);

            MediaFace? face = null;

            if (person.ProfileFaceId.HasValue)
            {
                face = await _faceStore.GetByIdAsync(person.ProfileFaceId.Value, cancellationToken);
            }
            else
            {
                SearchResult<MediaFace> searchResult = await _faceStore.SearchAsync(new SearchFacesRequest
                {
                    PageSize = 1,
                    Persons = new[] { personId }
                }, cancellationToken);

                if (searchResult.Items.Any())
                {
                    face = searchResult.Items.First();
                }
            }

            if (face is { })
            {
                face.Thumbnail.Data = await _thumbnailBlob.GetAsync(face.Thumbnail.Id, cancellationToken);

                return face.Thumbnail;
            }

            return null;
        }
    }
}
