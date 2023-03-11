using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Face;
using MagicMedia.Messaging;
using MagicMedia.Store;
using MassTransit;

namespace MagicMedia.Operations;

public class DeleteMediaHandler : IDeleteMediaHandler
{
    private readonly IMediaService _mediaService;
    private readonly IFaceService _faceService;
    private readonly IBus _bus;

    public DeleteMediaHandler(
        IMediaService mediaService,
        IFaceService faceService,
        IBus bus)
    {
        _mediaService = mediaService;
        _faceService = faceService;
        _bus = bus;
    }

    public async Task ExecuteAsync(
        DeleteMediaMessage message,
        CancellationToken cancellationToken)
    {
        var messages = new List<MediaOperationCompletedMessage>();

        foreach (Guid mediaId in message.Ids)
        {
            MediaOperationCompletedMessage msg = await DeleteAsync(
                mediaId,
                cancellationToken);

            msg.OperationId = message.OperationId;
            messages.Add(msg);

            await _bus.Publish(msg, cancellationToken);
        }

        var completedMessage = new MediaOperationRequestCompletedMessage
        {
            Type = MediaOperationType.Delete,
            OperationId = message.OperationId,
            SuccessCount = messages.Count(x => x.IsSuccess),
            ErrorCount = messages.Count(x => !x.IsSuccess),
        };

        await _bus.Publish(completedMessage, cancellationToken);
    }


    private async Task<MediaOperationCompletedMessage> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        MediaOperationCompletedMessage msg = new MediaOperationCompletedMessage
        {
            Type = MediaOperationType.Delete,
            MediaId = id,
        };

        Media media = await _mediaService.GetByIdAsync(id, cancellationToken);

        try
        {
            await _faceService.DeleteByMediaIdAsync(media.Id, cancellationToken);
            await _mediaService.DeleteAsync(media, cancellationToken);

            msg.Message = $"{media.Filename} deleted";

            msg.IsSuccess = true;
        }
        catch (Exception ex)
        {
            msg.IsSuccess = false;
            msg.Message = ex.Message;
        }

        return msg;
    }
}
