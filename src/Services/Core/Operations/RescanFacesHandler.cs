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

            await _bus.Publish(msg, cancellationToken);
        }

        var completedmsg = new MediaOperationRequestCompletedMessage
        {
            Type = MediaOperationType.RescanFaces,
            OperationId = message.OperationId,
            SuccessCount = messages.Where(x => x.IsSuccess).Count(),
            ErrorCount = messages.Where(x => !x.IsSuccess).Count(),
        };

        await _bus.Publish(completedmsg, cancellationToken);
    }
}
