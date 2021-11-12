using MagicMedia.Store;

namespace MagicMedia.GraphQL
{


    public class CreateGroupPayload : Payload
    {
        public CreateGroupPayload(Group group)
        {
            Group = group;
        }

        public CreateGroupPayload(IReadOnlyList<UserError>? errors = null)
            : base(errors)
        {
        }

        public Group? Group { get; set; }
    }

}
