using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Messaging;
using MagicMedia.Store;
using MassTransit;

namespace MagicMedia.Face
{
    public class FaceService : IFaceService
    {
        private readonly IFaceStore _faceStore;
        private readonly IPersonService _personService;
        private readonly IMediaStore _mediaStore;
        private readonly IFaceDetectionService _faceDetectionService;
        private readonly IAgeOperationsService _ageOperationsService;
        private readonly IBus _bus;

        public FaceService(
            IFaceStore faceStore,
            IPersonService personStore,
            IMediaStore mediaStore,
            IFaceDetectionService faceDetectionService,
            IAgeOperationsService ageOperationsService,
            IBus bus)
        {
            _faceStore = faceStore;
            _personService = personStore;
            _mediaStore = mediaStore;
            _faceDetectionService = faceDetectionService;
            _ageOperationsService = ageOperationsService;
            _bus = bus;
        }

        public async Task<MediaFace> AssignPersonByHumanAsync(
            Guid id,
            string personName,
            CancellationToken cancellationToken)
        {
            Person person = await _personService.GetOrCreatePersonAsync(
                personName,
                cancellationToken);

            MediaFace face = await _faceStore.GetByIdAsync(id, cancellationToken);

            face.PersonId = person.Id;
            face.State = FaceState.Validated;
            face.RecognitionType = FaceRecognitionType.Human;

            await UpdateAgeAsync(face, person, cancellationToken);
            await _faceStore.UpdateAsync(face, cancellationToken);

            await _bus.Publish(new FaceUpdatedMessage(face.Id, person.Id, "ASSIGN_BY_HUMAN"));

            return face;
        }

        private async Task UpdateAgeAsync(
            MediaFace face,
            Guid personId,
            CancellationToken cancellationToken)
        {
            Person person = await _mediaStore.Persons.GetByIdAsnc(personId, cancellationToken);

            await UpdateAgeAsync(face, person, cancellationToken);
        }

        private async Task UpdateAgeAsync(
        MediaFace face,
        Person person,
        CancellationToken cancellationToken)
        {
            if (person.DateOfBirth.HasValue)
            {
                Media media = await _mediaStore.GetByIdAsync(face.MediaId, cancellationToken);

                face.Age = _ageOperationsService.CalculateAge(
                    media.DateTaken,
                    person.DateOfBirth.Value);
            }
            else
            {
                face.Age = null;
            }
        }

        public async Task<(MediaFace face, bool hasMatch)> PredictPersonAsync(
            Guid faceId,
            double? distance,
            CancellationToken cancellationToken)
        {
            MediaFace face = await _faceStore.GetByIdAsync(faceId, cancellationToken);

            var distanceValue = distance.GetValueOrDefault(.4);
            Guid? personId = await _faceDetectionService.PredictPersonAsync(
                new PredictPersonRequest
                {
                    Encoding = face.Encoding,
                    Distance = distanceValue,
                }, cancellationToken);

            if (personId.HasValue)
            {
                face = await AssignPersonByComputerAsync(
                    face,
                    personId.Value,
                    distanceValue,
                    cancellationToken);
            }

            return (face, personId.HasValue);
        }

        public async Task<MediaFace> AssignPersonByComputerAsync(
            MediaFace face,
            Guid personId,
            double distance,
            CancellationToken cancellationToken)
        {
            face.PersonId = personId;
            face.RecognitionType = FaceRecognitionType.Computer;
            face.State = FaceState.Predicted;
            face.DistanceThreshold = distance;

            await UpdateAgeAsync(face, personId, cancellationToken);
            await _faceStore.UpdateAsync(face, cancellationToken);
            await _bus.Publish(new FaceUpdatedMessage(face.Id, personId, "ASSIGN_BY_COMPUTER"));

            return face;
        }


        public async Task<MediaFace> UnAssignPersonAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            MediaFace face = await _faceStore.GetByIdAsync(id, cancellationToken);

            return await UnAssignPersonAsync(face, cancellationToken);
        }

