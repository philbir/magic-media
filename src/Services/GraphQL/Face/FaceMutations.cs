using System;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Types;
using MagicMedia.Face;
using MagicMedia.Store;

namespace MagicMedia.GraphQL.Face
{
    [ExtendObjectType(Name = "Mutation")]
    public class FaceMutations
    {
        private readonly IFaceService _faceService;

        public FaceMutations(IFaceService faceService)
        {
            _faceService = faceService;
        }

        public async Task<UpdateFacePayload> AssignPersonByHumanAsync(
            AssignPersonByHumanInput input,
            CancellationToken cancellationToken)
        {
            MediaFace face = await _faceService.AssignPersonByHumanAsync(
                input.FaceId,
                input.PersonName,
                cancellationToken);

            return new UpdateFacePayload(face);
        }

        public async Task<UpdateFacePayload> ApproveFaceComputerAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            MediaFace face = await _faceService.ApproveComputerAsync(
                id,
                cancellationToken);

            return new UpdateFacePayload(face);
        }

        public async Task<DeleteFacePayload> DeleteFaceAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            await _faceService.DeleteAsync(
                id,
                cancellationToken);

            return new DeleteFacePayload(id);
        }
    }
}
