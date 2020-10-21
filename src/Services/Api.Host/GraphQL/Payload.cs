using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MagicMedia.Api.GraphQL
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
