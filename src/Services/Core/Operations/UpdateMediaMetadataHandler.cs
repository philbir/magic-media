using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Messaging;
using MagicMedia.Store;
using MassTransit;
using NGeoHash;

namespace MagicMedia.Operations;

public class UpdateMediaMetadataHandler : IUpdateMediaMetadataHandler
{
    private readonly IMediaStore _mediaStore;
    private readonly IMediaService _mediaService;
    private readonly IBus _bus;

    public UpdateMediaMetadataHandler(
        IMediaStore mediaStore,
        IMediaService mediaService,
        IBus bus)
    {
        _mediaStore = mediaStore;
        _mediaService = mediaService;
        _bus = bus;
    }

    public async Task ExecuteAsync(
        UpdateMediaMetadataMessage message,
        CancellationToken cancellationToken)
    {
        var messages = new List<MediaOperationCompletedMessage>();

        foreach (Guid mediaId in message.Ids)
        {
            MediaOperationCompletedMessage msg = await UpdateMetadataAsync(
                mediaId,
                message.DateTaken,
                message.GeoLocation,
                cancellationToken);

            msg.OperationId = message.OperationId;
            messages.Add(msg);

            await _bus.Publish(msg, cancellationToken);
        }

        var completedMessage = new MediaOperationRequestCompletedMessage
        {
            Type = MediaOperationType.UpdateMetadata,
            OperationId = message.OperationId,
            SuccessCount = messages.Count(x => x.IsSuccess),
            ErrorCount = messages.Count(x => !x.IsSuccess),
        };

        await _bus.Publish(completedMessage, cancellationToken);
    }

    private async Task<MediaOperationCompletedMessage> UpdateMetadataAsync(
        Guid id,
        DateTimeOffset? dateTaken,
        UpdateMedataGeoLocation? geoLocation,
        CancellationToken cancellationToken)
    {
        MediaOperationCompletedMessage msg = new MediaOperationCompletedMessage
        {
            Type = MediaOperationType.UpdateMetadata,
            MediaId = id,
        };

        try
        {
            if (dateTaken.HasValue)
            {
                await _mediaService.UpdateDateTakenAsync(id, dateTaken, cancellationToken);
            }
            if (geoLocation is { } geo && geo.Latitude.HasValue && geo.Longitude.HasValue)
            {
                Media media = await _mediaStore.GetByIdAsync(id, cancellationToken);

                media.GeoLocation = new GeoLocation
                {
                    Point = GeoPoint.Create(geo.Latitude.Value, geo.Longitude.Value),
                    Type = "User",
                    GeoHash = GeoHash.Encode(geo.Latitude.Value, geo.Longitude.Value),
                    Address = GetAddress(geoLocation)
                };

                await _mediaStore.UpdateAsync(media, cancellationToken);
            }

            msg.IsSuccess = true;
        }
        catch (Exception ex)
        {
            msg.IsSuccess = false;
            msg.Message = ex.Message;
        }

        return msg;
    }

    private GeoAddress GetAddress(UpdateMedataGeoLocation geoLocation)
    {
        return new GeoAddress
        {
            Name = geoLocation.Name,
            Address = geoLocation.Address,
            City = geoLocation.City,
            Country = geoLocation.Country,
            CountryCode = geoLocation.CountryCode,
            Distric1 = geoLocation.Distric1,
            Distric2 = geoLocation.Distric2
        };
    }
}
