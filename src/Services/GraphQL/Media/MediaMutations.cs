using System;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Types;
using MagicMedia.Operations;

namespace MagicMedia.GraphQL
{
    [ExtendObjectType(Name = "Mutation")]
    public class MediaMutations
    {
        private readonly IMediaOperationsService _operationsService;

        public MediaMutations(IMediaOperationsService operationsService)
        {
            _operationsService = operationsService;
        }

        public async Task<MediaOperationPayload> MoveMediaAsync(
            MoveMediaRequest request,
            CancellationToken cancellationToken)
        {
            request.OperationId = request.OperationId ?? Guid.NewGuid().ToString("N");
            await _operationsService.MoveMediaAsync(request, cancellationToken);

            return new MediaOperationPayload(request.OperationId);
        }

        public async Task<ToggleMediaFavoritePayload> ToggleMediaFavoriteAsync(ToggleMediaFavoriteInput input, CancellationToken cancellationToken)
        {
            await _operationsService.ToogleFavoriteAsync(input.Id, input.IsFavorite, cancellationToken);

            return new ToggleMediaFavoritePayload(input.Id, input.IsFavorite);
        }
    }

    public record ToggleMediaFavoriteInput(Guid Id, bool IsFavorite);
}
