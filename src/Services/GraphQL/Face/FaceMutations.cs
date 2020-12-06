using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IMediaService _mediaService;

        public FaceMutations(
            IFaceService faceService,
            IFaceModelBuilderService faceModelBuilder,
            IFaceDetectionService faceDetectionService,
            IMediaService mediaService)
        {
            _faceService = faceService;
            _faceModelBuilder = faceModelBuilder;
            _faceDetectionService = faceDetectionService;
            _mediaService = mediaService;
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

        public async Task<PredictPersonsByMediaPayload> PredictPersonsByMediaAsync(
            PredictPersonsByMediaInput input,
            CancellationToken cancellationToken)
        {
            IEnumerable<(MediaFace face, bool hasMatch)>? results = await _faceService
                .PredictPersonsByMediaAsync(
                    input.MediaId,
                    input.Distance,
                    cancellationToken);

            Media media = await _mediaService.GetByIdAsync(input.MediaId, cancellationToken);

            return new PredictPersonsByMediaPayload(results.Count(x => x.hasMatch), media);
        }
    }
}
