using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Types;
using MagicMedia.Face;
using MagicMedia.Store;

namespace MagicMedia.GraphQL.Face
{
    [ExtendObjectType(Name = "Mutation")]
    public partial class FaceMutations
    {
        private readonly IFaceService _faceService;
        private readonly IFaceModelBuilderService _faceModelBuilder;
        private readonly IFaceDetectionService _faceDetectionService;

        public FaceMutations(
            IFaceService faceService,
            IFaceModelBuilderService faceModelBuilder,
            IFaceDetectionService faceDetectionService)
        {
            _faceService = faceService;
            _faceModelBuilder = faceModelBuilder;
            _faceDetectionService = faceDetectionService;
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

        public async Task<UpdateFacesPayload> ApproveAllFacesByMediaAsync(
            Guid mediaId,
            CancellationToken cancellationToken)
        {
            IEnumerable<MediaFace>? faces = await _faceService.ApproveAllByMediaAsync(
                mediaId,
                cancellationToken);

            return new UpdateFacesPayload(faces);
        }


        public async Task<UpdateFacePayload> UnAssignPersonFromFaceAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            MediaFace face = await _faceService.UnAssignPersonAsync(
                id,
                cancellationToken);

            return new UpdateFacePayload(face);
        }

        public async Task<UpdateFacesPayload> UnAssignAllPredictedPersonsByMediaAsync(
            Guid mediaId,
            CancellationToken cancellationToken)
        {
            IEnumerable<MediaFace>? faces = await _faceService.UnassignAllPredictedByMediaAsync(
                mediaId,
                cancellationToken);

            return new UpdateFacesPayload(faces);
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

        public async Task<DeleteFacesPayload> DeleteUnassignedFacesByMediaAsync(
            Guid mediaId,
            CancellationToken cancellationToken)
        {
            IEnumerable<Guid>? faceIds = await _faceService.DeleteUnassingedByMediaAsync(
                mediaId,
                cancellationToken);

            return new DeleteFacesPayload(faceIds);
        }

        public async Task<BuildFaceModelPayload> BuildPersonModelAsync(CancellationToken cancellationToken)
        {
            BuildFaceModelResult? res = await _faceModelBuilder.BuildModelAsyc(cancellationToken);

            return new BuildFaceModelPayload(res.FaceCount);
        }

        public async Task<PredictPersonPayload> PredictPersonAsync(
            PredictPersonInput input,
            CancellationToken cancellationToken)
        {
            (MediaFace face, bool hasMatch) result = await _faceService.PredictPersonAsync(
                input.FaceId,
                input.Distance,
                cancellationToken);

            return new PredictPersonPayload(result.hasMatch, result.face);
        }
    }
}
