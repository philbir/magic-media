using HotChocolate;
using HotChocolate.Types;
using MagicMedia.Messaging;

namespace MagicMedia.GraphQL
{
    [ExtendObjectType(Name = "Subscription")]
    public class MediaSubscriptions
    {
        [Subscribe]
        public NewMediaOperationTaskMessage OperationCompleted([Topic] string name, [EventMessage] NewMediaOperationTaskMessage payload)
        {
            return payload;
        }
    }
}
