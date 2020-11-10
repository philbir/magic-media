using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Messaging;
using MagicMedia.Store;
using MassTransit;

namespace MagicMedia.Operations
{
    public class MoveMediaHandler : IMoveMediaHandler
    {
        private readonly IMediaStore _mediaStore;
        private readonly IMediaBlobStore _mediaBlobStore;
        private readonly IBus _bus;

        public MoveMediaHandler(
            IMediaStore _mediaStore,
            IMediaBlobStore mediaBlobStore,
            IBus bus)
        {
            this._mediaStore = _mediaStore;
            _mediaBlobStore = mediaBlobStore;
            _bus = bus;
        }

        public Guid MediaId { get; private set; }

        public async Task ExecuteAsync(
            MoveMediaMessage message,
            CancellationToken cancellationToken)
        {
            var messages = new List<MoveMediaCompletedMessage>();

            foreach (Guid mediaId in message.Ids)
            {
                MoveMediaCompletedMessage msg = await MoveMediaAsync(
                    mediaId,
                    message.NewLocation,
                    cancellationToken);

                msg.OperationId = message.OperationId;
                messages.Add(msg);

                await _bus.Publish(msg, cancellationToken);
            }

            var completedmsg = new MoveMediaRequestCompletedMessage
            {
                OperationId = message.OperationId,
                SuccessCount = messages.Where(x => x.IsSuccess).Count(),
                ErrorCount = messages.Where(x => !x.IsSuccess).Count(),
            };

            await _bus.Publish(completedmsg, cancellationToken);
        }

        private async Task<MoveMediaCompletedMessage> MoveMediaAsync(
            Guid id,
            string newLocation,
            CancellationToken cancellationToken)
        {
            Media media = await _mediaStore.GetByIdAsync(id, cancellationToken);

            MoveMediaCompletedMessage msg = new MoveMediaCompletedMessage
            {
                MediaId = id,
                OldFolder = media.Folder,
                NewFolder = newLocation
            };
            try
            {
                await _mediaBlobStore.MoveAsync(
                    new MediaBlobData
                    {
                        Directory = media.Folder,
                        Filename = media.Filename
                    },
                    newLocation,
                    cancellationToken);

                media.Folder = newLocation;

                await _mediaStore.UpdateAsync(media, cancellationToken);
                msg.Message = $"{media.Filename} moved from {msg.OldFolder} to {msg.NewFolder}";
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
}