        private async Task<MediaFace> UnAssignPersonAsync(
            MediaFace face,
            CancellationToken cancellationToken)
        {
            if (face.PersonId.HasValue)
            {
                Guid currentPersonId = face.PersonId.Value;

                if (face.RecognitionType == FaceRecognitionType.Computer)
                {
                    face.FalsePositivePersons = face.FalsePositivePersons ?? new List<Guid>();
                    face.FalsePositivePersons.Add(currentPersonId);
                }

                face.PersonId = null;
                face.State = FaceState.New;
                face.RecognitionType = FaceRecognitionType.None;
                face.Age = null;

                await _faceStore.UpdateAsync(face, cancellationToken);
                await _bus.Publish(new FaceUpdatedMessage(
                    face.Id,
                    currentPersonId,
                    "UNASSIGN_PERSON"));
            }

            return face;
        }

        public async Task<MediaFace> ApproveComputerAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            MediaFace face = await _faceStore.GetByIdAsync(id, cancellationToken);

            return await ApproveComputerAsync(face, cancellationToken);
        }

        public async Task<IEnumerable<MediaFace>> ApproveAllByMediaAsync(
            Guid mediaId,
            CancellationToken cancellationToken)
        {
            IEnumerable<MediaFace> faces = await _faceStore.GetFacesByMediaAsync(mediaId, cancellationToken);

            IEnumerable<MediaFace> filtered = faces
                .Where(x => x.PersonId.HasValue && x.RecognitionType == FaceRecognitionType.Computer);

            foreach (MediaFace face in filtered)
            {
                await ApproveComputerAsync(face, cancellationToken);
            }

            return filtered;
        }

        public async Task<IEnumerable<MediaFace>> UnassignAllPredictedByMediaAsync(
            Guid mediaId,
            CancellationToken cancellationToken)
        {
            IEnumerable<MediaFace> faces = await _faceStore.GetFacesByMediaAsync(mediaId, cancellationToken);

            IEnumerable<MediaFace> filtered = faces
                .Where(x =>
                    x.PersonId.HasValue &&
                    x.RecognitionType == FaceRecognitionType.Computer &&
                    x.State == FaceState.Predicted);


            foreach (MediaFace face in filtered)
            {
                await ApproveComputerAsync(face, cancellationToken);
            }

            return filtered;
        }

        public async Task<IEnumerable<Guid>> DeleteUnassingedByMediaAsync(
            Guid mediaId,
            CancellationToken cancellationToken)
        {
            IEnumerable<MediaFace> faces = await _faceStore.GetFacesByMediaAsync(mediaId, cancellationToken);

            IEnumerable<MediaFace> filtered = faces
                .Where(x =>
                    x.PersonId.HasValue == false && 
                    x.State == FaceState.New);

            foreach (MediaFace face in filtered)
            {
                await DeleteAsync(face.Id, cancellationToken);
            }

            return filtered.Select(x => x.Id);
        }

        private async Task<MediaFace> ApproveComputerAsync(
            MediaFace face,
            CancellationToken cancellationToken)
        {
            if (face.RecognitionType == FaceRecognitionType.Computer &&
                face.PersonId.HasValue)
            {
                face.State = FaceState.Validated;

                await _faceStore.UpdateAsync(face, cancellationToken);
                await _bus.Publish(new FaceUpdatedMessage(
                    face.Id,
                    face.PersonId.Value,
                    "APPROVE_COMPUTER"));
            }

            return face;
        }

        public async Task<MediaFace> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            MediaFace face = await _faceStore.GetByIdAsync(id, cancellationToken);

            return face;
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            await _faceStore.DeleteAsync(id, cancellationToken);

            await _bus.Publish(new FaceUpdatedMessage(id, "DELETED"));
        }

        public async Task<MediaThumbnail> GetThumbnailAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            MediaFace face = await GetByIdAsync(id, cancellationToken);

            face.Thumbnail.Data = await _mediaStore.Thumbnails.GetAsync(
                face.Thumbnail.Id,
                cancellationToken);

            return face.Thumbnail;
        }
    }
}
