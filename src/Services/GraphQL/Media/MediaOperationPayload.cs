namespace MagicMedia.GraphQL
{
    public class MediaOperationPayload : Payload
    {
        public string? OperationId { get; }

        public MediaOperationPayload(string operationId)
        {
            OperationId = operationId;
        }

        public MediaOperationPayload(IReadOnlyList<UserError>? errors = null)
            : base(errors)
        {
        }
    }
}
