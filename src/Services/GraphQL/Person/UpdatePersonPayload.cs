using MagicMedia.Store;

namespace MagicMedia.GraphQL
{
    public class UpdatePersonPayload : Payload
    {
        public Person? Person { get; }

        public UpdatePersonPayload(Person person)
        {
            Person = person;
        }

        public UpdatePersonPayload(IReadOnlyList<UserError>? errors = null)
            : base(errors)
        {
        }
    }
}
