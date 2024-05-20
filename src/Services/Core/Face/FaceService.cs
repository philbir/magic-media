using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Messaging;
using MagicMedia.Search;
using MagicMedia.Security;
using MagicMedia.Store;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace MagicMedia.Face;

public class FaceService(
    IFaceStore faceStore,
    IPersonService personStore,
    IMediaStore mediaStore,
    IUserContextFactory userContextFactory,
    IFaceDetectionService faceDetectionService,
    IAgeOperationsService ageOperationsService,
    IBus bus,
    ILogger<FaceService> logger)
    : IFaceService
{
    public async Task<MediaFace> AssignPersonByHumanAsync(
        Guid id,
        string personName,
        CancellationToken cancellationToken)
    {
        Person person = await personStore.GetOrCreatePersonAsync(
            personName,
            cancellationToken);

        MediaFace face = await faceStore.GetByIdAsync(id, cancellationToken);

        face.PersonId = person.Id;
        face.State = FaceState.Validated;
        face.RecognitionType = FaceRecognitionType.Human;

        await CalculateAgeAsync(face, person, cancellationToken);
        await faceStore.UpdateAsync(face, cancellationToken);

        await bus.Publish(new FaceUpdatedMessage(face.Id, "ASSIGN_BY_HUMAN") { PersonId = person.Id });

        return face;
    }

    public async Task<SearchResult<MediaFace>> SearchAsync(
        SearchFacesRequest request,
        CancellationToken cancellationToken)
    {
        IUserContext userContext = await userContextFactory.CreateAsync(cancellationToken);

        if (!userContext.HasPermission(Permissions.Media.ViewAll))
        {
            request.AuthorizedOnMedia = await userContext.GetAuthorizedMediaAsync(cancellationToken);
        }

        return await faceStore.SearchAsync(request, cancellationToken);
    }

    public async Task<MediaFace> UpdateAgeAsync(MediaFace face, CancellationToken cancellationToken)
    {
        await CalculateAgeAsync(face, cancellationToken);
        await faceStore.UpdateAsync(face, cancellationToken);

        return face;
    }

    private async Task CalculateAgeAsync(
       MediaFace face,
       CancellationToken cancellationToken)
    {
        if (face.PersonId.HasValue)
        {
            Person person = await mediaStore.Persons.GetByIdAsync(face.PersonId.Value, cancellationToken);
            await CalculateAgeAsync(face, person, cancellationToken);
        }
    }

    private async Task CalculateAgeAsync(
        MediaFace face,
        Person person,
        CancellationToken cancellationToken)
    {
        if (person.DateOfBirth.HasValue)
        {
            Media media = await mediaStore.GetByIdAsync(face.MediaId, cancellationToken);

            face.Age = ageOperationsService.CalculateAge(
                media.DateTaken,
                person.DateOfBirth.Value);
        }
        else
        {
            face.Age = null;
        }
    }

    public async Task<IEnumerable<(MediaFace face, bool hasMatch)>> PredictPersonsByMediaAsync(
        Guid mediaId,
        double? distance,
        CancellationToken cancellationToken)
    {
        IEnumerable<MediaFace> faces = await GetFacesByMediaAsync(mediaId, cancellationToken);

        var results = new List<(MediaFace face, bool hasMatch)>();

        foreach (MediaFace face in faces.Where(x => x.PersonId == null))
        {
            (MediaFace face, bool hasMatch) faceResult = await PredictPersonAsync(
                face,
                distance,
                cancellationToken);

            results.Add(faceResult);
        }

        return results;
    }

    public async Task<(MediaFace face, bool hasMatch)> PredictPersonAsync(
        MediaFace face,
        double? distance,
        CancellationToken cancellationToken)
    {
        var distanceValue = distance.GetValueOrDefault(.4);
        Guid? personId = await faceDetectionService.PredictPersonAsync(
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

    public async Task<(MediaFace face, bool hasMatch)> PredictPersonAsync(
        Guid faceId,
        double? distance,
        CancellationToken cancellationToken)
    {
        MediaFace face = await faceStore.GetByIdAsync(faceId, cancellationToken);

        return await PredictPersonAsync(face, distance, cancellationToken);
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

        await CalculateAgeAsync(face, cancellationToken);
        await faceStore.UpdateAsync(face, cancellationToken);
        await bus.Publish(
            new FaceUpdatedMessage(face.Id, "ASSIGN_BY_COMPUTER") { PersonId = personId },
            cancellationToken);

        return face;
    }


    public async Task<MediaFace> UnAssignPersonAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        MediaFace face = await faceStore.GetByIdAsync(id, cancellationToken);

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

            await faceStore.UpdateAsync(face, cancellationToken);
            await bus.Publish(
                new FaceUpdatedMessage(face.Id, "UNASSIGN_PERSON") { PersonId = currentPersonId },
                cancellationToken);
        }

        return face;
    }

    public async Task<MediaFace> ApproveComputerAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        MediaFace face = await faceStore.GetByIdAsync(id, cancellationToken);

        return await ApproveComputerAsync(face, cancellationToken);
    }

    public async Task<IEnumerable<MediaFace>> ApproveAllByMediaAsync(
        Guid mediaId,
        CancellationToken cancellationToken)
    {
        IEnumerable<MediaFace> faces = await faceStore.GetFacesByMediaAsync(mediaId, cancellationToken);

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
        IEnumerable<MediaFace> faces = await faceStore.GetFacesByMediaAsync(mediaId, cancellationToken);

        IEnumerable<MediaFace> filtered = faces
            .Where(x =>
                x.PersonId.HasValue &&
                x.RecognitionType == FaceRecognitionType.Computer &&
                x.State != FaceState.Validated);


        foreach (MediaFace face in filtered)
        {
            await UnAssignPersonAsync(face, cancellationToken);
        }

        return filtered;
    }

    public async Task<IEnumerable<Guid>> DeleteUnassignedByMediaAsync(
        Guid mediaId,
        CancellationToken cancellationToken)
    {
        IEnumerable<MediaFace> faces = await faceStore.GetFacesByMediaAsync(mediaId, cancellationToken);

        IEnumerable<MediaFace> filtered = faces
            .Where(x => x.PersonId.HasValue == false);

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

            await CalculateAgeAsync(face, cancellationToken);
            await faceStore.UpdateAsync(face, cancellationToken);

            await bus.Publish(new FaceUpdatedMessage(face.Id, "APPROVE_COMPUTER")
            {
                PersonId = face.PersonId.Value
            }, cancellationToken);
        }

        return face;
    }

    public async Task<MediaFace> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        MediaFace face = await faceStore.GetByIdAsync(id, cancellationToken);

        return face;
    }

    public async Task<IEnumerable<MediaFace>> GetFacesByMediaAsync(
        Guid mediaId,
        CancellationToken cancellationToken)
    {
        return await faceStore.GetFacesByMediaAsync(mediaId, cancellationToken);
    }

    public async Task DeleteByMediaIdAsync(Guid mediaId, CancellationToken cancellationToken)
    {
        IEnumerable<MediaFace> faces = await GetFacesByMediaAsync(mediaId, cancellationToken);

        faces.ToList().ForEach(async face => await DeleteAsync(face, cancellationToken));
     }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        MediaFace face = await GetByIdAsync(id, cancellationToken);
        await DeleteAsync(face, cancellationToken);
    }

    private async Task DeleteAsync(MediaFace face, CancellationToken cancellationToken)
    {
        logger.DeletingFace(face.Id);

        await faceStore.DeleteAsync(face.Id, cancellationToken);

        if (face.Thumbnail != null)
        {
            await mediaStore.Thumbnails.DeleteAsync(face.Thumbnail.Id, cancellationToken);
        }

        await bus.Publish(new FaceUpdatedMessage(face.Id, "DELETED"), cancellationToken);
    }

    public async Task<MediaThumbnail> GetThumbnailAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        MediaFace face = await GetByIdAsync(id, cancellationToken);

        face.Thumbnail.Data = await mediaStore.Thumbnails.GetAsync(
            face.Thumbnail.Id,
            cancellationToken);

        return face.Thumbnail;
    }
}

public static partial class FaceServiceLoggerExtensions
{
    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Deleting face {Id}")]
    public static partial void DeletingFace(this ILogger logger, Guid id);
}
