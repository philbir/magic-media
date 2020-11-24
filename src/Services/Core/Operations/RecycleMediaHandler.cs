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
    public class RecycleMediaHandler : IRecycleMediaHandler
    {
        private readonly IMediaStore _mediaStore;
        private readonly IMediaBlobStore _mediaBlobStore;
        private readonly IBus _bus;

        public RecycleMediaHandler(
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
            RecycleMediaMessage message,
            CancellationToken cancellationToken)
        {
            var messages = new List<MediaOperationCompletedMessage>();

            foreach (Guid mediaId in message.Ids)
            {
                MediaOperationCompletedMessage msg = await RecycleAsync(
                    mediaId,
                    cancellationToken);

                msg.OperationId = message.OperationId;
                messages.Add(msg);

                await _bus.Publish(msg, cancellationToken);
            }

            var completedmsg = new MediaOperationRequestCompletedMessage
            {
                Type = MediaOperationType.Recycle,
                OperationId = message.OperationId,
                SuccessCount = messages.Where(x => x.IsSuccess).Count(),
                ErrorCount = messages.Where(x => !x.IsSuccess).Count(),
            };

            await _bus.Publish(completedmsg, cancellationToken);
        }

        private async Task<MediaOperationCompletedMessage> RecycleAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            Media media = await _mediaStore.GetByIdAsync(id, cancellationToken);

            MediaOperationCompletedMessage msg = new MediaOperationCompletedMessage
            {
                Type = MediaOperationType.Recycle,
                MediaId = id,
            };

            try
            {
                await _mediaBlobStore.MoveToSpecialFolderAsync(
                    new MediaBlobData
                    {
                        Directory = media.Folder,
                        Filename = media.Filename,
                        Type = MediaBlobType.Media
                    },
                    MediaBlobType.Recycled,
                    cancellationToken);

                media.Folder = null;
                media.State = MediaState.Recycled;

                await _mediaStore.UpdateAsync(media, cancellationToken);
                msg.Message = $"{media.Filename} Recycled";

                await RecycleFacesAsync(media.Id, cancellationToken);

                msg.IsSuccess = true;
            }
            catch (Exception ex)
            {
                msg.IsSuccess = false;
                msg.Message = ex.Message;
            }

            return msg;
        }

        private async Task RecycleFacesAsync(Guid mediaId, CancellationToken cancellationToken)
        {
            IEnumerable<MediaFace> faces = await _mediaStore.Faces.GetFacesByMediaAsync(
                mediaId,
                cancellationToken);

            foreach (MediaFace face in faces)
            {
                face.State = FaceState.Recycled;

                await _mediaStore.Faces.UpdateAsync(face, cancellationToken);
            }
        }
    }
}
