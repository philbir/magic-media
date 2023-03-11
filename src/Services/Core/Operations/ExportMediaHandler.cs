using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Messaging;
using MassTransit;

namespace MagicMedia.Operations;

public class ExportMediaHandler : IExportMediaHandler
{
    private readonly IMediaExportService _exportService;
    private readonly IBus _bus;

    public ExportMediaHandler(
        IMediaExportService exportService,
        IBus bus)
    {
        _exportService = exportService;
        _bus = bus;
    }

    public async Task ExecuteAsync(ExportMediaMessage message, CancellationToken cancellationToken)
    {
        var messages = new List<MediaOperationCompletedMessage>();

        foreach (Guid mediaId in message.Ids)
        {
            MediaOperationCompletedMessage msg = new()
            {
                OperationId = message.OperationId, Type = MediaOperationType.Export, MediaId = mediaId
            };

            try
            {
                MediaExportResult export = await _exportService.ExportAsync(
                    mediaId,
                    new MediaExportOptions { ProfileId = message.ProfileId, Path = message.Path },
                    cancellationToken);

                msg.IsSuccess = true;
                msg.Message = export.Path;
            }
            catch (Exception e)
            {
                msg.IsSuccess = false;
                msg.Message = e.Message;
            }

            messages.Add(msg);

            await _bus.Publish(msg, cancellationToken);
        }

        var completedMessage = new MediaOperationRequestCompletedMessage
        {
            Type = MediaOperationType.Export,
            OperationId = message.OperationId,
            SuccessCount = messages.Count(x => x.IsSuccess),
            ErrorCount = messages.Count(x => !x.IsSuccess),
        };

        await _bus.Publish(completedMessage, cancellationToken);
    }
}
