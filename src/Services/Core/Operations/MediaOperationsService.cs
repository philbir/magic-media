using System;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Messaging;
using MagicMedia.Store;
using MassTransit;

namespace MagicMedia.Operations;

public class MediaOperationsService : IMediaOperationsService
{
    private readonly IBus _bus;
    private readonly IMediaStore _mediaStore;

    public MediaOperationsService(IBus bus, IMediaStore mediaStore)
    {
        _bus = bus;
        _mediaStore = mediaStore;
    }

    public async Task MoveMediaAsync(
        MoveMediaRequest request,
        CancellationToken cancellationToken)
    {
        var message = new MoveMediaMessage
        {
            Ids = request.Ids,
            NewLocation = request.NewLocation,
            Rule = request.Rule,
            OperationId = request.OperationId!,
        };

        await _bus.Publish(message, cancellationToken);
    }

    public async Task ToggleFavoriteAsync(Guid id, bool isFavorite, CancellationToken cancellationToken)
    {
        Media media = await _mediaStore.GetByIdAsync(id, cancellationToken);
        media.IsFavorite = isFavorite;

        await _mediaStore.UpdateAsync(media, cancellationToken);

        await _bus.Publish(new FavoriteMediaToggledMessage(id, isFavorite));
    }

    public async Task RecycleAsync(
        RecycleMediaRequest request,
        CancellationToken cancellationToken)
    {
        RecycleMediaMessage message = new(request.Ids)
        {
            OperationId = request.OperationId
        };

        await _bus.Publish(message, cancellationToken);
    }

    public async Task DeleteAsync(
        DeleteMediaRequest request,
        CancellationToken cancellationToken)
    {
        DeleteMediaMessage message = new(request.Ids)
        {
            OperationId = request.OperationId
        };

        await _bus.Publish(message, cancellationToken);
    }

    public async Task UpdateMetadataAsync(
        UpdateMediaMetadataRequest request,
        CancellationToken cancellationToken)
    {
        UpdateMediaMetadataMessage message = new(request.Ids)
        {
            OperationId = request.OperationId,
            DateTaken = request.DateTaken,
            GeoLocation = request.GeoLocation
        };

        await _bus.Publish(message, cancellationToken);
    }

    public async Task ReScanFacesAsync(
        RescanFacesRequest request,
        CancellationToken cancellationToken)
    {
        RescanFacesMessage message = new(request.Ids)
        {
            OperationId = request.OperationId
        };

        await _bus.Publish(message, cancellationToken);
    }

    public async Task ExportAsync(
        ExportMediaRequest request,
        CancellationToken cancellationToken)
    {
        ExportMediaMessage message = new(request.Ids, request.ProfileId)
        {
            OperationId = request.OperationId,
            Path = request.Path
        };

        await _bus.Publish(message, cancellationToken);
    }
}
