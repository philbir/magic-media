using System;

namespace MagicMedia.Store
{
    public record MediaHeaderData(Guid Id, string Filename, DateTimeOffset? DateTaken);
}
