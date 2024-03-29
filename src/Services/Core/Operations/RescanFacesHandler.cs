using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Messaging;
using MagicMedia.Processing;
using MassTransit;

namespace MagicMedia.Operations;

public class RescanFacesHandler : IRescanFacesHandler
{
    private readonly IMediaFaceScanner _mediaFaceScanner;
    private readonly IBus _bus;

    public RescanFacesHandler(
        IMediaFaceScanner mediaFaceScanner,
        IBus bus)
    {
        _mediaFaceScanner = mediaFaceScanner;
        _bus = bus;
    }

    public async Task ExecuteAsync(
        RescanFacesMessage message,
        CancellationToken cancellationToken)
    {
        var messages = new List<MediaOperationCompletedMessage>();

        foreach (Guid mediaId in message.Ids)
        {
            MediaOperationCompletedMessage msg = new();
            msg.Type = MediaOperationType.RescanFaces;

            try
            {
                await _mediaFaceScanner
                    .ScanByMediaIdAsync(mediaId, cancellationToken);

                msg.IsSuccess = true;
                msg.Type = MediaOperationType.RescanFaces;
                msg.MediaId = mediaId;
                msg.OperationId = message.OperationId;
            }
            catch (Exception ex)
            {
                msg.IsSuccess = false;
                msg.Message = ex.Message;
            }

            messages.Add(msg);

            await _bus.Publish(msg, cancellationToken);
        }

        var completedMessage = new MediaOperationRequestCompletedMessage
        {
            Type = MediaOperationType.RescanFaces,
            OperationId = message.OperationId,
            SuccessCount = messages.Count(x => x.IsSuccess),
            ErrorCount = messages.Count(x => !x.IsSuccess),
        };

        await _bus.Publish(completedMessage, cancellationToken);
    }
}
