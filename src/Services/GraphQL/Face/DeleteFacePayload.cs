namespace MagicMedia.GraphQL.Face
{
    public class DeleteFacePayload : Payload
    {
        public Guid Id { get; }

        public DeleteFacePayload(Guid id)
        {
            Id = id;
        }

        public DeleteFacePayload(IReadOnlyList<UserError>? errors = null)
            : base(errors)
        {
        }
    }

    public class DeleteFacesPayload : Payload
    {
        public IEnumerable<Guid>? Ids { get; }

        public DeleteFacesPayload(IEnumerable<Guid> ids)
        {
            Ids = ids;
        }

        public DeleteFacesPayload(IReadOnlyList<UserError>? errors = null)
            : base(errors)
        {
        }
    }
}
