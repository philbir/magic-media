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

        public async Task<MediaOperationPayload> MoveMediaAsync(MoveMediaRequest request, CancellationToken cancellationToken)
        {
            MediaOperationResult operation = await _operationsService.MoveMediaAsync(request, cancellationToken);

            return new MediaOperationPayload(operation.Id);
        }
    }
}
