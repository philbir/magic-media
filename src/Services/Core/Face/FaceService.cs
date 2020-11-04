using System;
using System.Collections.Generic;
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
        private readonly IFaceDetectionService _faceDetectionService;
        private readonly IBus _bus;

        public FaceService(
            IFaceStore faceStore,
            IPersonService personStore,
            IFaceDetectionService faceDetectionService,
            IBus bus)
        {
            _faceStore = faceStore;
            _personService = personStore;
            _faceDetectionService = faceDetectionService;
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

            await _faceStore.UpdateAsync(face, cancellationToken);

            await _bus.Publish(new FaceUpdatedMessage(face.Id, person.Id, "ASSIGN_BY_HUMAN"));

            return face;
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

            await _faceStore.UpdateAsync(face, cancellationToken);
            await _bus.Publish(new FaceUpdatedMessage(face.Id, personId, "ASSIGN_BY_COMPUTER"));

            return face;
        }

        public async Task<MediaFace> UnAssignPersonAsync(
        Guid id,
        CancellationToken cancellationToken)
        {
            MediaFace face = await _faceStore.GetByIdAsync(id, cancellationToken);

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
    }
}
