using System.Threading.Tasks;
using MagicMedia.Store;

namespace MagicMedia.Operations
{
    public class MoveMediaUpdateMedadataOperationStep : IMediaOperationStep
    {
        private readonly IMediaStore _mediaStore;

        public MoveMediaUpdateMedadataOperationStep(IMediaStore mediaStore)
        {
            _mediaStore = mediaStore;
        }

        public string Name => MediaOperationStepNames.MoveMediaUpdateMetadata;

        public async Task<MediaOperationStepResult> ExecuteAsync(MediaOperationStepContext context)
        {
            Media media = await _mediaStore.GetByIdAsync(
                context.Task.Entity.Id,
                context.OperationAbord);

            media.Folder = (string)context.Task.Data["NewLocation"];

            await _mediaStore.UpdateAsync(media, context.OperationAbord);

            return new MediaOperationStepResult
            {
                State = MediaOperationStepState.Success
            };
        }
    }
}
