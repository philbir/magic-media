using System;

namespace MagicMedia.Metadata;

public interface IDateTakenParser
{
    DateTime? Parse(string value);
}
