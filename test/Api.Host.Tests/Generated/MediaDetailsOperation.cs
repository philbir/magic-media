using System;
using System.Collections;
using System.Collections.Generic;
using StrawberryShake;

namespace MagicMedia.Api.Host.Tests
{
    public class MediaDetailsOperation
        : IOperation<IMediaDetails>
    {
        public string Name => "mediaDetails";

        public IDocument Document => Queries.Default;

        public OperationKind Kind => OperationKind.Query;

        public Type ResultType => typeof(IMediaDetails);

        public Optional<System.Guid> Id { get; set; }

        public IReadOnlyList<VariableValue> GetVariableValues()
        {
            var variables = new List<VariableValue>();

            if (Id.HasValue)
            {
                variables.Add(new VariableValue("id", "Uuid", Id.Value));
            }

            return variables;
        }
    }
}
