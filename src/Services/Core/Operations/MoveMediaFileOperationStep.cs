using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Store;

namespace MagicMedia.Operations
{
    public class MoveMediaFileOperationStep : IMediaOperationStep
    {
        private readonly IMediaStore _mediaStore;
        private readonly IMediaBlobStore _mediaBlobStore;

        public MoveMediaFileOperationStep(
            IMediaStore mediaStore,
            IMediaBlobStore mediaBlobStore)
        {
            _mediaStore = mediaStore;
            _mediaBlobStore = mediaBlobStore;
        }
        public string Name => MediaOperationStepNames.MoveMediaFile;

        public async Task<MediaOperationStepResult> ExecuteAsync(MediaOperationStepContext context)
        {
            Media media = await _mediaStore.GetByIdAsync(
                context.Task.Entity.Id,
                context.OperationAbord);

            await _mediaBlobStore.MoveAsync(new MediaBlobData
            {
                Directory = media.Folder,
                Filename = media.Filename
            }, (string) context.Task.Data["NewLocation"],
            context.OperationAbord);

            return new MediaOperationStepResult
            {
                State = MediaOperationStepState.Success
            };
        }
    }
}
