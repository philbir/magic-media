using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using MagicMedia.Messaging;
using MagicMedia.Store;
using MassTransit;

namespace MagicMedia.Operations;

public class MoveMediaHandler : IMoveMediaHandler
{
    private readonly IMediaStore _mediaStore;
    private readonly IMediaBlobStore _mediaBlobStore;
    private readonly IBus _bus;

    public MoveMediaHandler(
        IMediaStore mediaStore,
        IMediaBlobStore mediaBlobStore,
        IBus bus)
    {
        _mediaStore = mediaStore;
        _mediaBlobStore = mediaBlobStore;
        _bus = bus;
    }

    public Guid MediaId { get; private set; }

    public async Task ExecuteAsync(
        MoveMediaMessage message,
        CancellationToken cancellationToken)
    {
        var messages = new List<MediaOperationCompletedMessage>();

        foreach (Guid mediaId in message.Ids)
        {
            MediaOperationCompletedMessage msg = await MoveMediaAsync(
                mediaId,
                message.NewLocation,
                message.Rule,
                cancellationToken);

            msg.OperationId = message.OperationId;
            messages.Add(msg);

            await _bus.Publish(msg, cancellationToken);
        }

        var completedMessage = new MediaOperationRequestCompletedMessage
        {
             Type = MediaOperationType.Move,
            OperationId = message.OperationId,
            SuccessCount = messages.Count(x => x.IsSuccess),
            ErrorCount = messages.Count(x => !x.IsSuccess),
        };

        await _bus.Publish(completedMessage, cancellationToken);
    }

    private async Task<MediaOperationCompletedMessage> MoveMediaAsync(
        Guid id,
        string newLocation,
        string? rule,
        CancellationToken cancellationToken)
    {
        Media media = await _mediaStore.GetByIdAsync(id, cancellationToken);

        newLocation = GetNewLocation(media, newLocation, rule);

        MediaOperationCompletedMessage msg = new()
        {
            MediaId = id,
            Data = new() { ["OldFolder"] = media.Folder, ["NewFolder"] = newLocation },
        };
        try
        {
            var movedFilename = await _mediaBlobStore.MoveAsync(
                new MediaBlobData
                {
                    Directory = media.Folder,
                    Filename = media.Filename
                },
                newLocation,
                cancellationToken);

            media.Folder = newLocation;
            media.Filename = movedFilename;

            await _mediaStore.UpdateAsync(media, cancellationToken);
            msg.Message = $"{media.Filename} moved from " +
                $"{msg.Data["OldFolder"]} to {media.Folder}";

            msg.IsSuccess = true;
        }
        catch (Exception ex)
        {
            msg.IsSuccess = false;
            msg.Message = ex.Message;
        }

        return msg;
    }

    private string GetNewLocation(Media media, string newLocation, string? rule)
    {
        if (rule is { })
        {
            switch (rule)
            {
                case "YearAndMonth":
                    if (media.DateTaken.HasValue)
                    {
                        return Path.Combine(
                            newLocation,
                            media.DateTaken.Value.Year.ToString(),
                            "By Month",
                            $"{media.DateTaken.Value:MM} {media.DateTaken.Value:MMMM}");
                    }
                    else
                    {
                        return "Unknown_Date";
                    }
                    break;
            }
        }

        return newLocation;
    }
}
