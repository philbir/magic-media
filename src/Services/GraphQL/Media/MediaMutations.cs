using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Types;
using MagicMedia.Operations;
using MagicMedia.Store;

namespace MagicMedia.GraphQL
{
    [ExtendObjectType(Name = "Mutation")]
    public class MediaMutations
    {
        private readonly IMediaOperationsService _operationsService;
        private readonly ICloudAIMediaProcessingService _cloudAIMediaProcessing;

        public MediaMutations(
            IMediaOperationsService operationsService,
            ICloudAIMediaProcessingService cloudAIMediaProcessing)
        {
            _operationsService = operationsService;
            _cloudAIMediaProcessing = cloudAIMediaProcessing;
        }

        public async Task<MediaOperationPayload> MoveMediaAsync(
            MoveMediaRequest request,
            CancellationToken cancellationToken)
        {
            request.OperationId = request.OperationId ?? Guid.NewGuid().ToString("N");
            await _operationsService.MoveMediaAsync(request, cancellationToken);

            return new MediaOperationPayload(request.OperationId);
        }

        public async Task<ToggleMediaFavoritePayload> ToggleMediaFavoriteAsync(
            ToggleMediaFavoriteInput input, CancellationToken cancellationToken)
        {
            await _operationsService.ToggleFavoriteAsync(
                input.Id,
                input.IsFavorite,
                cancellationToken);

            return new ToggleMediaFavoritePayload(input.Id, input.IsFavorite);
        }

        public async Task<MediaOperationPayload> RecycleMediaAsync(
            RecycleMediaRequest input,
            CancellationToken cancellationToken)
        {
            RecycleMediaRequest request = input with
            {
                OperationId = input.OperationId ?? Guid.NewGuid().ToString("N")
            };

            await _operationsService.RecycleAsync(request, cancellationToken);

            return new MediaOperationPayload(request.OperationId);
        }

        public async Task<MediaOperationPayload> DeleteMediaAsync(
            DeleteMediaRequest input,
            CancellationToken cancellationToken)
        {
            DeleteMediaRequest request = input with
            {
                OperationId = input.OperationId ?? Guid.NewGuid().ToString("N")
            };

            await _operationsService.DeleteAsync(request, cancellationToken);

            return new MediaOperationPayload(request.OperationId);
        }

        public async Task<MediaOperationPayload> UpdateMediaMetadataAsync(
            UpdateMediaMetadataRequest input,
            CancellationToken cancellationToken)
        {
            input.OperationId = input.OperationId ?? Guid.NewGuid().ToString("N");
            await _operationsService.UpdateMetadataAsync(input, cancellationToken);

            return new MediaOperationPayload(input.OperationId);
        }

        public async Task<MediaOperationPayload> ReScanFacesAsync(
            RescanFacesRequest input,
            CancellationToken cancellationToken)
        {
            RescanFacesRequest request = input with
            {
                OperationId = input.OperationId ?? Guid.NewGuid().ToString("N")
            };

            await _operationsService.ReScanFacesAsync(request, cancellationToken);

            return new MediaOperationPayload(request.OperationId);
        }

        public async Task<AnalyseMediaPayload> AnalyseMediaAsync(
            AnalyseMediaInput input,
            CancellationToken cancellationToken)
        {
            MediaAI? mediaAi = await _cloudAIMediaProcessing
                .AnalyseMediaAsync(input.Id, cancellationToken);

            return new AnalyseMediaPayload(mediaAi);
        }
    }
}
