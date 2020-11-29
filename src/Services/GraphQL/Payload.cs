using System.Collections.Generic;

namespace MagicMedia.GraphQL
{
    public abstract class Payload
    {
        protected Payload(IReadOnlyList<UserError>? errors = null)
        {
            Errors = errors;
        }

        public IReadOnlyList<UserError>? Errors { get; }
    }

    public record UserError(string Message, string Code);
}
