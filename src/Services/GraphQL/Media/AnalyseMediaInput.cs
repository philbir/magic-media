using System;
using System.Collections.Generic;
using MagicMedia.Store;

namespace MagicMedia.GraphQL
{
    public record AnalyseMediaInput(Guid Id, IEnumerable<AISource>? Sources);
}
